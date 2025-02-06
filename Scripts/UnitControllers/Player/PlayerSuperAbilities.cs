using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Combat.Abilities;
    using UI.HUD;
    using Combat.SkillTree;
    using Misc;
    using Utils;

    /// <summary>
    /// Manages the player's super abilities, including input handling, ability usage, and visual effects.
    /// </summary>
    public class PlayerSuperAbilities : TankSuperAbilities, IInput, IDisplayedInput, IEquippableWeapon//, IResumable
    {
        public bool IsWeaponEquipped { get; set; } = false;

        [SerializeField] private ParticleSystem _superReadyEffect;
        [SerializeField] private AirIndicator _IndicatorPrefab;
        [SerializeField] private bool _isLocked = true;

        private PlayerComponents _playerComponents;
        private TankConstraints _constraints;
        private PlayerWeaponSwapper _weaponSwapper;
        private AirIndicator _currentIndicator;
        private PlayerSuperAbilityRecharger _abilityRecharger;
        private Coroutine _holdCoroutine;
        private PlayerHUD _HUD;
        private InputAction _aimInputAction; // keep this in case we go back to aiming
        private SkillProfile _startSuperAbility;

        private string _superActivationInput;
        private float _abilityUsageDuration;
        private bool _consumesChargeOnSuperUse = true;
        private bool _isHolding = false;
        private bool _isAbilityReady = false;
        private bool _isAbilityBeingPerformed = false;

        /// <summary>
        /// Sets up the player's super abilities.
        /// </summary>
        /// <param name="controller">The player components controller.</param>
        public override void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;

            _abilityRecharger = _playerComponents.SuperRecharger;

            _weaponSwapper = _playerComponents.PlayerWeaponSwapper;

            base.SetUp(_playerComponents);

            SpawnIndicator();

            _constraints = _playerComponents.Constraints;
            _HUD = GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex];

            SetupStartingAbility();

            // subscribe to when the super is fully charged to display the input for it
            _abilityRecharger.OnSuperAbilityCharged += OnSuperAbilityRecharged;
        }

        private void SetupStartingAbility()
        {
            _startSuperAbility = GameManager.Instance.StartingSkillsDB.GetStartingSuperAbility(_playerComponents.PlayerIndex);

            if (_startSuperAbility != null)
            {
                _startSuperAbility.RegisterSkill(_playerComponents);

                if (!_isLocked)
                {
                    _startSuperAbility.EquipSkill(_playerComponents);
                }
            }
            else
            {
                Debug.Log($"No starting super ability found for player {_playerComponents.PlayerIndex}");
            }
        }

        /// <summary>
        /// Spawns the indicator for the super ability.
        /// </summary>
        private void SpawnIndicator()
        {
            _currentIndicator = Instantiate(_IndicatorPrefab);
            _currentIndicator.SetUp(_playerComponents);
        }

        /// <summary>
        /// Handles the event when the super ability is fully charged.
        /// </summary>
        private void OnSuperAbilityRecharged()
        {
            _currentIndicator.SetActiveColor();

            if (IsWeaponEquipped)
            {
                DisplayActivationInput();
            }
        }

        #region Input
        /// <summary>
        /// Sets up the input actions for the player's super abilities.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            _aimInputAction = playerMap.FindAction(c.Player.Special_Hold.name);

            //_aimInputAction.performed += OnHoldDown;
            //_aimInputAction.canceled += OnHoldUp;

            playerMap.FindAction(c.Player.Shoot.name).performed += PerformSuper;

            _superActivationInput = c.Player.Shoot.name;
        }

        /// <summary>
        /// Disposes the input actions for the player's super abilities.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);

            //_aimInputAction.performed -= OnHoldDown;
            //_aimInputAction.canceled -= OnHoldUp;

            playerMap.FindAction(c.Player.Shoot.name).performed -= PerformSuper;
        }

        /// <summary>
        /// Updates the input display for the player's super abilities.
        /// </summary>
        /// <param name="playerIndex">The index of the player.</param>
        public void UpdateInputDisplay(int playerIndex)
        {
            int superActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(InputManager.Controls.Player.Shoot.name, playerIndex);

            _HUD.SetSuperAbilityInfo(_currentSuperAbilityHolder.GetIcon(), Helper.GetInputIcon(superActionIconIndex));
        }
        #endregion

        #region Hold Inputs
        //private bool CanAimWithSuper()
        //{
        //    return _isAbilityReady && _isAbilityEquipped && !IsConstrained && IsActive && IsWeaponEquipped;
        //}

        //private void OnHoldDown(InputAction.CallbackContext _)
        //{
        //    if (!CanAimWithSuper())
        //    {
        //        return;
        //    }

        //    this.StopCoroutineSafe(_holdCoroutine);

        //    //DisplayActivationInput();

        //    //_constraints.ApplyConstraints(true, _currentSuperAbilityHolder.OnAimConstraints);

        //    _currentIndicator.StartIndicator();

        //    // check for constraints and apply them
        //    _currentSuperAbilityHolder.Ability.OnAbilityHoldStart();
        //    _holdCoroutine = StartCoroutine(HoldRoutine());
        //}

        /// <summary>
        /// Displays the activation input for the super ability.
        /// </summary>
        private void DisplayActivationInput()
        {
            _playerComponents.InGameUIController.DisplayInput(_superActivationInput);
        }

        /// <summary>
        /// Hides the activation input for the super ability.
        /// </summary>
        private void HideActivationInput()
        {
            _playerComponents.InGameUIController.HideInput();
        }

        /// <summary>
        /// Coroutine for handling the hold input for the super ability.
        /// </summary>
        private IEnumerator HoldRoutine()
        {
            _isHolding = true;
            _playerComponents.Shooter.EnableShooting(false);

            // while the player is holding the button down and the super hasn't been fully charged for more than the cancel duration
            while (_isHolding)
            {
                _currentIndicator.UpdateIndicator();
                _currentSuperAbilityHolder.Ability.OnAbilityHoldUpdate();
                yield return null;
            }
        }

        //private void OnHoldUp(InputAction.CallbackContext _)
        //{
        //    if (!_isHolding)
        //    {
        //        return;
        //    }

        //    this.StopCoroutineSafe(_holdCoroutine);

        //    _constraints.ApplyConstraints(false, _currentSuperAbilityHolder.OnAimConstraints);

        //    _currentIndicator.EndIndicator();
        //    _isHolding = false;
        //    _playerComponents.Shooter.EnableShooting(true);
        //}

        /// <summary>
        /// Performs the super ability when the input action is triggered.
        /// </summary>
        /// <param name="_">The input action context.</param>
        public void PerformSuper(InputAction.CallbackContext _)
        {
            if (!_isAbilityReady && IsWeaponEquipped)
            {
                _weaponSwapper.ActivateBaseWeapon();
                return;
            }

            if (!_isHolding || IsConstrained || !IsActive)
            {
                return;
            }

            _isHolding = false;

            this.StopCoroutineSafe(_holdCoroutine);

            UseAbility();
        }
        #endregion

        /// <summary>
        /// Uses the super ability.
        /// </summary>
        public override void UseAbility()
        {
            if (!_isAbilityReady || IsConstrained)
            {
                return;
            }

            HideActivationInput();

            _constraints.ApplyConstraints(false, _currentSuperAbilityHolder.OnAimConstraints);
            _constraints.ApplyConstraints(true, _currentSuperAbilityHolder.OnPerformConstraints);

            _isAbilityReady = false;

            // must be called before the ability starts for when the ability wants to override the crosshair's speed. Remember that the indicator resets the indicator's speed when it ends.
            _currentIndicator.EndIndicator();

            base.UseAbility();

            // start cooldown counter
            StartCoroutine(AbilityPerformanceTimer());

            PlayVFX();

            if (!_consumesChargeOnSuperUse)
            {
                EnableAbilityUsage();
            }
        }

        /// <summary>
        /// Sets a timer for how long it takes the ability to be fully performed before allowing the player to recharge it again.
        /// </summary>
        private IEnumerator AbilityPerformanceTimer()
        {
            float time = 0f;
            _HUD.OnAbilityChargeEmptied();

            _abilityRecharger.DisableRecharging();
            _isAbilityBeingPerformed = true;

            // count down process
            while (time < _abilityUsageDuration)
            {
                time += Time.deltaTime;
                _HUD.SetSuperAbilityChargeAmount(1 - time / _abilityUsageDuration, 0);
                yield return null;
            }

            _isAbilityBeingPerformed = false;
            OnAbilityFinishedPerforming();
        }

        /// <summary>
        /// Handles the logic when the ability has finished performing.
        /// </summary>
        private void OnAbilityFinishedPerforming()
        {
            _constraints.ApplyConstraints(false, _currentSuperAbilityHolder.OnPerformConstraints);

            _abilityRecharger.EnableRecharging();

            // enable these and comment the line after them to disable swapping back to the weapon after performing the super ability
            //OnSuperAbilityEquipped();
            //SetIndicatorColorBasedOnChargeState();

            _weaponSwapper.ActivateBaseWeapon();
        }

        #region Ability Addition
        /// <summary>
        /// Adds a new skill to the player's super abilities.
        /// </summary>
        /// <param name="newAbilityHolder">The new skill holder.</param>
        public override void AddSkill(SkillHolder newAbilityHolder)
        {
            base.AddSkill(newAbilityHolder);
        }

        /// <summary>
        /// Equips a new skill to the player's super abilities.
        /// </summary>
        /// <param name="newAbilityHolder">The new skill holder.</param>
        public override void EquipSkill(SkillHolder newAbilityHolder)
        {
            if (newAbilityHolder == null)
            {
                Debug.Log($"No ability passed to {gameObject.name}");
                return;
            }

            if (newAbilityHolder is not SuperAbilityHolder superAbilityHolder)
            {
                Helper.LogWrongSkillHolder(gameObject.name, typeof(BoostAbilityHolder).Name, newAbilityHolder.GetType().Name);
                return;
            }

            if (!_superAbilityHolders.ContainsKey(superAbilityHolder))
            {
                AddSkill(superAbilityHolder);
            }

            _currentSuperAbilityHolder = _superAbilityHolders[superAbilityHolder];

            if (!_currentSuperAbilityHolder.Ability.IsSetUp)
            {
                _currentSuperAbilityHolder.Ability.SetUp(_playerComponents);
            }

            _abilityUsageDuration = _currentSuperAbilityHolder.Ability.GetDuration();
            //_isAbilityReady = false;

            _lastSuperAbilityHolder = superAbilityHolder;
            // set up ability recharging methods
            _abilityRecharger.SetUpRechargeMethods(_currentSuperAbilityHolder.RechargeInfo);

            _currentSuperAbilityHolder.Ability.SetUpIndicatorSpecialValues(_currentIndicator);

            UpdateInputDisplay(_playerComponents.PlayerIndex);
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Enables the usage of the super ability.
        /// </summary>
        public void EnableAbilityUsage()
        {
            _isAbilityReady = true;
            _superReadyEffect.Play();
        }

        /// <summary>
        /// Gets the current super ability holder.
        /// </summary>
        /// <returns>The current super ability holder.</returns>
        public SuperAbilityHolder GetAbilityHolder()
        {
            return _currentSuperAbilityHolder;
        }

        /// <summary>
        /// Gets all the super ability holders.
        /// </summary>
        /// <returns>A list of all super ability holders.</returns>
        public List<SuperAbilityHolder> GetAbilityHolders()
        {
            List<SuperAbilityHolder> abilities = new List<SuperAbilityHolder>();

            foreach (var ability in _superAbilityHolders)
            {
                abilities.Add(ability.Value);
            }

            return abilities;
        }

        /// <summary>
        /// Enables or disables charge consumption on super ability use.
        /// </summary>
        /// <param name="enable">True to enable charge consumption, false to disable.</param>
        public void EnableChargeConsumption(bool enable)
        {
            _consumesChargeOnSuperUse = enable;
        }

        /// <summary>
        /// Plays the visual effects for the super ability.
        /// </summary>
        private void PlayVFX()
        {
            ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Buffs.SuperAbility;
            vfx.transform.parent = transform;
            vfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
            vfx.gameObject.SetActive(true);
            vfx.Play();
            _superReadyEffect.Stop(true);
        }

        /// <summary>
        /// Checks if the super ability is fully charged.
        /// </summary>
        /// <returns>True if the super ability is fully charged, false otherwise.</returns>
        public bool IsSuperAbilityCharged()
        {
            return _abilityRecharger.IsFullyCharged();
        }
        #endregion

        #region Weapon Equipment
        /// <summary>
        /// Equips the super ability as a weapon.
        /// </summary>
        public void Equip()
        {
            IsWeaponEquipped = true;

            OnSuperAbilityEquipped();

            SetIndicatorColorBasedOnChargeState();
        }

        /// <summary>
        /// Handles the logic when the super ability is equipped.
        /// </summary>
        private void OnSuperAbilityEquipped()
        {
            this.StopCoroutineSafe(_holdCoroutine);

            _constraints.ApplyConstraints(true, _currentSuperAbilityHolder.OnAimConstraints);

            _currentIndicator.StartIndicator();

            // check for constraints and apply them
            _currentSuperAbilityHolder.Ability.OnAbilityHoldStart();

            _holdCoroutine = StartCoroutine(HoldRoutine());
        }

        /// <summary>
        /// Sets the color of the indicator based on the charge state of the super ability.
        /// </summary>
        internal void SetIndicatorColorBasedOnChargeState()
        {
            if (IsSuperAbilityCharged())
            {
                DisplayActivationInput();
                _currentIndicator.SetActiveColor();
            }
            else
            {
                HideActivationInput();
                _currentIndicator.SetInactiveColor();
            }
        }

        /// <summary>
        /// Unequips the super ability as a weapon.
        /// </summary>
        public void Unequip()
        {
            IsWeaponEquipped = false;

            this.StopCoroutineSafe(_holdCoroutine);

            _constraints.ApplyConstraints(false, _currentSuperAbilityHolder.OnAimConstraints);

            _currentIndicator.EndIndicator();
            _isHolding = false;
            _playerComponents.Shooter.EnableShooting(true);
            _currentSuperAbilityHolder.Ability.OnAbilityInterrupted();

            HideActivationInput();
        }

        /// <summary>
        /// Checks if the super ability can be equipped.
        /// </summary>
        /// <returns>True if the super ability can be equipped, false otherwise.</returns>
        public bool CanEquip()
        {
            return !_isAbilityBeingPerformed && !_isLocked;
        }

        /// <summary>
        /// Checks if the super ability can be unequipped.
        /// </summary>
        /// <returns>True if the super ability can be unequipped, false otherwise.</returns>
        public bool CanUnequip()
        {
            return !_isAbilityBeingPerformed && !_isLocked;
        }
        #endregion

        #region Constraints
        /// <summary>
        /// Applies the specified constraints to the super ability.
        /// </summary>
        /// <param name="constraints">The constraints to apply.</param>
        public override void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canUseAbility = (constraints & AbilityConstraint.SuperAbility) == 0;

            if (IsConstrained == !canUseAbility)
            {
                return;
            }

            IsConstrained = !canUseAbility;

            if (!IsConstrained && IsWeaponEquipped && IsSuperAbilityCharged())
            {
                ResumeComponent();
            }
        }

        public void ResumeComponent()
        {
            //if (_aimInputAction.inProgress)
            //{
            //    OnHoldDown(new InputAction.CallbackContext());
            //}

            //if (IsWeaponEquipped)
            //{
            //    DisplayActivationInput();
            //}
        }
        #endregion

        #region Lock/Unlock
        internal void Unlock()
        {
            _isLocked = false;

            _abilityRecharger.EnableRecharging();
            _startSuperAbility.EquipSkill(_playerComponents);
        }
        #endregion

        #region IController
        public override void Activate()
        {
            base.Activate();
            ResumeComponent();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Restart()
        {
            base.Restart();

            SetUpInput(_playerComponents.PlayerIndex);

            _HUD.SetSuperAbilityChargeAmount(0f, 0);
        }

        public override void Dispose()
        {
            base.Dispose();

            _currentIndicator.EndIndicator();

            DisposeInput(_playerComponents.PlayerIndex);

            StopAllCoroutines();
        }
        #endregion
    }

    [System.Flags]
    public enum AbilityRechargingMethod
    {
        None = 0,
        OnEnemyHit = 1,
        OverTime = 2,
        OnPlayerHit = 4
    }
}