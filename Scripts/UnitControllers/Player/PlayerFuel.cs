using System.Collections;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;

    /// <summary>
    /// Handles the player's fuel consumption and refilling.
    /// </summary>
    public class PlayerFuel : MonoBehaviour, IController, IUnitDataReciever
    {
        public bool IsActive { get; private set; }

        [Header("Settings")]
        [SerializeField] private float _maxFuel;
        [SerializeField] private float _refillSpeed;
        [SerializeField] private float _refillDelay = 1f;

        private PlayerComponents _playerComponents;
        private Coroutine _refillCoroutine;
        private WaitForSeconds _refillWaitForSeconds;
        
        private System.Action OnFuelFull;
        private System.Action OnFuelNotFull;

        private float _currentFuel;
        private bool _canConsumeFuel = true;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;

            _refillWaitForSeconds = new WaitForSeconds(_refillDelay);
        }

        /// <summary>
        /// We only use this to consume fuel. Refilling it should have its own method.
        /// </summary>
        /// <param name="amount"></param>
        public void UseFuel(float amount)
        {
            if (!_canConsumeFuel)
            {
                return;
            }

            if (IsFuelFull())
            {
                OnFuelNotFull?.Invoke();
            }

            _currentFuel -= amount;
            _currentFuel = Mathf.Clamp(_currentFuel, 0f, _maxFuel);

            UpdateFuelUI();

            StartRefillProcess();
        }

        private void UpdateFuelUI()
        {
            GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex].UpdateFuelBar(_currentFuel, _maxFuel);
        }

        private void StartRefillProcess()
        {
            this.StopCoroutineSafe(_refillCoroutine);
            _refillCoroutine = StartCoroutine(RefillRoutine());
        }

        private IEnumerator RefillRoutine()
        {
            yield return _refillWaitForSeconds;

            while (_currentFuel < _maxFuel && IsActive)
            {
                _currentFuel += _refillSpeed * Time.deltaTime;
                UpdateFuelUI();

                yield return null;
            }

            if (IsActive)
            {
                OnFuelFull?.Invoke();
            }
        }

        #region Utilities
        public void RefillFuel()
        {
            _currentFuel = _maxFuel;
            UpdateFuelUI();
        }

        public bool HasEnoughFuel(float amount)
        {
            if (_currentFuel < amount)
            {
                return false;
            }

            return true;
        }

        public bool IsFuelFull()
        {
            return _currentFuel >= _maxFuel;
        }

        public bool HasEnoughFuel()
        {
            return _currentFuel > 0f;
        }

        public void EnableFuelConsumption(bool enable)
        {
            _canConsumeFuel = enable;
        }

        #region External Subscription
        public void AddOnFuelFullListener(System.Action onFullFuel, bool triggerIfConditionMet = true)
        {
            if(onFullFuel == null)
            {
                Debug.LogError("No event passed");
                return;
            }

            OnFuelFull += onFullFuel;

            if(IsFuelFull() && triggerIfConditionMet)
            {
                onFullFuel.Invoke();
            }
        }

        public void RemoveOnFuelFullListener(System.Action onFullFuel)
        {
            if (onFullFuel == null)
            {
                Debug.LogError("No event passed");
                return;
            }

            OnFuelFull -= onFullFuel;
        }

        public void AddOnFuelNotFullListener(System.Action onNotFullFuel, bool triggerIfConditionMet = true)
        {
            if (onNotFullFuel == null)
            {
                Debug.LogError("No event passed");
                return;
            }

            OnFuelNotFull += onNotFullFuel;

            if (!IsFuelFull() && triggerIfConditionMet)
            {
                onNotFullFuel.Invoke();
            }
        }

        public void RemoveOnFuelNotFullListener(System.Action onNotFullFuel)
        {
            if (onNotFullFuel == null)
            {
                Debug.LogError("No event passed");
                return;
            }

            OnFuelNotFull -= onNotFullFuel;
        }
        #endregion
        #endregion

        #region IController
        public void Activate()
        {
            IsActive = true;

            if (!IsFuelFull())
            {
                StartRefillProcess();
            }
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            _currentFuel = _maxFuel;
            UpdateFuelUI();
            RefillFuel();
        }

        public void Dispose()
        {
            _currentFuel = 0f;
            UpdateFuelUI();
            OnFuelFull = null;
        }
        #endregion

        #region IUnitDataReciever
        public void ApplyData(UnitData data)
        {
            if (data is not PlayerData playerData)
            {
                Debug.LogError("Unit data is not of type PlayerData");
                return;
            }

            _maxFuel = playerData.MaxFuel;
        }
        #endregion
    }
}
