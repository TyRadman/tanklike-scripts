using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace TankLike.UI.SkillTree
{
    using TankLike.Combat.Abilities;

    /// <summary>
    /// The individual group of skills per ability type i.e. super, hold, boost, etc.
    /// </summary>
    public class SwappableSkillsSelector : Navigatable
    {
        [Header("References")]
        [SerializeField] private List<SwappableCell> _cells;
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;
        private SwappableCell _selectedCell;
        private int _selectedCellIndex = 0;
        private SwappablesNavigatable _swappableNavigator;

        public override void SetUp()
        {
            base.SetUp();

            _cells.ForEach(c => c.SetUp());
            _selectedCell = _cells[_selectedCellIndex];
        }

        public void SetParentNavigatable(SwappablesNavigatable parent)
        {
            _swappableNavigator = parent;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            GameManager.Instance.InputManager.EnableInput(ActionMap.Empty);
            GameManager.Instance.InputManager.EnableInput(ActionMap.UI);
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;

            UIMap.FindAction(c.UI.Submit.name).performed += SelectAction;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;

            UIMap.FindAction(c.UI.Submit.name).performed -= SelectAction;
        }

        public override void NavigateLeft(InputAction.CallbackContext _)
        {
            Navigate(Direction.Left);
        }

        public override void NavigateRight(InputAction.CallbackContext _)
        {
            Navigate(Direction.Right);
        }

        public void SelectAction(InputAction.CallbackContext _)
        {
            Select();
        }
        #endregion

        #region Open and Close
        public override void Open(int playerIndex = 0)
        {
            base.Open(playerIndex);

            _selectedCell = _cells.Find(c => c.CellState == SwappableCell.PlayerSelectionState.Active);
            _selectedCell.Highlight();

            SetUpInput(PlayerIndex);
        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsOpened)
            {
                return;
            }

            base.Close(playerIndex);

            // change the active skill to the last selected one in the swappable navigatable
            _swappableNavigator.LoadCellWithSkill(_cells.Find(c => c.CellState == SwappableCell.PlayerSelectionState.Active).Skill);

            // dehighlight the selected skill
            _selectedCell.Unhighlight();

            DisposeInput(playerIndex);
        }
        #endregion

        /// <summary>
        /// loads the skills that the player has
        /// </summary>
        /// <param name="skills">Skills to load</param>
        /// <param name="selectedSkillIndex">The index of the skill selected by default</param>
        public void LoadSkillsIntoCells(List<SkillHolder> skills, int selectedSkillIndex)
        {
            _cells.ForEach(c => c.gameObject.SetActive(false));

            for (int i = 0; i < skills.Count; i++)
            {
                _cells[i].gameObject.SetActive(true);

                _cells[i].SetSkill(skills[i]);
                _cells[i].SetInactiveSkill();

                // find the indices of the next and previous cells. We can't put the values in the inspector because we enable and disable cells
                int previousIndex = i - 1 < 0 ? skills.Count - 1 : i - 1;
                int nextIndex = (i + 1) % skills.Count;
                _cells[i].ConnectedCells.Find(c => c.CellDirection == Direction.Left).Cell = _cells[previousIndex];
                _cells[i].ConnectedCells.Find(c => c.CellDirection == Direction.Right).Cell = _cells[nextIndex];
            }

            _cells[selectedSkillIndex].SetActiveSkill();
        }

        public void SetEmpty()
        {
            // set it as empty
        }

        #region Input Methods
        public override void Navigate(Direction direction)
        {
            if (!IsOpened)
            {
                return;
            }

            base.Navigate(direction);
            
            // dehighlight the previous cell
            _selectedCell.Unhighlight();

            // swap the cells
            SwappableCell newCell = _selectedCell.ConnectedCells.Find(c => c.CellDirection == direction).Cell as SwappableCell;

            _selectedCell = newCell;

            // highlight the previous cell
            _selectedCell.Highlight();

            UpdateSkillInfo();
        }

        private void UpdateSkillInfo()
        {
            _skillNameText.text = _selectedCell.Skill.GetName();
            _skillDescriptionText.text = _selectedCell.Skill.GetDescription();
        }

        public override void Select()
        {
            if (!IsOpened)
            {
                return;
            }

            base.Select();

            // if we're selecting the already selected cell, then do nothing
            if (_selectedCell.CellState != SwappableCell.PlayerSelectionState.Active)
            {
                // disable all the cells from being activated
                _cells.ForEach(c => c.SetInactiveSkill());

                // set the selected cell as the active cell
                _selectedCell.SetActiveSkill();

                // change it for the player
                SetSkillForPlayer();
            }

            Close(PlayerIndex);
            _swappableNavigator.ResumeControls();
        }

        private void SetSkillForPlayer()
        {
            if(_selectedCell == null)
            {
                Debug.Log("No cell");
            }

            if (_selectedCell.Skill == null)
            {
                Debug.Log("No skill");
            }

            if (_swappableNavigator == null)
            {
                Debug.Log("No navigator");
            }

            _swappableNavigator.LoadCellWithSkill(_selectedCell.Skill);
        }
        #endregion
    }
}
