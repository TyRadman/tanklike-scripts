using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using System;
    using Utils;

    public class FireTrailSpot : MonoBehaviour, IPoolable
    {
        [Header("References")]
        [SerializeField] private List<ParticleSystem> _particles;
        [SerializeField] private ParticleSystem _charParticles;
        [SerializeField] private SphereCollider _collider;

        public Action<IPoolable> OnReleaseToPool { get; private set; }

        private int _damageToDeal = 10;
        private LayerMask _targetLayerMask;

        private const float PARTICLES_DEATH_DELAY = 1f;
        private const float CHAR_EXTRA_DURATION = 1.5f;

        /// <summary>
        /// Set up the values of the fire spot.
        /// </summary>
        /// <param name="damage">Damage to deal.</param>
        /// <param name="target">The tag of the target for collisions.</param>
        /// <param name="duration">How long will the fire particles play.</param>
        public void SetUp(int damage, string target, float duration)
        {
            _collider.enabled = true;

            _damageToDeal = damage;

            if (target == TanksTag.Enemy.ToString())
            {
                _targetLayerMask |= 1 << Constants.EnemyDamagableLayer;
            }
            else
            {
                _targetLayerMask |= 1 << Constants.PlayerDamagableLayer;
            }

            SetUpParticles(duration);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damagable))
            {
                if ((1 << other.gameObject.layer & _targetLayerMask) == 1 << other.gameObject.layer)
                {
                    DealDamageToDamagable(damagable);
                }
            }
        }

        private void DealDamageToDamagable(IDamageable target)
        {
            DamageInfo damageInfo = DamageInfo.Create()
                .SetDamage(_damageToDeal)
                .Build();

            target.TakeDamage(damageInfo);
        }

        private void SetUpParticles(float duration)
        {
            Invoke(nameof(StopParticles), duration);

            _particles.ForEach(p => p.Play());

            // set the lifetime of the char particles because they're not on loop
            ParticleSystem.MainModule charMain = _charParticles.main;
            charMain.startLifetime = duration + CHAR_EXTRA_DURATION;
            _charParticles.Play();
        }

        private void StopParticles()
        {
            _particles.ForEach(p => p.Stop());
            _collider.enabled = false;

            Invoke(nameof(TurnOff), PARTICLES_DEATH_DELAY + CHAR_EXTRA_DURATION);
        }

        public float GetDamage()
        {
            return _damageToDeal;
        }

        #region Pool
        public void Init(Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }

        public void OnRequest()
        {
            GameManager.Instance.SetParentToRoomSpawnables(gameObject);
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
