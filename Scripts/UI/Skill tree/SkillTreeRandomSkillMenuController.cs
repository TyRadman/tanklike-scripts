using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

namespace TankLike.Combat.SkillTree
{
    using Upgrades;
    using UI;
    using UnitControllers;
    using UI.SkillTree;
    using TankLike.Sound;

    /// <summary>
    /// Controls the pop up that appears when a random skill in the skills tree is selected. It displays two skills to choose from.
    /// </summary>
    public class SkillTreeRandomSkillMenuController : Navigatable, IInput
    {
        [SerializeField] private Audio _navigationSFX;
        [SerializeField] private Audio _selectionSFX;
        [Header("References")]
        [SerializeField] private SkillTreeUIDisplayer _UIDisplayer;
        [SerializeField] private SkillTreeHolder _skillTreeHolder;
        [SerializeField] private SkillTreeUpgradeCell _firstCell;
        [SerializeField] private SkillTreeUpgradeCell _secondCell;
        [SerializeField] private GameObject _content;

        private PlayerComponents _player;
        private SkillTreeUpgradeCell _activeCell;
        private bool _canMove;
        private readonly float _navigationCoolDown = 0.1f;

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;
            UIMap.FindAction(c.UI.Submit.name).performed += SelectAction;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Right.name).performed -= NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Left.name).performed -= NavigateLeft;
            UIMap.FindAction(c.UI.Submit.name).performed -= SelectAction;
        }

        public override void NavigateRight(InputAction.CallbackContext _)
        {
            Navigate(Direction.Right);
        }

        public override void NavigateLeft(InputAction.CallbackContext _)
        {
            Navigate(Direction.Left);
        }

        private void SelectAction(InputAction.CallbackContext obj)
        {
            // carry over the selected upgrade
            SkillTreeCell activeCell = _skillTreeHolder.GetActiveCell();
            activeCell.SetUpgrade(_activeCell.OnUnlockedUpgrade);
            _activeCell.OnUnlockedUpgrade.GetUpgrade().ApplyUpgrade();

            GameManager.Instance.AudioManager.Play(_selectionSFX);
            Close(PlayerIndex);
        }
        #endregion

        #region Open and Close
        public override void Open(int playerIndex)
        {
            base.Open(playerIndex);

            _player = GameManager.Instance.PlayersManager.GetPlayer(playerIndex);

            _canMove = true;

            _content.SetActive(true);

            _firstCell.Highlight();
            _secondCell.Unhighlight();
            _activeCell = _firstCell;
            UpdateUI();

            // disable UI that is not needed
            _UIDisplayer.UpdateSkillState(CellState.None);
            _UIDisplayer.UpdateSkillPoints(false, 0, 0);

            SetUpInput(playerIndex);
        }

        public override void Close(int playerIndex)
        {
            base.Close(playerIndex);

            _content.SetActive(false);
            DisposeInput(playerIndex);
            _skillTreeHolder.EnableSkillTreeHolder();
        }
        #endregion

        public override void Navigate(Direction direction)
        {
            if (!IsOpened || !_canMove)
            {
                return;
            }

            base.Navigate(direction);

            SkillsConnectedCell nextCell = _activeCell.ConnectedCells.Find(c => c.CellDirection == direction);

            if (nextCell == null || nextCell.Cell == null)
            {
                return;
            }

            _activeCell.Unhighlight();
            _activeCell = nextCell.Cell as SkillTreeUpgradeCell;
            _activeCell.Highlight();
            
            UpdateUI();

            _canMove = false;
            Invoke(nameof(EnableMovement), _navigationCoolDown);
            GameManager.Instance.AudioManager.Play(_navigationSFX);
        }

        #region Update UI
        private void UpdateUI()
        {
            UpdateName();
            UpdateDescription();
        }

        private void UpdateName()
        {
            _UIDisplayer.UpdateName(_activeCell.GetName());
        }

        private void UpdateDescription()
        {
            _UIDisplayer.UpdateDescription(_activeCell.GetDescription());
        }
        #endregion

        private void EnableMovement()
        {
            _canMove = true;
        }

        public void SetSkillProfiles(SkillUpgrade firstSkill, SkillUpgrade secondCell)
        {
            firstSkill.RefreshProperties();
            secondCell.RefreshProperties();
            _firstCell.SetUpgrade(firstSkill);
            _secondCell.SetUpgrade(secondCell);
        }

        public void SetCellIconColor(Color color)
        {
            _firstCell.SetIconColor(color);
            _secondCell.SetIconColor(color);
        }
    }
}
