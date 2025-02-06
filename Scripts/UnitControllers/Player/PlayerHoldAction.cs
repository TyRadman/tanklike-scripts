using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Combat.Abilities;
    using Combat.SkillTree;
    using UI.HUD;
    using UI.InGame;
    using Sound;
    using Utils;

    public class PlayerHoldAction : TankOnHoldAction, IController, IInput, IDisplayedInput, IConstraintedComponent, IResumable, ISkill
    {
        public bool IsActive { get; private set; }
        public bool IsConstrained { get; set; }

        [HideInInspector] public bool IsHolding { get; private set; } = false;

        [SerializeField] private ChargeAttackBars _crosshairBars;
        [SerializeField] private bool _isLocked = true;

        [Header("Effects")]
        [SerializeField] private ParticleSystem _holdChargeEffect;
        [SerializeField] private ParticleSystem _holdReadyEffect;
        [SerializeField] private ParticleSystem _holdReleaseEffect;

        [Header("Audio")]
        [SerializeField] private Audio _holdChargeAudio;
        [SerializeField] private Audio _holdReadyAudio;
        [SerializeField] private Audio _holdReleaseAudio;

        private Dictionary<HoldAbilityHolder, HoldAbilityHolder> _abilityHolders = new Dictionary<HoldAbilityHolder, HoldAbilityHolder>();
        private PlayerComponents _playerComponents;
        private TankConstraints _constraints;
        private PlayerShooter _playerShooter;

        private HoldAbilityHolder _currentAbilityHolder;
        private Ability _currentChargeAbility;
        private Ability _currentPerfectChargeAbility;
        private SkillProfile _startingSkillProfile;

        private PlayerHUD _HUD;
        private Coroutine _holdCoroutine;
        private WaitForSeconds _abilityCooldownWait;
        private AudioSource _currentAudio;
        private InputAction _chargeInputAction;
        private Vector2 _perfectChargeRange = new Vector2(0.8f, 0.9f);
        private Vector2 _perfectChargeValueRange = new Vector2(0.0f, 0.1f);
        private float _chargeAmount;
        private float _holdDuration;
        private bool _successfulHold;
        private bool _canCharge = false;
        private bool _hasAbilityEquipped = false;
        private bool _hasPerfectChargeAbility = false;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _constraints = _playerComponents.Constraints;
            _playerShooter = _playerComponents.Shooter as PlayerShooter;

            _HUD = GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex];
            _crosshairBars.HidePerfectChargeBars();

            // get the starting skill
            _startingSkillProfile = GameManager.Instance.StartingSkillsDB.GetStartingChargeAttack(_playerComponents.PlayerIndex);

            if (_startingSkillProfile != null)
            {
                _startingSkillProfile.RegisterSkill(_playerComponents);

                if (!_isLocked)
                {
                    _startingSkillProfile.EquipSkill(_playerComponents);
                }
            }
            else 
            { 
                Debug.LogError($"No starting skill profile found for player {_playerComponents.PlayerIndex}");
                return;
            }
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            _chargeInputAction = playerMap.FindAction(c.Player.Hold.name);

            _chargeInputAction.performed += OnHoldDown;
            _chargeInputAction.canceled += OnHoldUp;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(_playerComponents.PlayerIndex, ActionMap.Player);

            if(_chargeInputAction == null)
            {
                Helper.ThrowInputActionError();
                return;
            }

            _chargeInputAction.performed -= OnHoldDown;
            _chargeInputAction.canceled -= OnHoldUp;
        }

        public void UpdateInputDisplay(int playerIndex)
        {
            // set the input key for the hold action
            int holdActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(InputManager.Controls.Player.Hold.name, playerIndex);

            // TODO: Skill tree fixes
            GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex].SetHoldDownInfo(_currentAbilityHolder.GetIcon(), Helper.GetInputIcon(holdActionIconIndex));
        }
        #endregion

        private void OnHoldDown(InputAction.CallbackContext context)
        {
            bool canNotHoldDown = !IsActive || !_canCharge || !_hasAbilityEquipped || IsConstrained;

            if (canNotHoldDown)
            {
                return;
            }

            _canCharge = false;

            _constraints.ApplyConstraints(true, _currentAbilityHolder.OnHoldConstraints);

            this.StopCoroutineSafe(_holdCoroutine);
            _holdCoroutine = StartCoroutine(HoldRoutine());
        }

        private void OnHoldUp(InputAction.CallbackContext context)
        {
            bool canNotHoldUp = !IsActive || !_hasAbilityEquipped || IsConstrained || !IsHolding;

            if (canNotHoldUp)
            {
                return;
            }

            IsHolding = false;
            _holdChargeEffect.Stop();
            StopChargeAudio();
            _constraints.ApplyConstraints(false, _currentAbilityHolder.OnHoldConstraints);

            if (_successfulHold)
            {
                OnFullyChargedAttack();
            }
            else if (_perfectChargeRange.HasFloatInRange(_chargeAmount) && _hasPerfectChargeAbility)
            {
                OnPerfectChargedAttack();
            }
            else
            {
                OnFailedChargedAttack();
            }

            this.StopCoroutineSafe(_holdCoroutine);
            StartCoroutine(HoldUpRoutine());
        }

        private void OnFullyChargedAttack()
        {
            _constraints.ApplyConstraints(true, _currentAbilityHolder.OnPerformConstraints);
            _currentChargeAbility.PerformAbility();
            _holdReadyEffect.Stop(true);
            _holdReleaseEffect.Play();
            StartCoroutine(AbilityCooldownRoutine());
        }

        private void OnPerfectChargedAttack()
        {
            _constraints.ApplyConstraints(true, _currentAbilityHolder.OnPerformConstraints);
            _currentPerfectChargeAbility.PerformAbility();
            _holdReadyEffect.Stop(true);
            _holdReleaseEffect.Play();
            StartCoroutine(AbilityCooldownRoutine());
        }

        private void OnFailedChargedAttack()
        {
            _playerShooter.Shoot();
            _canCharge = true;
        }

        private IEnumerator HoldRoutine()
        {
            float timer = 0f;
            _successfulHold = false;
            IsHolding = true;
            _holdChargeEffect.Play();
            _currentAudio = GameManager.Instance.AudioManager.Play(_holdChargeAudio, _holdDuration);
            _chargeAmount = 0f;

            while (timer < _holdDuration)
            {
                timer += Time.deltaTime;
                _chargeAmount = timer / _holdDuration;
                _HUD.SetHoldDownChargeAmount(1 - _chargeAmount, _playerComponents.PlayerIndex);

                UpdateBars();

                yield return null;
            }

            _HUD.SetHoldDownChargeAmount(0f, _playerComponents.PlayerIndex);
            _successfulHold = true;
            _holdChargeEffect.Stop();
            _holdReadyEffect.Play();
            StopChargeAudio();
            GameManager.Instance.AudioManager.Play(_holdReadyAudio);
        }

        private IEnumerator HoldUpRoutine()
        {
            float progress = Mathf.InverseLerp(1f, 0f, _crosshairBars.GetMainBarAmount());
            float timer = Mathf.Lerp(0, 0.2f, progress);

            while (timer < 0.2f)
            {
                timer += Time.deltaTime;
                float t = timer / 0.2f;
                _chargeAmount = Mathf.Clamp(1 - t, 0f, 1f);
                _HUD.SetHoldDownChargeAmount(1 - _chargeAmount, _playerComponents.PlayerIndex);

                UpdateBars();

                yield return null;
            }

            _crosshairBars.SetMainBarAmount(0f);
        }

        private void UpdateBars()
        {
            _crosshairBars.UpdateBarsValue(_chargeAmount);

            if (_hasPerfectChargeAbility)
            {
                _crosshairBars.UpdatePerfectChargeBarsValue(_chargeAmount, _perfectChargeRange, _perfectChargeValueRange);
            }
        }

        private IEnumerator AbilityCooldownRoutine()
        {
            yield return _abilityCooldownWait;
            _canCharge = true;
            _constraints.ApplyConstraints(false, _currentAbilityHolder.OnPerformConstraints);
        }

        #region Skill Addition
        public void AddSkill(SkillHolder newAbilityHolder)
        {
            if(newAbilityHolder == null)
            {
                Debug.LogError($"No ability passed");
                return;
            }

            if (newAbilityHolder is not HoldAbilityHolder holdAbilityHolder)
            {
                Helper.LogWrongSkillHolder(gameObject.name, typeof(HoldAbilityHolder).Name, newAbilityHolder.GetType().Name);
                return;
            }

            if (_abilityHolders.ContainsKey(holdAbilityHolder))
            {
                Debug.LogError($"Ability {holdAbilityHolder.Ability.name} exists");
                return;
            }

            // main ability
            HoldAbilityHolder newHolder = Instantiate(holdAbilityHolder);
            _abilityHolders.Add(holdAbilityHolder, newHolder);
            Ability newAbility = Instantiate(holdAbilityHolder.Ability);
            newHolder.SetAbility(newAbility);

            // perfect charge ability

            if (_currentAbilityHolder == null)
            {
                EquipSkill(holdAbilityHolder);
            }
        }

        public void EquipSkill(SkillHolder newAbilityHolder)
        {
            if (newAbilityHolder == null)
            {
                Debug.LogError($"No ability passed");
                return;
            }

            if (newAbilityHolder is not HoldAbilityHolder holdAbilityHolder)
            {
                Helper.LogWrongSkillHolder(gameObject.name, typeof(HoldAbilityHolder).Name, newAbilityHolder.GetType().Name);
                return;
            }

            if (!_abilityHolders.ContainsKey(holdAbilityHolder))
            {
                AddSkill(newAbilityHolder);
            }

            _hasAbilityEquipped = true;

            _currentAbilityHolder = _abilityHolders[holdAbilityHolder];
            _currentChargeAbility = _currentAbilityHolder.Ability;

            SetUpPerfectChargeValues(holdAbilityHolder);

            SetPerfectChargeValues();

            _currentChargeAbility.SetUp(_playerComponents);

            _holdDuration = holdAbilityHolder.HoldDownDuration;

            UpdateInputDisplay(_playerComponents.PlayerIndex);

            _abilityCooldownWait = new WaitForSeconds(holdAbilityHolder.Ability.GetDuration());
        }

        public void ReEquipSkill()
        {
            EquipSkill(_currentAbilityHolder);
        }
        #endregion

        private void SetUpPerfectChargeValues(HoldAbilityHolder abilityToEquip)
        {
            if (abilityToEquip.PerfectChargeAbility == null && _currentAbilityHolder.PerfectChargeAbility == null)
            {
                _crosshairBars.HidePerfectChargeBars();
                return;
            }

            if (abilityToEquip.PerfectChargeAbility != null && _currentAbilityHolder.PerfectChargeAbility == null)
            {
                Ability perfectChargeAbilityInstance = Instantiate(abilityToEquip.PerfectChargeAbility);
                _currentAbilityHolder.SetPerfectChargeAbility(perfectChargeAbilityInstance);
                _hasPerfectChargeAbility = true;
            }
        }

        public void AddPerfectChargeAbility(Ability perfectChargeAbility)
        {
            if (perfectChargeAbility == null)
            {
                Debug.LogError($"No ability passed");
                return;
            }

            if (_currentAbilityHolder == null)
            {
                Debug.LogError($"No ability holder equipped");
                return;
            }

            Ability perfectChargeAbilityInstance = Instantiate(perfectChargeAbility);
            _currentAbilityHolder.SetPerfectChargeAbility(perfectChargeAbilityInstance);
            _hasPerfectChargeAbility = true;

            SetPerfectChargeValues();
        }

        public void SetPerfectChargeValues()
        {
            if(_currentAbilityHolder == null || _currentAbilityHolder.PerfectChargeAbility == null)
            {
                return;
            }

            _currentPerfectChargeAbility = _currentAbilityHolder.PerfectChargeAbility;
            _perfectChargeRange = _currentAbilityHolder.PerfectChargeRange;
            _perfectChargeValueRange = new Vector2(0.0f, _perfectChargeRange.y - _perfectChargeRange.x);
            _crosshairBars.SetSize(_perfectChargeRange);
            _currentPerfectChargeAbility.SetUp(_playerComponents);
        }

        public HoldAbilityHolder GetAbilityHolder()
        {
            return _currentAbilityHolder;
        }

        // TODO: not needed as we're not doing the ability swapping
        public List<HoldAbilityHolder> GetAbilityHolders()
        {
            List<HoldAbilityHolder> abilities = new List<HoldAbilityHolder>();

            foreach (KeyValuePair<HoldAbilityHolder, HoldAbilityHolder> holder in _abilityHolders)
            {
                abilities.Add(holder.Value);
            }

            return abilities;
        }

        public void ForceStopHoldAction()
        {
            if (!IsHolding)
            {
                return;
            }

            _constraints.ApplyConstraints(false, _currentAbilityHolder.OnHoldConstraints);

            _successfulHold = false;

            this.StopCoroutineSafe(_holdCoroutine);

            _crosshairBars.ResetBarsAmount();

            StopChargeEffect();
        }

        private void StopChargeEffect()
        {
            _holdChargeEffect.Stop();
            _holdReadyEffect.Stop(true);
            _holdReleaseEffect.Stop();
            StopChargeAudio();
        }

        private void StopChargeAudio()
        {
            if (_currentAudio == null)
            {
                return;
            }

            _currentAudio.Stop();
            _currentAudio.clip = null;
        }

        #region Constraints
        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canUseAbility = (constraints & AbilityConstraint.HoldDownAction) == 0;

            if (IsConstrained == !canUseAbility)
            {
                return;
            }

            IsConstrained = !canUseAbility;

            if (IsConstrained)
            {
                ForceStopHoldAction();
            }
            else
            {
                ResumeComponent();
            }
        }

        public void ResumeComponent()
        {
            if(_isLocked)
            {
                return;
            }

            _canCharge = true;

            if (_chargeInputAction.inProgress)
            {
                OnHoldDown(new InputAction.CallbackContext());
            }
        }
        #endregion

        #region Lock/Unlock
        internal void Unlock()
        {
            _isLocked = false;
            _startingSkillProfile.EquipSkill(_playerComponents);
        }
        #endregion

        #region IController
        public void Activate()
        {
            IsActive = true;

            _canCharge = true;

            ResumeComponent();
        }

        public void Deactivate()
        {
            IsActive = false;

            ForceStopHoldAction();

            _canCharge = false;
        }

        public void Restart()
        {
            SetUpInput(_playerComponents.PlayerIndex);

            _crosshairBars.SetUp();
            _crosshairBars.ResetBarsAmount();
        }

        public void Dispose()
        {
            DisposeInput(_playerComponents.PlayerIndex);

            StopAllCoroutines();
            StopChargeAudio();
        }
        #endregion
    }
}
