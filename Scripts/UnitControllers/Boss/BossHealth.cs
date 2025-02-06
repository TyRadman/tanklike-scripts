using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Combat;
    using TankLike.Utils;

    public class BossHealth : EnemyHealth
    {
        public System.Action<int, int> OnTakeDamage;

        [SerializeField] protected float _explosionEffectDelay = 0.3f;

        private BossComponents _bossComponents;
        private ThreeCannonBossAnimations _animations;
        private Coroutine _deathEffectCoroutine;

        public override void SetUp(IController controller)
        {
            BossComponents components = controller as BossComponents;

            if (components == null)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _bossComponents = components;

            base.SetUp(_bossComponents);

            _animations = (ThreeCannonBossAnimations)(_bossComponents).Animations;

        }

        private void SetUpSubscriptions()
        {
            OnTakeDamage += GameManager.Instance.HUDController.BossHUD.UpdateHealthBar;

            OnDeath += GameManager.Instance.ReportManager.ReportBossKill;
            OnDeath += GameManager.Instance.BossesManager.RemoveBoss;
            OnDeath += GameManager.Instance.EnemiesManager.RemoveEnemy;
            OnDeath += _bossComponents => GameManager.Instance.HUDController.BossHUD.HideBossHUD();
            OnDeath += GameManager.Instance.ResultsUIController.DisplayVictoryScreen;
            OnDeath += _bossComponents.OnDeathHandler;

            OnHit += _bossComponents.Visuals.OnHitHandler;
        }

        public override void TakeDamage(DamageInfo damageInfo)
        {
            if (!_canTakeDamage || IsConstrained)
            {
                return;
            }

            base.TakeDamage(damageInfo);

            if (_bossComponents != null)
            {
                _bossComponents.VisualEffects.PlayOnHitEffect();
            }

            OnTakeDamage?.Invoke(_currentHealth, _maxHealth);
        }

        public override void Die()
        {
            _canTakeDamage = false;
            _animations.TriggerDeathAnimation();
            OnDeath?.Invoke(_bossComponents);
        }

        private void RemoveSubscriptions()
        {
            OnTakeDamage = null;
            OnDeath = null;
            OnHit = null;
        }

        public void ExplodeParts()
        {
            // Return to pool if we're using pooling
            Destroy(gameObject);

            _bossComponents.TankBodyParts.HandlePartsExplosion();
            _bossComponents.VisualEffects.PlayDeathEffects();

            // Switch back to the level BG music
            GameManager.Instance.BossesManager.SwitchBackBGMusic();
        }

        public override void Restart()
        {
            if(PlayersManager.PlayersCount == 2)
            {
                _maxHealth = (int)((float)_maxHealth * 1.5f);
            }

            RestoreFullHealth();
            Debug.Log("Max Health " + _maxHealth);
            GameManager.Instance.HUDController.BossHUD.SetupHealthBar();
            SetUpSubscriptions();
        }

        public override void Dispose()
        {
            RemoveSubscriptions();
        }
    }
}
