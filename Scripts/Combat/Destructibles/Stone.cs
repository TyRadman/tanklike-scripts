using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Destructible
{
    using UnitControllers;
    using Cam;

    public class Stone : DestructibleDropper, IBoostDestructible
    {
        [Header("Special Values")]
        [SerializeField] private Renderer _meshRenderer;
        [SerializeField] private float _shatterDuration;

        [Header("References")]
        [SerializeField] private Collider _collider;
        [SerializeField] private Collider _trigger;
        [SerializeField] private ParticleSystem _shatterEffect;

        public void Destruct()
        {
            StartCoroutine(ShatterRoutine());
        }

        protected override void OnDestructibleDeath(UnitComponents tank)
        {
            base.OnDestructibleDeath(tank);

            StartCoroutine(ShatterRoutine());
        }

        private IEnumerator ShatterRoutine()
        {
            _collider.enabled = false;
            _trigger.enabled = false;

            CollectablesDropRequest collectablesDropSettings = new CollectablesDropRequest()
            {
                Position = transform.position,
                DropperTag = DropperTag,
                Settings = _dropSettings,
                Drops = _drops
            };

            GameManager.Instance.CollectableManager.SpawnCollectablesOfType(collectablesDropSettings);

            if (_meshRenderer != null)
            {
                _meshRenderer.gameObject.SetActive(false);
            }

            _shatterEffect.Play();
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);
            yield return new WaitForSeconds(_shatterDuration);

            Destroy(gameObject);
        }
    }
}
