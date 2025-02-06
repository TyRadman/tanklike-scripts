using UnityEngine;

namespace TankLike.UnitControllers
{
    using Combat;
    using TankLike.UI;
    using TankLike.Utils;

    public class EnemyHealth : TankHealth
    {
        [Header("Health Bar")]
        [SerializeField] private bool _enableHealthBar;
        [SerializeField] private GameObject _healthBarCanvas;
        [SerializeField] private HealthBar _healthBar;

        public int PlayerIndex { get; protected set; } = -1;

        protected EnemyComponents _enemyComponents;

        public override void SetUp(IController controller)
        {
            EnemyComponents components = controller as EnemyComponents;

            // TODO: dirty, clean it
            if (controller is null or not EnemyComponents and not BossComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _enemyComponents = components;

            // TODO: Dirty and I hate it :'). Recommended solution: base.SetUp(controller);
            if (controller is not BossComponents)
            {
                base.SetUp(_enemyComponents);
            }
            else
            {
                BossComponents bossComponents = (BossComponents)controller;
                base.SetUp(bossComponents);
            }
        }

        public override void TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead)
            {
                return;
            }

            UnitComponents instigator = damageInfo.Instigator;

            if (instigator is PlayerComponents playerComponents)
            {
                PlayerIndex = playerComponents.PlayerIndex;
            }

            base.TakeDamage(damageInfo);

            // Checking for the boss
            if (_enemyComponents != null)
            {
                _enemyComponents.VisualEffects.PlayOnHitEffect();
            }

            if (instigator != null)
            {
                instigator.GetShooter().OnTargetHit?.Invoke();
            }

            if (_currentHealth > 0)
            {
                UpdateHealthBar();
            }
        }

        protected void UpdateHealthBar()
        {
            if (_healthBar != null)
            {
                _healthBar.UpdateHealthBar(_currentHealth, _maxHealth);
            }
        }

        public override void Die()
        {
            base.Die();

            ReportDeath();

            _enemyComponents.ItemDrop.DropItem();
        }

        /// <summary>
        /// Reports the death of the enemy to the enemies manager and the report manager.
        /// </summary>
        protected void ReportDeath()
        {
            EnemyData enemyStats = (EnemyData)_stats;

            EntityEliminationReport report = new EntityEliminationReport()
            {
                Target = _enemyComponents,
                PlayerIndex = PlayerIndex,
                Position = PopUpAnchor.Anchor
            };

            GameManager.Instance.ReportManager.ReportEnemyElimination(report);
            GameManager.Instance.EnemiesManager.RemoveEnemy(_enemyComponents);
        }

        public override void RestoreFullHealth()
        {
            base.RestoreFullHealth();

            UpdateHealthBar();
        }

        #region IController
        public override void Activate()
        {
            base.Activate();

            if (_healthBarCanvas != null)
            {
                if (_enableHealthBar)
                {
                    _healthBarCanvas.SetActive(true);
                    _healthBar.SetupHealthBar();
                }
                else
                {
                    _healthBarCanvas.SetActive(false);
                }
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Dispose()
        {
            base.Dispose();

            _currentHealth = 0;
            UpdateHealthBar();

            OnDeath -= _enemyComponents.OnDeathHandler;
        }

        public override void Restart()
        {
            base.Restart();

            RestoreFullHealth();
            
            OnDeath += _enemyComponents.OnDeathHandler;
        }
        #endregion
    }
}
