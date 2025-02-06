using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class HealthBar : MonoBehaviour
    {
        [Header("Health Bar")]
        [SerializeField] private Image _healthBarFill;

        [Header("Damage Bar")]
        [SerializeField] private Image _damageBarImage;
        [SerializeField] private float _damageBarShrinkDelay = 1f;
        [SerializeField] private float _damageBarShrinkSpeed = 1f;

        private Coroutine _damageBarShrinkCoroutine;

        public void SetupHealthBar()
        {
            if (_healthBarFill == null) return;

            _healthBarFill.fillAmount = 1f;
            _damageBarImage.fillAmount = 1f;
        }

        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (_healthBarFill == null)
            {
                return;
            }

            _healthBarFill.fillAmount = (float)currentHealth / (float)maxHealth;

            if (_damageBarImage.fillAmount > _healthBarFill.fillAmount)
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

            while (_damageBarImage.fillAmount > _healthBarFill.fillAmount)
            {
                _damageBarImage.fillAmount -= _damageBarShrinkSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }
}
