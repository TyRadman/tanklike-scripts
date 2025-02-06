using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.HUD
{
    using Utils;

    /// <summary>
    /// A bar that has a slant and its fill amount should be slanted (has an angle)
    /// </summary>
    public class SlantedHealthBar : MonoBehaviour, IPausable
    {
        [Header("References")]
        [SerializeField] private RectTransform _fillBarTransform;
        [SerializeField] private Image _fillBarImage;
        [SerializeField] private RectTransform _damageBarTransform;
        [SerializeField] private Image _damageBarImage;
        [SerializeField] private Vector2 _positionRange = new Vector2(-218f, 0f);

        [Header("Health Bar Flash")]
        [SerializeField] private Color _healthBarFlashColor;
        [SerializeField] private float _healthBarFlashSpeed = 1.0f;
        [SerializeField] private Color _hudFlashColor;
        [SerializeField] private Image _hudImage;
        [SerializeField] private Color _healthBarOriginalColor;

        private Coroutine _damageBarShrinkCoroutine;
        private Coroutine _healthBarFlashCoroutine;
        private bool _isFlashing;
        private float _damageBarShrinktimer;

        private const float DAMAGE_SHRINK_DELAY = 1f;
        private const float DAMAGE_SHRINK_SPEED = 200f;

        /// <summary>
        /// Sets the max health position based on the value provided.
        /// </summary>
        /// <param name="maxHealth">A float value from 0 to 1.</param>
        public void SetMaxHealth(float maxHealth, bool fillHealthBar = false)
        {
            float xPosition = _positionRange.Lerp(maxHealth);

            Vector2 newPosition = new Vector2(xPosition, _fillBarTransform.anchoredPosition.y);

            _fillBarTransform.anchoredPosition = newPosition;
            _damageBarTransform.anchoredPosition = newPosition;

            if (fillHealthBar)
            {
                SetValue(1f, 1f, 1f);
            }
        }

        /// <summary>
        /// Set the bar value.
        /// </summary>
        /// <param name="value">A float value from 0 to 1.</param>
        public void SetValue(float lastValue, float currentValue, float maxValue)
        {
            if (_fillBarImage == null)
            {
                return;
            }

            float value = currentValue / maxValue;
            float xPosition = _positionRange.Lerp(value);

            _fillBarTransform.anchoredPosition = new Vector2(xPosition, _fillBarTransform.anchoredPosition.y);

            if (lastValue <= currentValue || currentValue == 0f)
            {
                return;
            }

            if (_damageBarTransform.anchoredPosition.x > _fillBarTransform.anchoredPosition.x)
            {
                this.StopCoroutineSafe(_damageBarShrinkCoroutine);

                _damageBarShrinktimer = 0f;
                _damageBarShrinkCoroutine = StartCoroutine(DamageBarShrinkRoutine());
            }
            else
            {
                _damageBarTransform.anchoredPosition = _fillBarTransform.anchoredPosition;
            }
        }

        public void ResetDamageBar()
        {
            this.StopCoroutineSafe(_damageBarShrinkCoroutine);

            _damageBarTransform.anchoredPosition = _fillBarTransform.anchoredPosition;
            _damageBarShrinktimer = 0f;
        }

        public void SetFixedValue(float currentValue, float maxValue)
        {
            if (_fillBarImage == null)
            {
                return;
            }

            float value = currentValue / maxValue;
            float xPosition = _positionRange.Lerp(value);

            _fillBarTransform.anchoredPosition = new Vector2(xPosition, _fillBarTransform.anchoredPosition.y);
            _damageBarTransform.anchoredPosition = _fillBarTransform.anchoredPosition;
        }

        public void ResetValue()
        {
            float xPosition = _positionRange.Lerp(0);
            Vector2 newPosition = new Vector2(xPosition, _fillBarTransform.anchoredPosition.y);

            _fillBarTransform.anchoredPosition = newPosition;
            _damageBarTransform.anchoredPosition = newPosition;

            _fillBarImage.color = _healthBarOriginalColor;
        }

        /// <summary>
        /// Flashes the health bar
        /// </summary>
        public void StartHealthBarFlash()
        {
            _isFlashing = true;

            if (_healthBarFlashCoroutine != null)
            {
                StopCoroutine(_healthBarFlashCoroutine);
            }

            _healthBarFlashCoroutine = StartCoroutine(HealthBarFlashRoutine());
        }

        public void StopHealthBarFlash()
        {
            if (_healthBarFlashCoroutine != null)
            {
                StopCoroutine(_healthBarFlashCoroutine);
            }

            _isFlashing = false;
            _fillBarImage.color = _healthBarOriginalColor;
            _hudImage.color = _healthBarOriginalColor;
        }

        private IEnumerator DamageBarShrinkRoutine()
        {
            while (_damageBarShrinktimer < DAMAGE_SHRINK_DELAY)
            {
                _damageBarShrinktimer += Time.deltaTime;
                yield return null;
            }
            
            while (_damageBarTransform.anchoredPosition.x > _fillBarTransform.anchoredPosition.x)
            {
                _damageBarTransform.anchoredPosition -= Vector2.right * DAMAGE_SHRINK_SPEED * Time.deltaTime;
                yield return null;
            }

            _damageBarTransform.anchoredPosition = _fillBarTransform.anchoredPosition;
            _damageBarShrinktimer = 0f;
        }

        private IEnumerator HealthBarFlashRoutine()
        {
            float t = 0f;

            while (_isFlashing)
            {
                // Lerp between the two colors over time
                t = Mathf.PingPong(Time.time * _healthBarFlashSpeed, 1f);
                _fillBarImage.color = Color.Lerp(_healthBarOriginalColor, _healthBarFlashColor, t);
                _hudImage.color = Color.Lerp(_healthBarOriginalColor, _hudFlashColor, t);

                yield return null;
            }
        }

        #region IPausable
        public void OnPaused()
        {
            // Stop the health bar flashing
            if (_isFlashing)
            {
                this.StopCoroutineSafe(_healthBarFlashCoroutine);
            }

            // Check if the shrink timer stil ticking, stop the coroutine
            if (_damageBarShrinktimer > 0f)
            {
                this.StopCoroutineSafe(_damageBarShrinkCoroutine);
            }
        }

        public void OnResumed()
        {
            // Continue the health bar flashing
            if (_isFlashing)
            {
                _healthBarFlashCoroutine = StartCoroutine(HealthBarFlashRoutine());
            }

            // Check if the shrink timer stil ticking, if so continue the coroutine by restarting it
            if (_damageBarShrinktimer > 0f)
            {
                _damageBarShrinkCoroutine = StartCoroutine(DamageBarShrinkRoutine());
            }
        }
        #endregion
    }
}
