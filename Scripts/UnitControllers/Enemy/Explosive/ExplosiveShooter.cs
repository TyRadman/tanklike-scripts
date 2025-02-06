using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Combat;

    public class ExplosiveShooter : EnemyShooter
    {
        public System.Action OnExplosionTriggered;

        [Header("Explosion Settings")]
        [SerializeField] private CollisionEventPublisher _explosionTrigger;
        [SerializeField] private IndicatorEffects.IndicatorType _indicatorType;
        [SerializeField] private PoolableParticlesReference _explosionEffect;

        private Indicator _indicator;
        private bool _explosionTimerStarted;

        public void Explode()
        {
            _currentWeapon.OnShot();
            PlayExplosionEffects();
        }

        public void SetRadius(float radius)
        {
            ((AOEWeapon)_currentWeapon).SetExplosionRadius(radius);
        }

        public void StartExplosionTimer()
        {
            if (_explosionTimerStarted)
            {
                return;
            }

            if (_telegraphCoroutine != null)
            {
                StopCoroutine(_telegraphCoroutine);
            }

            _telegraphCoroutine = StartCoroutine(ExplosionTimerRoutine());
        }

        protected IEnumerator ExplosionTimerRoutine()
        {
            _explosionTimerStarted = true;

            // Inform the Health component about the explosion trigger so the next time it takes damage it explodes
            ((ExplosiveHealth)_enemyComponents.Health).SetExplosionTriggered();

            // Disable explosion trigger collider
            _explosionTrigger.EnableCollider(false);

            // Start flashing the enemy meshes
            _enemyComponents.Visuals.StartFlashing();

            AOEWeapon weapon = (AOEWeapon)_currentWeapon;

            if (_indicatorType == IndicatorEffects.IndicatorType.None)
            {
                Debug.LogError("Indicator type is not set!");
                yield break;
            }
            
            Vector3 indicatorSize = new Vector3(weapon.ExplosionRadius * 2, 0f, weapon.ExplosionRadius * 2);
            _indicator = GameManager.Instance.VisualEffectsManager.Indicators.GetIndicatorByType(_indicatorType);
            _indicator.gameObject.SetActive(true);
            var pos = transform.position;
            pos.y = Constants.GroundHeight;
            _indicator.transform.position = pos;
            _indicator.transform.rotation = Quaternion.identity;
            _indicator.transform.localScale = indicatorSize;
            _indicator.transform.parent = transform;
            _indicator.Play();
            _activePoolables.Add(_indicator);
            
            yield return new WaitForSeconds(_telegraphDuration);

            // Explode
            Explode();

            _indicator.TurnOff();
            //_indicator.transform.parent = null;
            _activePoolables.Remove(_indicator);
            _enemyComponents.Visuals.StopFlashing();

            _enemyComponents.Health.Die();
        }

        public void PlayExplosionEffects()
        {
            AOEWeapon weapon = (AOEWeapon)_currentWeapon;

            var explosion = GameManager.Instance.VisualEffectsManager.Explosions.GetExplosion(_explosionEffect);
            Vector3 pos = transform.position;
            pos.y = Constants.GroundHeight;
            explosion.transform.SetPositionAndRotation(pos, Quaternion.identity);
            explosion.transform.localScale = new Vector3(weapon.ExplosionRadius, weapon.ExplosionRadius, weapon.ExplosionRadius);
            explosion.gameObject.SetActive(true);
            explosion.Play();

            var decal = GameManager.Instance.VisualEffectsManager.Explosions.ExplosionDecal;
            float randomAngle = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
            decal.transform.SetPositionAndRotation(pos, randomRotation);
            decal.transform.localScale = Vector3.one;
            decal.gameObject.SetActive(true);
            decal.Play();
        }

        private void OnExplosionTriggerEnterHandler(Collider target)
        {
            if(!IsActive)
            {
                return;
            }

            if (target.gameObject.CompareTag("Player"))
            {
                OnExplosionTriggered?.Invoke();
            }
        }

        public void ForceExplosionTrigger()
        {
            if (!IsActive)
            {
                return;
            }

            OnExplosionTriggered?.Invoke();
        }

        public void ForceExplosion()
        {
            if (_telegraphCoroutine != null)
            {
                StopCoroutine(_telegraphCoroutine);
            }

            Explode();
            
            if(_indicator != null)
            {
                _indicator.TurnOff();
                _activePoolables.Remove(_indicator);
            }

            _enemyComponents.Visuals.StopFlashing();
            _enemyComponents.Health.Die();
        }


        #region IController
        public override void Restart()
        {
            base.Restart();

            _explosionTimerStarted = false;

            _explosionTrigger.EnableCollider(true);
            _explosionTrigger.OnTriggerEnterEvent += OnExplosionTriggerEnterHandler;
        }

        public override void Dispose()
        {
            base.Dispose();

            _explosionTrigger.EnableCollider(false);
            _explosionTrigger.OnTriggerEnterEvent -= OnExplosionTriggerEnterHandler;
        }
        #endregion
    }
}
