using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.HUD
{
    using Utils;

    /// <summary>
    /// A bar that can change its max value based on the values provided.
    /// </summary>
    public class ResizableBar : MonoBehaviour, IPausable
    {
        [SerializeField] private bool _resizableBar;
        [SerializeField] private RectTransform _barTransform;
        [SerializeField] private Vector2 _positionRange = new Vector2(-286f, 0f);
        [SerializeField] private Image _barImage;
        [SerializeField] private Image _damageBarImage;

        [Header("Health Bar Flash")]
        [SerializeField] private Color _healthBarFlashColor;
        [SerializeField] private float _healthBarFlashSpeed = 1.0f;
        [SerializeField] private Color _hudFlashColor;
        [SerializeField] private Image _hudImage;
        [SerializeField] private Color _healthBarOriginalColor;

        private float _minBarFillAmount;
        private Coroutine _damageBarShrinkCoroutine;
        private Coroutine _healthBarFlashCoroutine;
        private bool _isFlashing;
        private float _damageBarShrinktimer;

        private const float DAMAGE_SHRINK_DELAY = 1f;
        private const float DAMAGE_SHRINK_SPEED = 1f;

        /// <summary>
        /// Sets the max size based on the value provided.
        /// </summary>
        /// <param name="size">A float value from 0 to 1.</param>
        public void SetMaxSize(float size, bool fillHealthBar = false)
        {
            if (_resizableBar)
            {
                float xPosition = _positionRange.Lerp(size);
                Vector2 newPosition = new Vector2(xPosition, _barTransform.anchoredPosition.y);
                _barTransform.anchoredPosition = newPosition;
                _damageBarImage.GetComponent<RectTransform>().anchoredPosition = newPosition;

                _minBarFillAmount = 1f - size;
            }
            else
            {
                _minBarFillAmount = 0f;
            }


            if (fillHealthBar)
            {
                SetValue(1f, 1f);
            }

            // TODO: create a SetUp function and add this to it
            _damageBarImage.fillAmount = 1f; // TODO: find the right values later when we have the increase max health logic
        }

        /// <summary>
        /// Set the bar value.
        /// </summary>
        /// <param name="value">A float value from 0 to 1.</param>
        public void SetValue(float currentValue, float maxValue)
        {
            if (_barImage == null)
            {
                return;
            }

            float value = currentValue / maxValue;

            _barImage.fillAmount = Mathf.Lerp(_minBarFillAmount, 1f, value);

            if (_damageBarImage.fillAmount > _barImage.fillAmount)
            {
                if (_damageBarShrinkCoroutine != null)
                {
                    StopCoroutine(_damageBarShrinkCoroutine);
                }

                _damageBarShrinktimer = 0f;
                _damageBarShrinkCoroutine = StartCoroutine(DamageBarShrinkRoutine());
            }
            else
            {
                _damageBarImage.fillAmount = _barImage.fillAmount;
            }
        }

        public void ResetValue()
        {
            _barImage.fillAmount = Mathf.Lerp(_minBarFillAmount, 1f, 0);
            _damageBarImage.fillAmount = Mathf.Lerp(_minBarFillAmount, 1f, 0);
            _barImage.color =_healthBarOriginalColor;
        }

        /// <summary>
        /// Flashes the health bar
        /// </summary>
        public void StartHealthBarFlash()
        {
            _isFlashing = true;

            if(_healthBarFlashCoroutine != null)
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
            _barImage.color = _healthBarOriginalColor;
            _hudImage.color = _healthBarOriginalColor;
        }

        private IEnumerator DamageBarShrinkRoutine()
        {
            while (_damageBarShrinktimer < DAMAGE_SHRINK_DELAY)
            {
                _damageBarShrinktimer += Time.deltaTime;
                yield return null;
            }

            while (_damageBarImage.fillAmount > _barImage.fillAmount)
            {
                _damageBarImage.fillAmount -= DAMAGE_SHRINK_SPEED * Time.deltaTime;
                yield return null;
            }

            _damageBarShrinktimer = 0f;
        }

        private IEnumerator HealthBarFlashRoutine()
        {
            float t = 0f;

            while (_isFlashing)
            {
                // Lerp between the two colors over time
                t = Mathf.PingPong(Time.time * _healthBarFlashSpeed, 1f);
                _barImage.color = Color.Lerp(_healthBarOriginalColor, _healthBarFlashColor, t);
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
            if(_damageBarShrinktimer > 0f)
            {
                _damageBarShrinkCoroutine = StartCoroutine(DamageBarShrinkRoutine());
            }
        }
        #endregion
    }
}
