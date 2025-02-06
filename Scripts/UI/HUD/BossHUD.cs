using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.HUD
{
    public class BossHUD : MonoBehaviour, IManager
    {
        [SerializeField] private GameObject _parent;
        [Header("Health")]
        [SerializeField] private Image _healthFillImage;

        [Header("Damage Bar")]
        [SerializeField] private Image _damageBarImage;
        [SerializeField] private float _damageBarShrinkDelay = 1f;
        [SerializeField] private float _damageBarShrinkSpeed = 1f;

        [Header("Animation")]
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _idleAnimation;
        [SerializeField] private AnimationClip _showAnimation;
        [SerializeField] private AnimationClip _hideAnimation;
        
        private Coroutine _damageBarShrinkCoroutine;
        private float _damageBarShrinkTimer;

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _animation.clip = _idleAnimation;
            _animation.Play();
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        public void DisplayBossHUD()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _animation.clip = _showAnimation;
            _animation.Play();
        }

        public void HideBossHUD()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _animation.clip = _hideAnimation;
            _animation.Play();
        }

        public void SetupHealthBar()
        {
            if (_healthFillImage == null) return;

            _healthFillImage.fillAmount = 1f;
            _damageBarImage.fillAmount = 1f;
        }

        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (_healthFillImage == null)
            {
                return;
            }

            _healthFillImage.fillAmount = (float)currentHealth / (float)maxHealth;

            if(_damageBarImage.fillAmount > _healthFillImage.fillAmount)
            {
                if (_damageBarShrinkCoroutine != null)
                {
                    StopCoroutine(_damageBarShrinkCoroutine);
                }

                _damageBarShrinkCoroutine = StartCoroutine(DamageBarShrinkRoutine());
            }
        }

        private IEnumerator DamageBarShrinkRoutine()
        {
            float timer = 0f;

            while (timer < _damageBarShrinkDelay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            while(_damageBarImage.fillAmount > _healthFillImage.fillAmount)
            {
                _damageBarImage.fillAmount -= _damageBarShrinkSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }
}
