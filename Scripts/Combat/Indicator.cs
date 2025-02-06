using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    public class Indicator : MonoBehaviour, IPoolable
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private bool _flash;
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _startAnimationClip;
        [SerializeField] private AnimationClip _finishAnimationClip;
        [SerializeField] private GameObject _graphics;
        public Action<IPoolable> OnReleaseToPool { get; private set; }

        private void Awake()
        {
            if (_graphics != null)
            {
                _graphics.SetActive(false);
            }
        }

        public virtual void Play(float duration = 0f)
        {
            if(_flash)
                _animator.Play("Flash", 0, 0f);
        }

        public virtual void OnIndicatorStarted()
        {
            CancelInvoke();
            PlayAnimation(_startAnimationClip);
        }

        public virtual void OnIndicatorFinished()
        {
            PlayAnimation(_finishAnimationClip);
            Invoke(nameof(DisableIndicator), _finishAnimationClip.length);
        }

        private void DisableIndicator()
        {
            gameObject.SetActive(false);
        }

        private void PlayAnimation(AnimationClip clip)
        {
            if (_animation.isPlaying)
            {
                _animation.Stop();
            }

            _animation.clip = clip;
            _animation.Play();
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
            GameManager.Instance.SetParentToSpawnables(gameObject);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion

    }
}
