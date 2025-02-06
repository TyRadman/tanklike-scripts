using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Utils;
    using Combat;
    using Sound;

    /// <summary>
    /// Holds the tools that the player possesses and handles the logic of the input to start those tools.
    /// </summary>
    public class PlayerTools : TankTools, IInput, IDisplayedInput, IConstraintedComponent
    {
        [System.Serializable]
        public struct ToolToAdd
        {
            public ToolInfo Tool;
            public bool Add;
        }

        public bool IsConstrained { get; set; }

        [SerializeField] private List<ToolToAdd> _toolsToAdd;
        [SerializeField] private int _currentIndex = 0;
        [SerializeField] private Audio _onSwitchAudio;

        private bool _canChangeTools = true;
        private PlayerComponents _playerComponents;

        private const float CHANGING_TOOLS_COOLDOWN = 0.1f;

        public override void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;

            base.SetUp(_playerComponents);

            UpdateInputDisplay(_playerComponents.PlayerIndex);
            _toolsToAdd.FindAll(t => t.Add).ForEach(t => AddTool(t.Tool, t.Tool.ToolReference.GetMaxAmount()));
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Tool.name).performed += OnUseTool;
            playerMap.FindAction(c.Player.ToolSwitchRight.name).performed += OnMoveSelectionRight;
            playerMap.FindAction(c.Player.ToolSwitchLeft.name).performed += OnMoveSelectionLeft;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Tool.name).performed -= OnUseTool;
            playerMap.FindAction(c.Player.ToolSwitchRight.name).performed -= OnMoveSelectionRight;
            playerMap.FindAction(c.Player.ToolSwitchLeft.name).performed -= OnMoveSelectionLeft;
        }

        public void UpdateInputDisplay(int playerIndex)
        {
            string toolSelect = GameManager.Instance.InputManager.GetButtonBindingKey(
                   InputManager.Controls.Player.Tool.name, playerIndex);
            string switchNegative = GameManager.Instance.InputManager.GetButtonBindingKey(
                   InputManager.Controls.Player.ToolSwitchLeft.name, playerIndex);
            string switchPositive = GameManager.Instance.InputManager.GetButtonBindingKey(
                   InputManager.Controls.Player.ToolSwitchRight.name, playerIndex);
            string switchNegativeWithoutDash = switchNegative.Replace("-Pad", "");
            string switchPositiveWithoutDash = switchPositive.Replace("-Pad", "");
            GameManager.Instance.HUDController.PlayerHUDs[playerIndex].SetToolsKeys(toolSelect, switchNegativeWithoutDash, switchPositiveWithoutDash);

        }
        #endregion

        private void OnMoveSelectionRight(InputAction.CallbackContext ctx)
        {
            MoveSelection(true);
        }

        private void OnMoveSelectionLeft(InputAction.CallbackContext ctx)
        {
            MoveSelection(false);
        }

        private void MoveSelection(bool isNext)
        {
            if (!_canChangeTools || _tools.Count <= 0)
            {
                return;
            }

            // if there is only one tool available, then select it
            if (_tools.Count == 1)
            {
                _currentIndex = 0;
                _currentTool = _tools[_currentIndex];
            }

            _canChangeTools = false;
            Invoke(nameof(EnableChangingTools), CHANGING_TOOLS_COOLDOWN);

            // get the current tool
            _currentIndex = Helper.AddInRange(_currentIndex, isNext ? 1 : -1, 0, _tools.Count - 1);
            _currentTool = _tools[_currentIndex];

            // UI stuff and the left and right tools
            UpdateToolsPanel();
            GameManager.Instance.AudioManager.Play(_onSwitchAudio);
        }

        private void EnableChangingTools()
        {
            _canChangeTools = true;
        }

        public override void AddTool(ToolInfo tool, int amount)
        {
            base.AddTool(tool, amount);
            UpdateToolsPanel();
        }

        private void OnUseTool(InputAction.CallbackContext _)
        {
            UseTool();
        }

        public override void UseTool()
        {
            bool canUseTools = _currentTool != null && !IsConstrained && _currentTool.Tool != null && _currentTool.Tool.CanUseTool();

            if (!canUseTools)
            {
                return;
            }

            base.UseTool();

            GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex].UpdateActiveTool(_currentTool.Tool.GetAmount().ToString());
            // if the tank runs out of tool uses for the currently selected tool
            RemoveTool(_currentTool);
        }

        #region Remove Tool
        private void RemoveTool(ToolPack pack)
        {
            if (pack.Tool.GetAmount() > 0) return;

            // we're caching this because the easiest way to slide the tools after the current tool runs out, is to perform a slide similar to the one the player performs which changes the reference of _currentTool, therefore losing any trace of the previously active tool (the one the player ran out of) so we cache it
            ToolPack toolToRemove = pack;
            _tools.Remove(toolToRemove);

            // if there are any tools left other than the one the player ran out of
            if (_tools.Count > 0)
            {
                // set its UI
                MoveSelection(true);
                UpdateToolsPanel();
            }
            // if there are no other tools available
            else
            {
                _currentTool = null;
                GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex].SetActiveToolIconAsEmpty();
            }
        }

        public void RemoveToolByTag(ToolTags toolTag, int amount)
        {
            // cache the tool to remove
            var tool = _tools.Find(t => t.Tool.GetTag() == toolTag);
            // free the nodes it occupied
            //_PlayerComponents.Nodes.AddNodes(tool.Info.Nodes * amount);
            // set the amount of the tool to zero
            //tool.Tool.SetAmount(0);
            tool.Tool.AddAmount(-amount);
            // remove the tool
            RemoveTool(tool);
        }
        #endregion

        private void UpdateToolsPanel()
        {
            int nextIndex = Helper.AddInRange(_currentIndex, 1, 0, _tools.Count - 1);
            int previousIndex = Helper.AddInRange(_currentIndex, -1, 0, _tools.Count - 1);
            // if any of the indices is the same as the currect one, then it means we have one ability, in which case we pass null
            Tool previousTool = previousIndex == _currentIndex ? null : _tools[previousIndex].Tool;
            Tool nextTool = nextIndex == _currentIndex ? null : _tools[nextIndex].Tool;
            // ui stuff
            GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex].UpdateTools(_tools[_currentIndex].Tool, nextTool, previousTool);
        }

        #region Constraints
        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canUseTools = (constraints & AbilityConstraint.Tools) == 0;
            IsConstrained = !canUseTools;
        }
        #endregion

        #region IController
        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Restart()
        {
            base.Restart();
            SetUpInput(_playerComponents.PlayerIndex);
        }

        public override void Dispose()
        {
            base.Dispose();
            DisposeInput(_playerComponents.PlayerIndex);
        }
        #endregion
    }
}
