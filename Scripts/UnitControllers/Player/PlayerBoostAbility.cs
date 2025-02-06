using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Combat.SkillTree;
    using Combat.Abilities;
    using TankLike.Utils;
    using System;

    public class PlayerBoostAbility : MonoBehaviour, IController, IDisplayedInput, ISkill
    {
        [SerializeField] private SkillProfile _abilityToAdd;
        public bool IsActive { get; set; }
        private PlayerComponents _playerComponents;
        private Ability _currentAbility;
        private BoostAbilityHolder _currentAbilityHolder;
        private Dictionary<BoostAbilityHolder, BoostAbilityHolder> _abilityHolders = new Dictionary<BoostAbilityHolder, BoostAbilityHolder>();
        private float _updateDistance = 1f;
        private PlayerBoost _boost;
        private bool _abilityIsPerformed = false;

        [Header("Debug")]
        [SerializeField] private bool _usePlayerDataAbility = false;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _boost = _playerComponents.PlayerBoost;

            _abilityToAdd.RegisterSkill(_playerComponents);
            _abilityToAdd.EquipSkill(_playerComponents);
        }

        public void AddSkill(SkillHolder abilityHolder)
        {
            if (abilityHolder == null)
            {
                Debug.LogError($"No boost ability passed to {gameObject.name}");
                return;
            }

            if(abilityHolder is not BoostAbilityHolder boostAbilityHolder)
            {
                Helper.LogWrongSkillHolder(gameObject.name, typeof(BoostAbilityHolder).Name, abilityHolder.GetType().Name);
                return;
            }

            // set up the ability
            BoostAbilityHolder newAbilityHolder = Instantiate(boostAbilityHolder);

            Ability newAbility = Instantiate(newAbilityHolder.Ability);

            newAbilityHolder.SetAbility(newAbility);

            _abilityHolders.Add(boostAbilityHolder, newAbilityHolder);

            if(_currentAbilityHolder == null)
            {
                EquipSkill(boostAbilityHolder);
            }
        }

        public void EquipSkill(SkillHolder abilityHolder)
        {
            if (abilityHolder == null)
            {
                Debug.LogError($"No boost ability passed to {gameObject.name}");
                return;
            }

            if (abilityHolder is not BoostAbilityHolder boostAbilityHolder)
            {
                Helper.LogWrongSkillHolder(gameObject.name, typeof(BoostAbilityHolder).Name, abilityHolder.GetType().Name);
                return;
            }

            if (!_abilityHolders.ContainsKey(boostAbilityHolder))
            {
                AddSkill(boostAbilityHolder);
            }

            _currentAbilityHolder = _abilityHolders[boostAbilityHolder];

            _currentAbility = _currentAbilityHolder.Ability;

            _currentAbilityHolder.Ability.SetUp(_playerComponents);

            _updateDistance = _currentAbilityHolder.DistancePerActivation;

            // set up the UI
            UpdateInputDisplay(_playerComponents.PlayerIndex);

            ApplyBoostModifiers();

            // subscribe the ability to the playerBoost events
            _playerComponents.PlayerBoost.OnBoostStart += StartAbility;
            _playerComponents.PlayerBoost.OnBoostUpdate += UpdateAbility;
            _playerComponents.PlayerBoost.OnBoostEnd += EndAbility;
        }

        public void ReEquipSkill()
        {
            EquipSkill(_currentAbilityHolder);
        }

        public void UpdateInputDisplay(int playerIndex)
        {
            // set the input key for the boost ability
            string boostActionName = InputManager.Controls.Player.Boost.name;
            int boostActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(boostActionName, playerIndex);
            string keyIcon = Helper.GetInputIcon(boostActionIconIndex);

            Sprite abilityIcon = _currentAbilityHolder.GetIcon();

            GameManager.Instance.HUDController.PlayerHUDs[playerIndex].SetBoostInfo(abilityIcon, keyIcon);
        }

        private void ApplyBoostModifiers()
        {
            _boost.SetStartConsumptionRate(_currentAbilityHolder.FuelStartConsumptionRate);
            _boost.SetConsumptionRate(_currentAbilityHolder.FuelConsumptionRate);
            _boost.SetSpeedMultiplier(_currentAbilityHolder.SpeedMultiplier);
        }

        private void StartAbility()
        {
            if(_abilityIsPerformed)
            {
                return;
            }

            _abilityIsPerformed = true;
            _currentAbility.PerformAbility();
        }

        private void UpdateAbility()
        {
            if (_boost.DistanceTravelled > _updateDistance)
            {
                _boost.ResetDistanceCalculator();
                _currentAbility.PerformAbility();
            }
        }

        private void EndAbility()
        {
            _abilityIsPerformed = false;
        }

        public BoostAbilityHolder GetAbilityHolder()
        {
            return _currentAbilityHolder;
        }

        public List<BoostAbilityHolder> GetAbilityHolders()
        {
            List<BoostAbilityHolder> abilities = new List<BoostAbilityHolder>();

            foreach (KeyValuePair<BoostAbilityHolder, BoostAbilityHolder> holder in _abilityHolders)
            {
                abilities.Add(holder.Value);
            }

            return abilities;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {

        }

        public void Dispose()
        {
            EndAbility();
        }

        public void SetActiveAbility(BoostAbilityHolder boostAbility)
        {
            // set up the ability
            _currentAbilityHolder = boostAbility;

            _currentAbility = _currentAbilityHolder.Ability;

            _currentAbilityHolder.Ability = _currentAbility;

            if (!_currentAbility.IsSetUp)
            {
                _currentAbilityHolder.Ability.SetUp(_playerComponents);
            }

            _updateDistance = _currentAbilityHolder.DistancePerActivation;

            // set up the UI
            UpdateInputDisplay(_playerComponents.PlayerIndex);
        }
        #endregion
    }
}
