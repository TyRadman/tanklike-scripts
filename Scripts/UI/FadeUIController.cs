using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class FadeUIController : MonoBehaviour, IManager
    {
        [SerializeField] private Image _fadeOutImage;
        [field: SerializeField, Range(0.1f, 5f)] public float FadeOutDuration { get; private set; } = 1f;
        [field: SerializeField, Range(0.1f, 5f)] public float FadeInDuration { get; private set; } = 1f;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Color _fadeOutStartColor;
        [SerializeField] private Animator _animator;

        private readonly int _idleInHash = Animator.StringToHash("Idle");
        private readonly int _fadeInHash = Animator.StringToHash("FadeIn");
        private readonly int _fadeOutHash = Animator.StringToHash("FadeOut");

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _animator.speed = 1;
            _fadeOutImage.color = _fadeOutStartColor;
            _canvasGroup.alpha = 0f;
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        [ContextMenu("FadeOut")]
        public void StartFadeOut()
        {
            //Debug.Log("Fade out");

            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            PlayAnimation(_fadeOutHash, FadeOutDuration);
        }

        [ContextMenu("FadeIn")]
        public void StartFadeIn()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            PlayAnimation(_fadeInHash, FadeInDuration);
        }

        private void PlayAnimation(int hash, float duration = 1f)
        {
            _animator.speed = 1 / duration;
            _animator.Play(hash, -1, 0f);
        }

        public void ResetFadeController()
        {
            _canvasGroup.alpha = 0f;
            _animator.speed = 1;
        }
    }
}
