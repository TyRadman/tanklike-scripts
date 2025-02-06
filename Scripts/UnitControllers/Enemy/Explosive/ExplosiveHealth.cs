using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Cam;
    using TankLike.Combat;
    using TankLike.UI.DamagePopUp;

    public class ExplosiveHealth : EnemyHealth
    {
        [Header("Explosive Settings")]
        [SerializeField] private Vector2 _explosiveImpactExplosionTimeRange;

        private bool _wasKilled;
        private bool _explosionTriggered;


        public override void TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead)
            {
                return;
            }

            UnitComponents instigator = damageInfo.Instigator;
            int damage = damageInfo.Damage;

            if (instigator != null && instigator == _enemyComponents)
            {
                return;
            }

            if (!_canTakeDamage || IsConstrained)
            {
                return;
            }

            if (damageInfo.DamageType == ImpactType.Explosive)
            {
                if(_currentHealth <= 0)
                {
                    return;
                }

                _currentHealth = 0;
                Explode(true);
                return;
            }

            if (_explosionTriggered)
            {
                _canTakeDamage = false;
                ((ExplosiveShooter)_enemyComponents.Shooter).ForceExplosion();
                return;
            }

            OnHit?.Invoke();

            // if it's damage that we're dealing with, not health increment, then we apply the multiplier to it
            if (damage > 0)
            {
                damage += Mathf.RoundToInt(damage * _damageMultiplier);
            }

            _currentHealth -= damage;
            _damageEffects.SetIntensity((float)_currentHealth / (float)_maxHealth);

            ///// display pop up /////
            // if the damage is positive, then it's a heal
            if (PopUpAnchor != null)
            {
                if (damage > 0)
                {
                    GameManager.Instance.DamagePopUpManager.DisplayPopUp(
                        DamagePopUpType.Damage, damage, PopUpAnchor.Anchor);
                }
                else
                {
                    GameManager.Instance.DamagePopUpManager.DisplayPopUp(
                        DamagePopUpType.Heal, damage, PopUpAnchor.Anchor);
                }
            }

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _explosionTriggered = true;
                Explode();
            }

            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }

            if (instigator is PlayerComponents playerComponents)
            {
                PlayerIndex = playerComponents.PlayerIndex;
            }

            if (instigator != null)
            {
                instigator.GetShooter().OnTargetHit?.Invoke();
            }

            _enemyComponents.VisualEffects.PlayOnHitEffect();

            if (_currentHealth > 0)
            {
                UpdateHealthBar();
            }
        }

        public void SetExplosionTriggered()
        {
            _explosionTriggered = true;
        }

        public override void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;

            _enemyComponents.ItemDrop.DropItem();
            ReportDeath();

            //_enemyComponents.TankBodyParts.HandlePartsExplosion();
            GameManager.Instance.EnemiesManager.RemoveEnemy(_enemyComponents);
            GameManager.Instance.ScreenFreezer.FreezeScreen(DeathScreenFreeze);
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.EXPLOSION);
            OnDeath?.Invoke(_enemyComponents);
        }

        public void Explode(bool explosiveImpact = false)
        {
            (_enemyComponents.Movement as EnemyMovement).OnPathEndReached(false);

            if (!explosiveImpact)
            {
                (_enemyComponents.Shooter as ExplosiveShooter).ForceExplosionTrigger();
            }
            else
            {
                float timer = Random.Range(_explosiveImpactExplosionTimeRange.x, _explosiveImpactExplosionTimeRange.y);
                Invoke(nameof(ExplosiveImpactExplosion), timer);
            }
        }

        private void ExplosiveImpactExplosion()
        {
            (_enemyComponents.Shooter as ExplosiveShooter).Explode();
            Die();
        }

        #region IController
        public override void Restart()
        {
            base.Restart();

            PlayerIndex = -1;
            _explosionTriggered = false;
            _canTakeDamage = true;
        }
        #endregion
    }
}
