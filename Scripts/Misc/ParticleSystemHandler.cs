using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Misc
{
    public class ParticleSystemHandler : MonoBehaviour, IPoolable
    {
        [SerializeField] private float _lifetime;
        [SerializeField] private float _simulationSpeed = 1f;
        public Action<IPoolable> OnReleaseToPool { get; private set; }

        private Coroutine _turnOffCoroutine;
        private WaitForSeconds _turnOffWaitForseconds;
        private ParticleSystem _particles;
        private ParticleSystem[] _childParticles;
        private Vector3 _originalScale;
        public ParticleSystem Particles => _particles;

        private void Awake()
        {
            _particles = GetComponent<ParticleSystem>();
           
            if (_particles != null)
            {
                _childParticles = GetChildParticleSystems(_particles);
            }

            if(_particles == null)
            {
                return;
            }

            _particles.Stop();
            _turnOffWaitForseconds = new WaitForSeconds(_lifetime);
            _originalScale = transform.localScale;
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
            GameManager.Instance.SetParentToSpawnables(gameObject);
            transform.localScale = _originalScale;
            Stop();
        }

        private IEnumerator TurnOffRoutine()
        {
            yield return _turnOffWaitForseconds;
            OnReleaseToPool(this);
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }

        public void Play(float speed = 1f)
        {
            var mainModule = _particles.main;
            mainModule.simulationSpeed = speed != 1? speed : _simulationSpeed;

            if(_childParticles.Length > 0)
            {
                foreach (ParticleSystem childParticleSystem in _childParticles)
                {
                    var childMainModule = childParticleSystem.main;
                    childMainModule.simulationSpeed = speed;
                }
            }

            _particles.Play();

            if (_lifetime != 0f)
            {
                if (_turnOffCoroutine != null)
                    StopCoroutine(_turnOffCoroutine);
                _turnOffCoroutine = StartCoroutine(TurnOffRoutine());
            }
        }

        private ParticleSystem[] GetChildParticleSystems(ParticleSystem parent)
        {
            // Get all child Particle Systems (sub-emitters) of the parent.
            ParticleSystem[] childSystems = new ParticleSystem[parent.transform.childCount];

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                childSystems[i] = parent.transform.GetChild(i).GetComponent<ParticleSystem>();
            }

            return childSystems;
        }

        public void Stop()
        {
            _particles.Stop();

            if (_turnOffCoroutine != null)
                StopCoroutine(_turnOffCoroutine);
        }

        public void Clear()
        {
            if (_turnOffCoroutine != null)
                StopCoroutine(_turnOffCoroutine);

            Destroy(gameObject);
        }

        public void Init(Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void OnRequest()
        {

        }
    }
}
