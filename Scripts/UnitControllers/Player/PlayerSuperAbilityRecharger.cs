using System.Collections;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;
    using Combat.Abilities;
    using UI.HUD;

    public sealed class PlayerSuperAbilityRecharger : MonoBehaviour, IController
    {
        public System.Action OnSuperAbilityCharged { get; set; }

        [SerializeField] private AbilityRechargingMethod _currentRechargingMethod;

        private RechargingValues _rechargingMethods;
        private PlayerHUD _HUD;
        private PlayerComponents _playerComponents;

        /// <summary>
        /// A value between 0 and 1 where 1 indicating the ability is fully charged and ready to use
        /// </summary>
        private float _abilityChargeAmount = 0f;
        private float _chargePerSecond = 0f;
        private float _chargePerEnemyHit = 0f;
        private float _chargePerHit = 0f;

        private bool _canCharge = false;

        public bool IsActive { get; private set; }

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _HUD = GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex];
        }

        public RechargingValues GetRechargingMethod()
        {
            return _rechargingMethods;
        }

        #region Set Up
        public void SetUpRechargeMethods(RechargingValues rechargingMethods)
        {
            _rechargingMethods = rechargingMethods;

            // unsubscribe, just in case
            _playerComponents.Health.OnHit -= RechargeOnHit;
            _playerComponents.Shooter.UnregisterOnTargetHitCallback(RechargeOnEnemyHit);
            
            _currentRechargingMethod = rechargingMethods.RechargingMethod;
            // get the values that are gonna be used with every burst of super recharge
            _chargePerSecond = 1f / rechargingMethods.Time;
            _chargePerHit = 1f / rechargingMethods.NumberOfHits;
            _chargePerEnemyHit = 1f / rechargingMethods.NumberOfEnemyHits;

            // subscribe recharging methods to their corresponding events
            bool isOnPlayerHitRecharge = (rechargingMethods.RechargingMethod & AbilityRechargingMethod.OnPlayerHit) != 0;

            if (isOnPlayerHitRecharge)
            {
                _playerComponents.Health.OnHit += RechargeOnHit;
            }

            bool isOnEnemyHitRecharge = (rechargingMethods.RechargingMethod & AbilityRechargingMethod.OnEnemyHit) != 0;

            if (isOnEnemyHitRecharge)
            {
                _playerComponents.Shooter.RegisterOnTargetHitCallback(RechargeOnEnemyHit);
            }

            // we start recharging by default since we start with an empty bar
            //_abilityChargeAmount = 0f;

            // we check if the current recharging methods include charging the super over time
            if ((_currentRechargingMethod & AbilityRechargingMethod.OverTime) != 0)
            {
                StartCoroutine(OverTimeChargingProcess());
            }
        }
        #endregion

        public void EnableRecharging()
        {
            _canCharge = true;
            // set the amount charged of the ability to zero
            _abilityChargeAmount = 0f;

            // we check if the current recharging methods include charging the super over time
            if ((_currentRechargingMethod & AbilityRechargingMethod.OverTime) != 0)
            {
                StartCoroutine(OverTimeChargingProcess());
            }
        }

        public void DisableRecharging()
        {
            _canCharge = false;
        }

        private void RechargeOnHit()
        {
            if (_abilityChargeAmount >= 1f)
            {
                return;
            }

            AddAbilityChargeAmount(_chargePerHit);
        }

        private void RechargeOnEnemyHit()
        {
            if (_abilityChargeAmount >= 1f)
            {
                return;
            }

            AddAbilityChargeAmount(_chargePerEnemyHit);
        }

        private IEnumerator OverTimeChargingProcess()
        {
            while (_abilityChargeAmount < 1f)
            {
                AddAbilityChargeAmount(_chargePerSecond * Time.deltaTime);
                yield return null;
            }
        }

        private void AddAbilityChargeAmount(float amount)
        {
            if(_abilityChargeAmount >= 1f)
            {
                return;
            }

            if (!_canCharge)
            {
                return;
            }

            _abilityChargeAmount += amount;

            _HUD.SetSuperAbilityChargeAmount(_abilityChargeAmount, 0);

            if (_abilityChargeAmount >= 1f)
            {
                // reset the amount in case it overflowed
                _abilityChargeAmount = 1f;
                // enable the usage of the super ability
                _playerComponents.SuperAbilities.EnableAbilityUsage();

                _HUD.OnAbilityFullyCharged();

                OnSuperAbilityCharged?.Invoke();
            }
        }

        public void FullyChargeSuperAbility()
        {
            if (_abilityChargeAmount == 1f)
            {
                return;
            }

            AddAbilityChargeAmount(1f);
        }

        internal bool IsFullyCharged()
        {
            return _abilityChargeAmount >= 1f;
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
            _HUD.OnPlayerDeath();
        }
        #endregion
    }
}
