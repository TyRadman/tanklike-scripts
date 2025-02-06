using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Sound;
    using UI.HUD;
    using Utils;
    using Attributes;

    /// <summary>
    /// Player energy controller. Handles the player energy and healing ability.
    /// </summary>
    public class PlayerEnergy : MonoBehaviour, IController, IInput, IDisplayedInput, IUnitDataReciever
    {
        public bool IsActive { get; private set; }

        public System.Action OnEnergyFull { get; set; }
        /// <summary>
        /// Event that gets triggered when the player's energy goes from full to anything less than full.
        /// </summary>
        public System.Action OnEnergyEmpty { get; set; }

        [Header("Settings")]
        [SerializeField] private float _maxEnergy;
        [SerializeField] private float _healRequiredEnergy = 20f;
        [SerializeField] private float _healChargeDuration = 1.5f;
        [SerializeField] private int _healAmount = 20;
        [SerializeField, AllowCreationIfNull] private StatModifierType _energyFullStatType;

        [Header("Constraints")]
        [SerializeField] private AbilityConstraint _constraints;

        [Header("Effects")]
        [SerializeField] private ParticleSystem _chargeEffect;
        [SerializeField] private ParticleSystem _blastEffect;

        [Header("Audio")]
        [SerializeField] private Audio _healBlastAudio;

        private PlayerComponents _playerComponents;
        private PlayerHealth _health;
        private PlayerExperience _experience;
        private PlayerHUD _HUD;
        private Coroutine _holdCoroutine;
        private StatIconReference _statIconReference;
        private float _healMultiplier = 1f;
        private float _currentEnergy;
        private bool _canChargeEnergy = false;
        private float _lastZoom;
        private bool _isCharging;
        private bool _isEnergyFull = false;

        private const float XP_MULTIPLIER = 0.5f;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _health = _playerComponents.Health as PlayerHealth;
            _experience = _playerComponents.Experience;

            _HUD = GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex];

            UpdateInputDisplay(_playerComponents.PlayerIndex);
            _canChargeEnergy = true;

            _statIconReference = GameManager.Instance.StatIconReferenceDB.GetStatIconReference(_energyFullStatType);

            OnEnergyFull += AddEnergyFullStatIcon;
            OnEnergyEmpty += RemoveEnergyFullStatIcon;
        }

        private void AddEnergyFullStatIcon()
        {
            _HUD.StatModifiersDisplayer.AddIcon(_statIconReference);
        }

        private void RemoveEnergyFullStatIcon()
        {
            _HUD.StatModifiersDisplayer.RemoveIcon(_statIconReference);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Energy.name).performed += OnHoldDown;
            playerMap.FindAction(c.Player.Energy.name).canceled += OnHoldUp;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(_playerComponents.PlayerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Energy.name).performed -= OnHoldDown;
            playerMap.FindAction(c.Player.Energy.name).canceled -= OnHoldUp;
        }

        public void UpdateInputDisplay(int playerIndex)
        {
            int actionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(InputManager.Controls.Player.Energy.name, playerIndex);

            GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex].SetEnergyKey(Helper.GetInputIcon(actionIconIndex));
        }
        #endregion

        private void OnHoldDown(InputAction.CallbackContext context)
        {
            bool isHealthFull = _health.IsFull();
            bool hasEnoughEnergy = _currentEnergy >= _healRequiredEnergy;

            _lastZoom = GameManager.Instance.CameraManager.Zoom.ZoomAmount;

            bool canCharge = IsActive && hasEnoughEnergy && !isHealthFull && _canChargeEnergy;

            if (!canCharge)
            {
                return;
            }

            this.StopCoroutineSafe(_holdCoroutine);

            _holdCoroutine = StartCoroutine(HoldDownRoutine());
        }

        private IEnumerator HoldDownRoutine()
        {
            _isCharging = true;
            _playerComponents.Constraints.ApplyConstraints(true, _constraints);

            GameManager.Instance.CameraManager.PlayerCameraFollow.SetPlayerTargetToPlayerMode(_playerComponents.PlayerIndex);
            GameManager.Instance.CameraManager.Zoom.PerformFocusZoom();

            _chargeEffect.Play();

            float timer = 0f;

            while (_currentEnergy >= _healRequiredEnergy && !_health.IsFull())
            {
                timer += Time.deltaTime;

                if (timer >= _healChargeDuration)
                {
                    Heal();
                    timer = 0f;
                }

                yield return null;
            }

            _chargeEffect.Stop();

            _playerComponents.Constraints.ApplyConstraints(false, _constraints);

            GameManager.Instance.CameraManager.PlayerCameraFollow.SetPlayerTargetToLastMode(_playerComponents.PlayerIndex);
            GameManager.Instance.CameraManager.Zoom.SetToZoomValue(_lastZoom);
        }

        private void OnHoldUp(InputAction.CallbackContext context)
        {
            if(!_canChargeEnergy || !_isCharging)
            {
                return;
            }

            _isCharging = false;

            _chargeEffect.Stop();
            _playerComponents.Constraints.ApplyConstraints(false, _constraints);

            GameManager.Instance.CameraManager.PlayerCameraFollow.SetPlayerTargetToLastMode(_playerComponents.PlayerIndex);

            GameManager.Instance.CameraManager.Zoom.SetToZoomValue(_lastZoom);

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }

            if (_holdCoroutine != null)
            {
                StopCoroutine(_holdCoroutine);
            }
        }

        private void Heal()
        {
            int amountToheal = Mathf.CeilToInt((float)_healAmount * _healMultiplier);
            _playerComponents.Health.Heal(_healAmount);
            _blastEffect.Play();
            GameManager.Instance.AudioManager.Play(_healBlastAudio);
            _currentEnergy -= _healRequiredEnergy * (1 / _healMultiplier);
            CheckForFullEnergy();
            UpdateEnergyUI();
        }

        public void AddEnergy(float amount)
        {
            _currentEnergy += amount;

            CheckForFullEnergy();

            UpdateEnergyUI();
        }

        public void SetEnergyAmount(float amount)
        {
            _currentEnergy = amount;

            CheckForFullEnergy();

            UpdateEnergyUI();
        }

        private void CheckForFullEnergy()
        {
            if (_currentEnergy >= _maxEnergy)
            {
                int xpPointsToAdd = Mathf.CeilToInt((_currentEnergy - _maxEnergy) * XP_MULTIPLIER);
                _experience.AddExperience(xpPointsToAdd);

                _currentEnergy = _maxEnergy;

                if (!_isEnergyFull)
                {
                    OnEnergyFull?.Invoke();
                    _isEnergyFull = true;
                }
            }
            else
            {
                if (_isEnergyFull)
                {
                    OnEnergyEmpty?.Invoke();
                    _isEnergyFull = false;
                }
            }
        }

        public void MaxFillEnergy()
        {
            _currentEnergy = _maxEnergy;
            CheckForFullEnergy();
            UpdateEnergyUI();
        }

        private void UpdateEnergyUI()
        {
            _HUD.UpdateEnergyBar(_currentEnergy, _maxEnergy);
        }

        public float GetCurrentEnergy()
        {
            return _currentEnergy;
        }

        public float GetMaxEnergy()
        {
            return _maxEnergy;
        }

        public float GetEnergyPercentage()
        {
            return _currentEnergy / _maxEnergy;
        }

        public void EnableFuelUsage(bool enable)
        {
            _canChargeEnergy = enable;
        }

        public void SetHealMultiplier(float multiplier)
        {
            _healMultiplier *= multiplier;
        }

        public void ResetHealMultiplier()
        {
            _healMultiplier = 1f;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;

            // Cache last camera zoom amount
            _lastZoom = GameManager.Instance.CameraManager.Zoom.ZoomAmount;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            SetUpInput(_playerComponents.PlayerIndex);
            _currentEnergy = 0f;
            UpdateEnergyUI();
        }

        public void Dispose()
        {
            DisposeInput(_playerComponents.PlayerIndex);
            _currentEnergy = 0f;
            UpdateEnergyUI();

            this.StopCoroutineSafe(_holdCoroutine);


            OnEnergyFull = null;
        }
        #endregion

        #region IUnitDataReciever
        public void ApplyData(UnitData data)
        {
            if (data is not PlayerData playerData)
            {
                Debug.LogError($"Unit data is not of type PlayerData");
                return;
            }

            _maxEnergy = playerData.MaxEnergyPoints;
        }
        #endregion
    }
}
