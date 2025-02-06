using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TankLike.ItemsSystem
{
    using TankLike.Misc;
    using UI.Notifications;
    using UnitControllers;
    using Attributes;

    [SelectionBase]
    public abstract class Collectable : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public CollectableType Type { get; private set; }
        public Action<IPoolable> OnReleaseToPool { get; private set; }
        public bool CanBeCollected { get; set; }

        [Header("Drop Settings")]
        [SerializeField] private float _bounceHeight = 1.5f;
        [SerializeField] private AnimationCurve _bounceCurve;
        [SerializeField] private bool _skipCountDown = false;

        [Header("Attraction")]
        [SerializeField] private float _attractionRadius = 3f;
        [SerializeField, InChildren(true)] private CollectableAttractor _attractor;

        [Header("Other Values")]
        [SerializeField] private bool _bounceOnStart = true;

        [Header("Notifications")]
        [SerializeField] protected NotificationBarSettings_SO _notificationSettings;

        [Header("Child Objects")]
        [SerializeField] private List<ParticlesChildDetacher> _childrenToDetach;

        [Header("Pool")]
        [SerializeField] private bool _usePool = true;

        private WaitForSeconds _shrinkingWait;
        private bool _hasDeathCountDown = true;
        private Coroutine _shrinkingRoutine;

        private const float GROUND_LEVEL = 0f;
        private const float SHRINKING_DURATION = 0.2f;

        public void StartCollectable()
        {
            _shrinkingWait = new WaitForSeconds(GameManager.Instance.Constants.Collectables.DisplayDuration);
            _attractor.SetUpAttractor(_attractionRadius, this);
            SpawnCollectable();
            transform.localScale = Vector3.one;
        }

        public virtual void SpawnCollectable()
        {
            if (_bounceOnStart)
            {
                CanBeCollected = false;
                _attractor.EnableAttractor(false);

                GameManager.Instance.CoroutineManager.StartExternalCoroutine(BounceProcess());
            }
            else
            {
                _attractor.EnableAttractor(true);
                CanBeCollected = true;
            }
        }

        public virtual void OnCollected(IPlayerController player)
        {
            GameManager.Instance.CoroutineManager.StopExternalCoroutine(_shrinkingRoutine);

            _attractor.EnableAttractor(false);
            CancelInvoke();
            DisableCollectable();
            PlayParticles();

            if (_usePool)
            {
                OnReleaseToPool(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void PlayParticles()
        {
            var vfx = GetPoofParticles();
            vfx.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            vfx.gameObject.SetActive(true);
            vfx.Play();
        }

        public virtual void DisableCollectable()
        {
            CanBeCollected = false;
            _attractor.EnableAttractor(false);
        }

        private IEnumerator BounceProcess()
        {
            float timer = 0;

            float bounceTime = GameManager.Instance.Constants.Collectables.BounceTime;

            while (timer < bounceTime)
            {
                float offset = _bounceCurve.Evaluate(timer / bounceTime) * _bounceHeight;
                transform.position = new Vector3(transform.position.x, GROUND_LEVEL + offset, transform.position.z);
                timer += Time.deltaTime;
                yield return null;
            }

            transform.position = new Vector3(transform.position.x, GROUND_LEVEL + _bounceCurve.Evaluate(1f) * _bounceHeight, transform.position.z);
            _attractor.EnableAttractor(true);
            CanBeCollected = true;
            OnBounceFinished();
        }

        protected virtual void OnBounceFinished()
        {
            if (_hasDeathCountDown && !_skipCountDown)
            {
                _shrinkingRoutine = GameManager.Instance.CoroutineManager.StartExternalCoroutine(ShrinkingProcess());
            }
        }

        private IEnumerator ShrinkingProcess()
        {
            yield return _shrinkingWait;

            float timeElapsed = 0f;
            Vector3 startScale = transform.localScale;

            while(timeElapsed < SHRINKING_DURATION)
            {
                timeElapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timeElapsed / SHRINKING_DURATION);
                yield return null;
            }

            if (_usePool)
            {
                OnReleaseToPool(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        protected virtual ParticleSystemHandler GetPoofParticles()
        {
            return GameManager.Instance.VisualEffectsManager.Misc.OnCollectedPoof;
        }

        public void ActivateSelfDestructTimer(bool isActive)
        {
            _hasDeathCountDown = isActive;
        }

        #region Pool
        public void Init(Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void TurnOff()
        {

        }

        public void OnRequest()
        {
        }

        public void OnRelease()
        {
            _childrenToDetach.ForEach(c => c.DetachAndReattach());
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
