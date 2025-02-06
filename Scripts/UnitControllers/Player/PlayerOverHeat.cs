using UnityEngine;

namespace TankLike.UnitControllers
{
    using UI.Notifications;
    using UI.InGame;
    using Sound;
    using Combat;

    /// <summary>
    /// Responsible for ammo management and the visuals related to it.
    /// </summary>
    public class PlayerOverHeat : MonoBehaviour, IController
    {
        public System.Action OnShotRecharged { get; set; }

        [Tooltip("How many bars will recharge a second")]
        [SerializeField][Range(0.1f, 10f)] protected float _shotsBarsIncrement = 0f;
        
        [Header("References")]
        [SerializeField] private SegmentedBar _segmentedBar;
        [SerializeField] private Audio _rechargeAudio;
        [SerializeField] private NotificationBarSettings_SO _notificationSettings;
        
        protected float _maxShotAmount;
        protected float _currectShotAmount;

        private float _beforeChargeTime = 0f;
        private int _lastShotsCount = 0;
        private bool _canRechargeShots = true;
        
        public const float PRIOR_RECHARGING_WAIT_TIME = 0.4f;

        private const float RECHARGE_SFX_TRIGGER_THRESHOLD = 0.2f;

        public bool IsActive { get; private set; }

        public void SetUp(IController controller)
        {
            _segmentedBar.SetUp();

            OnShotRecharged += () => GameManager.Instance.AudioManager.Play(_rechargeAudio);
        }

        private void Update()
        {
            if (!_canRechargeShots)
            {
                return;
            }

            IncreaseShotsOverTime(_shotsBarsIncrement * Time.deltaTime);
        }

        /// <summary>
        /// Updates the number of segments the crosshair's bar has.
        /// </summary>
        /// <param name="ammoCapacity">The number of segments to be made.</param>
        public void UpdateCrossHairBars(Weapon weapon)
        {
            _maxShotAmount = weapon.AmmoCapacity;
            _segmentedBar.SetCount(weapon.AmmoCapacity);
            _segmentedBar.SetTotalAmount(1f);

            if(weapon.AmmoRechargeSpeed > 0)
            {
                _shotsBarsIncrement = weapon.AmmoRechargeSpeed;
            }

            SetBeforeChargeDuration(weapon.GetCoolDownTime());

            _currectShotAmount = _maxShotAmount;
            _lastShotsCount = (int)_maxShotAmount;
        }

        private void IncreaseShotsOverTime(float amount)
        {
            if (_shotsBarsIncrement <= 0 || _currectShotAmount >= _maxShotAmount)
            {
                return;
            }

            if (_currectShotAmount < _maxShotAmount)
            {
                AddShotBarAmount(amount);
            }
        }

        public void ReduceAmmoBarByOne()
        {
            _lastShotsCount--;

            _segmentedBar.AddAmountToSegments(-1);

            _currectShotAmount--;
            _currectShotAmount = Mathf.Clamp(_currectShotAmount, 0, _maxShotAmount);

            _canRechargeShots = false;

            // cancel any invokes that might have been called before
            CancelInvoke();
            
            // enable recharging after a while
            Invoke(nameof(EnableRecharging), _beforeChargeTime);
        }

        private void EnableRecharging()
        {
            _canRechargeShots = true;
        }

        private void AddShotBarAmount(float amount)
        {
            _segmentedBar.AddAmountToSegments(amount);
            _currectShotAmount += amount;
            _currectShotAmount = Mathf.Clamp(_currectShotAmount, 0, _maxShotAmount);

            if(Mathf.FloorToInt(_currectShotAmount + RECHARGE_SFX_TRIGGER_THRESHOLD) > _lastShotsCount)
            {
                OnShotRecharged?.Invoke();
                _lastShotsCount++;
            }
        }

        public void FillBars()
        {
            //GameManager.Instance.NotificationsManager.PushCollectionNotification(_notificationSettings, 0, _playerComponents.PlayerIndex);
            _segmentedBar.AddAmountToSegments(_maxShotAmount - _currectShotAmount);
            _currectShotAmount = _maxShotAmount;
            _lastShotsCount = (int)_maxShotAmount;
        }

        public bool HasEnoughShots(int amount)
        {
            return _currectShotAmount >= amount;
        }

        private void SetBeforeChargeDuration(float weaponCooldown)
        {
            _beforeChargeTime = weaponCooldown + PRIOR_RECHARGING_WAIT_TIME;
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
            _currectShotAmount = _maxShotAmount;
            _lastShotsCount = (int)_maxShotAmount;
            _segmentedBar.SetTotalAmount(1f);
        }

        public void Dispose()
        {
            OnShotRecharged = null;
        }
        #endregion
    }
}
