using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    public class HealField : MonoBehaviour, IPoolable
    {
        [SerializeField] float _fieldRaduis = 3f;
        [SerializeField] private int _healAmount = 1;
        [SerializeField] private float _healingRatePerSecond = 3f;
        [SerializeField] private LayerMask _targetsMask;
        [SerializeField] private ParticleSystem _particles;

        private float _duration;

        public Action<IPoolable> OnReleaseToPool { get; private set; }

        private void Start()
        {
        }

        public void Activate(bool value, float duration)
        {
            _duration = duration;
            StartCoroutine(HealCoroutine());
        }

        private IEnumerator HealCoroutine()
        {
            float healingTimer = 0f;
            float fieldTimer = 0f;

            _particles.Play();

            while (fieldTimer < _duration)
            {     
                healingTimer += Time.deltaTime;
                fieldTimer += Time.deltaTime;
              
                if (healingTimer >= _healingRatePerSecond)
                {
                    Heal();
                    healingTimer = 0;
                }

                yield return null;
            }

            _particles.Stop(true);

            yield return new WaitForSeconds(_particles.main.duration);

            OnReleaseToPool(this);
        }

        private void Heal()
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, _fieldRaduis, _targetsMask);

            for (int i = 0; i < targets.Length; i++)
            {
                TankHealth target = targets[i].GetComponent<TankHealth>();
                target.Heal(_healAmount);

                // visuals
                var vfx = GameManager.Instance.VisualEffectsManager.Buffs.HealOnce;
                vfx.transform.SetPositionAndRotation(target.transform.position + Vector3.up * 1.2f, Quaternion.identity);
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _fieldRaduis);
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
