using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_ThreeCannon_Death", menuName = MENU_PATH + "Three Cannon/Death State")]
    public class ThreeCannonBossDeathState : BossDeathState
    {
        private float _timer;
        private float _explosionTimer;

        private bool _explodeParts;

        public override void OnEnter()
        {
            base.OnEnter();

            _isActive = true;

            _timer = 0f;
            _explosionTimer = _deathExplosionInterval;
            _explodeParts = true;
        }

        public override void OnUpdate()
        {
            if(_timer < _deathStateDuration)
            {
                if(_explosionTimer >= _deathExplosionInterval)
                {
                    SpawnExplosion();
                    _explosionTimer = 0f;
                }

                _explosionTimer += Time.deltaTime;
                _timer += Time.deltaTime;
            }
            else
            {
                if (_explodeParts)
                {
                    _health.ExplodeParts();
                    _explodeParts = false;
                }
            }
        }

        private void SpawnExplosion()
        {
            var rand = Random.insideUnitSphere * _deathExplosionRadius;
            var explosion = GameManager.Instance.VisualEffectsManager.Explosions.DeathExplosion;
            explosion.transform.position = _health.transform.position + Vector3.up * _explosionCenterHeight + rand;
            explosion.gameObject.SetActive(true);
            explosion.Play();
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
        }
    }
}
