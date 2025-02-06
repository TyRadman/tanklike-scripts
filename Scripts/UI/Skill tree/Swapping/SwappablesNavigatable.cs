using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UI.SkillTree
{
    using Combat.Abilities;
    using TankLike.Combat;
    using UI;
    using UnitControllers;
    using Utils;

    public class SwappablesNavigatable : Navigatable, IInput, IManager
    {
        public enum CellType
        {
            Weapon = 0, HoldDownAction = 1, SuperAbility = 2, BoostAbility = 3
        }

        [System.Serializable]
        public class SwappableGrouping
        {
            public SwappableCell Cell;
            public SwappableSkillsSelector SkillSelector;
            public CellType CellType;
        }

        [Header("References")]
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;
        [SerializeField] private int _playerIndex = 0;
        [SerializeField] private List<SwappableGrouping> _groups;

        private PlayerComponents _player;
        private SwappableGrouping _selectedSwappableGroup;
        private int _currentCellIndex = 0;

        #region IManager
        public override void SetUp()
        {
            base.SetUp();

            _selectedSwappableGroup = _groups[_currentCellIndex];
            _selectedSwappableGroup.Cell.Highlight();

            _groups.ForEach(g => g.SkillSelector.SetUp());
            _groups.ForEach(g => g.SkillSelector.SetParentNavigatable(this));
        }

        public override void Dispose()
        {
            base.Dispose();

        }
        #endregion

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Up.name).performed += NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed += NavigateDown;
            
            UIMap.FindAction(c.UI.Submit.name).performed += SelectAction; 
            UIMap.FindAction(c.UI.Cancel.name).performed += Close;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Up.name).performed -= NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed -= NavigateDown;

            UIMap.FindAction(c.UI.Submit.name).performed -= SelectAction;
            UIMap.FindAction(c.UI.Cancel.name).performed -= Close;
        }

        public override void NavigateUp(InputAction.CallbackContext _)
        {
            Navigate(Direction.Up);
        }

        public override void NavigateDown(InputAction.CallbackContext _)
        {
            Navigate(Direction.Down);
        }

        public void SelectAction(InputAction.CallbackContext _)
        {
            Select();
        }

        public void Close(InputAction.CallbackContext _)
        {
            Close();
        }
        #endregion

        public override void Load(int playerIndex = 0)
        {
            if (_playerIndex >= GameManager.Instance.PlayersManager.GetPlayersCount() - 1)
            {
                return;
            }

        }

        public override void Open(int playerIndex = 0)
        {
            base.Open(playerIndex);
            SetUpInput(playerIndex);

            _playerIndex = playerIndex;
            _selectedSwappableGroup.Cell.Highlight();

            _player = GameManager.Instance.PlayersManager.GetPlayer(_playerIndex);
            
            LoadSkillsIntoCells();

            _groups.ForEach(g => LoadSkillsBasedOnSkillType(g));
        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsOpened)
            {
                return;
            }

            base.Close(playerIndex);
        }

        // loads the skills that the player is currently equipped with
        public void LoadSkillsIntoCells()
        {
            _groups.Find(g => g.CellType == CellType.Weapon).Cell.SetSkill(_player.Shooter.GetWeaponHolder());
            _groups.Find(g => g.CellType == CellType.HoldDownAction).Cell.SetSkill(_player.ChargeAttack.GetAbilityHolder());
            _groups.Find(g => g.CellType == CellType.SuperAbility).Cell.SetSkill(_player.SuperAbilities.GetAbilityHolder());
            _groups.Find(g => g.CellType == CellType.BoostAbility).Cell.SetSkill(_player.BoostAbility.GetAbilityHolder());
        }

        public override void Navigate(Direction direction)
        {
            if (!IsOpened)
            {
                return;
            }

            base.Navigate(direction);
            
            _selectedSwappableGroup.Cell.Unhighlight();

            var newCell = _selectedSwappableGroup.Cell.ConnectedCells.Find(c => c.CellDirection == direction).Cell as SwappableCell;
            _selectedSwappableGroup = _groups.Find(g => g.Cell == newCell);

            _selectedSwappableGroup.Cell.Highlight();

            _skillNameText.text = _selectedSwappableGroup.Cell.Skill.GetName();
            _skillDescriptionText.text = _selectedSwappableGroup.Cell.Skill.GetDescription();
        }

        public override void Select()
        {
            if (!IsOpened)
            {
                return;
            }

            base.Select();

            // if we're selecting the already selected cell, then do nothing
            //LoadSkillsBasedOnSkillType(_groups.Find(g => g.Cell.CellHighlightState == SwappableCell.HighlightState.Highlighted));

            _selectedSwappableGroup.SkillSelector.Open();

            _selectedSwappableGroup.Cell.Unhighlight();

            IsOpened = false;
        }

        private void LoadSkillsBasedOnSkillType(SwappableGrouping group)
        {
            List<SkillHolder> skills = new List<SkillHolder>();
            
            switch (group.CellType)
            {
                case CellType.Weapon:
                    {
                        //_player.Shooter.GetWeaponHolders().ForEach(s => skills.Add(s));
                        PopulateSkillListForSkillsSelector(skills, _player.Shooter.GetWeaponHolder(), group);
                        return;
                    }
                case CellType.HoldDownAction:
                    {
                        _player.ChargeAttack.GetAbilityHolders().ForEach(s => skills.Add(s));
                        PopulateSkillListForSkillsSelector(skills, _player.ChargeAttack.GetAbilityHolder(), group);
                        return;
                    }
                case CellType.SuperAbility:
                    {
                        _player.SuperAbilities.GetAbilityHolders().ForEach(s => skills.Add(s));
                        PopulateSkillListForSkillsSelector(skills, _player.SuperAbilities.GetAbilityHolder(), group);
                        return;
                    }
                case CellType.BoostAbility:
                    {
                        _player.BoostAbility.GetAbilityHolders().ForEach(s => skills.Add(s));
                        PopulateSkillListForSkillsSelector(skills, _player.BoostAbility.GetAbilityHolder(), group);
                        return;
                    }
            }
        }

        private List<SkillHolder> PopulateSkillListForSkillsSelector(List<SkillHolder> playerSkills, SkillHolder equippedSkill, SwappableGrouping group)
        {
            if(playerSkills.Count == 0)
            {
                return null;
            }

            List<SkillHolder> skills = new List<SkillHolder>();
            playerSkills.ForEach(w => skills.Add(w));

            if (equippedSkill == null)
            {
                group.SkillSelector.SetEmpty();
                return null;
            }

            int abilityIndex = skills.IndexOf(equippedSkill);
            group.SkillSelector.LoadSkillsIntoCells(skills, abilityIndex);
            return skills;
        }

        public void LoadCellWithSkill(SkillHolder skill)
        {
            _selectedSwappableGroup.Cell.SetSkill(skill);

            skill.EquipSkill(_player);
        }

        public void ResumeControls()
        {
            IsOpened = true;
            _selectedSwappableGroup.Cell.Highlight();
        }
    }
}
