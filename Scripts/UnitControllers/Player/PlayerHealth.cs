using UnityEngine;

namespace TankLike.UnitControllers
{
    using UI.HUD;
    using Combat;
    using Cam;
    using Utils;
    using UI.DamagePopUp;

    public class PlayerHealth : TankHealth
    {
        private PlayerComponents _playerComponents;
        private PlayerHUD _HUD;

        public override void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            
            base.SetUp(_playerComponents);

            _HUD = GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex];

            OnHealthValueChanged += _HUD.UpdateHealthBar;
        }

        public override void TakeDamage(DamageInfo damageInfo)
        {
            if (!_canTakeDamage || IsConstrained || IsDead)
            {
                return;
            }

            base.TakeDamage(damageInfo);

            if(_currentHealth != 0f)
            {
                GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.HIT);
            }
        }

        public override void Die()
        {
            base.Die();
            gameObject.SetActive(false);
            _playerComponents.TankBodyParts.HandlePartsExplosion();
        }

        public override void SetHealthPercentage(float value, bool playEffects = true)
        {
            value = Mathf.Clamp01(value);

            int expectedHealth = Mathf.CeilToInt((float)_maxHealth * value);

            if(!playEffects)
            {
                int health = expectedHealth - _currentHealth;

                _currentHealth = expectedHealth;
                _lastCurrentHealth = expectedHealth; // To avoid playing the damage effects (when lastCurrentHealth < currentHealth)
                //UpdateHealthUI();

                // Display pop up
                if (PopUpAnchor != null)
                {
                    DamagePopUpType type = health < 0 ? DamagePopUpType.Damage : DamagePopUpType.Heal;
                    GameManager.Instance.DamagePopUpManager.DisplayPopUp(type, health, PopUpAnchor.Anchor);
                }

                return;
            }

            // if the current health is greater than the new health, then the change is a damage, otherwise, it's a heal
            if (_currentHealth >= expectedHealth)
            {
                int damageAmountToInflict = _currentHealth - expectedHealth;

                DamageInfo damageInfo = DamageInfo.Create()
                    .SetDamage(damageAmountToInflict)
                    .Build();

                TakeDamage(damageInfo);
            }
            else
            {
                int amountTohealth = expectedHealth - _currentHealth;
                Heal(amountTohealth);
            }
        }

        #region IController
        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Restart()
        {
            base.Restart();
            RestoreFullHealth();
            _HUD.SetupHealthBar(_maxHealth);

            OnHit += _playerComponents.Shield.ActivateShield;
            OnDeath += _playerComponents.OnDeathHandler;

            int layer = GameManager.Instance.Constants.Alignments.Find(a => a.Alignment == TankAlignment.PLAYER).LayerNumber;
            SetDamageDetectorsLayer(layer);

            if (GameManager.Instance.PlayersManager != null)
            {
                OnDeath += GameManager.Instance.PlayersManager.ReportPlayerDeath;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _currentHealth = 0;

            OnHit -= _playerComponents.Shield.ActivateShield;
            OnDeath -= _playerComponents.OnDeathHandler;

            if (GameManager.Instance.PlayersManager != null)
            {
                OnDeath -= GameManager.Instance.PlayersManager.ReportPlayerDeath;
            }
        }
        #endregion

        #region IUnitDataReciever
        public override void ApplyData(UnitData data)
        {
            if(data is not PlayerData playerData)
            {
                Debug.Log($"Unit data is not of type PlayerData");
                return;
            }

            _maxHealth = playerData.MaxHealthPoints;
        }
        #endregion
    }
}
