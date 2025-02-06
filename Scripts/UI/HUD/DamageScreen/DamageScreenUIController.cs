using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class DamageScreenUIController : MonoBehaviour, IManager
    {
        [Header("Settings")]
        [SerializeField] private float _damageScreenAlpha;
        [SerializeField] private float _damageScreenFadeSpeed;
        [SerializeField] private float _lowHealthScreenAlpha = 0.2f;

        [Header("References")]
        [SerializeField] private Image _singlePlayerDamageScreen;
        [SerializeField] private Image[] _multiplayerDamageScreens;

        private bool _isMultiplayer;
        private Coroutine[] _damageScreenCoroutines = new Coroutine[2];
        private Coroutine[] _lowHealthOverlayCoroutines = new Coroutine[2];
        private bool[] _lowHealthOverlay = new bool[2];

        public bool IsActive { get; private set; }


        #region IManager
        public void SetUp()
        {
            IsActive = true;

            // Check if is multiplayer mode
            if (PlayersManager.PlayersCount == 1)
            {
                _isMultiplayer = false;
                _singlePlayerDamageScreen.gameObject.SetActive(true);
                _multiplayerDamageScreens[0].gameObject.SetActive(false);
                _multiplayerDamageScreens[1].gameObject.SetActive(false);
            }
            else if(PlayersManager.PlayersCount == 2)
            {
                _isMultiplayer = true;
                _singlePlayerDamageScreen.gameObject.SetActive(false);
                _multiplayerDamageScreens[0].gameObject.SetActive(true);
                _multiplayerDamageScreens[1].gameObject.SetActive(true);
            }

            SetStartColor(_singlePlayerDamageScreen);
            SetStartColor(_multiplayerDamageScreens[0]);
            SetStartColor(_multiplayerDamageScreens[1]);
        }

        public void Dispose()
        {
            IsActive = false;

            StopAllCoroutines();
        }
        #endregion

        private void SetStartColor(Image damageScreen)
        {
            Color startColor = damageScreen.color;
            startColor.a = 0f;
            damageScreen.color = startColor;
        }

        public void ShowDamageScreen(int playerIndex)
        {
            _lowHealthOverlay[playerIndex] = false;

            if (_damageScreenCoroutines[playerIndex] != null)
            {
                StopCoroutine(_damageScreenCoroutines[playerIndex]);
            }

            _damageScreenCoroutines[playerIndex] = StartCoroutine(DamageScreenRoutine(playerIndex, false));
        }

        public void ShowLowHealthOverlay(int playerIndex)
        {
            _lowHealthOverlay[playerIndex] = true;

            if (_damageScreenCoroutines[playerIndex] != null)
            {
                StopCoroutine(_damageScreenCoroutines[playerIndex]);
            }

            _damageScreenCoroutines[playerIndex] = StartCoroutine(DamageScreenRoutine(playerIndex, true));
        }

        private IEnumerator DamageScreenRoutine(int playerIndex, bool lowHealth)
        {
            Image damageScreenImage;

            if (_isMultiplayer)
            {
                damageScreenImage = _multiplayerDamageScreens[playerIndex];
            }
            else
            {
                damageScreenImage = _singlePlayerDamageScreen;
            }

            damageScreenImage.color = new Color(damageScreenImage.color.r, damageScreenImage.color.g, damageScreenImage.color.b, _damageScreenAlpha);

            Color currentColor = damageScreenImage.color;
            float startAlpha = currentColor.a;
            float finalAlpha = lowHealth ? _lowHealthScreenAlpha : 0f;
            float newAlpha = startAlpha;
            float t = 0f;

            while (currentColor.a != finalAlpha)
            {
                t += _damageScreenFadeSpeed * Time.deltaTime;

                // Calculate the new alpha based on time
                newAlpha = Mathf.Lerp(startAlpha, finalAlpha, t);

                // Update the image color with the new alpha
                currentColor.a = newAlpha;
                damageScreenImage.color = currentColor;

                yield return null;
            }

            // Make sure the final alpha is exactly the target alpha
            Color finalColor = damageScreenImage.color;
            finalColor.a = finalAlpha;
            damageScreenImage.color = finalColor;
        }

        public void HideLowHealthOverlay(int playerIndex)
        {
            if(!_lowHealthOverlay[playerIndex])
            {
                return;
            }

            if (_damageScreenCoroutines[playerIndex] != null)
            {
                StopCoroutine(_damageScreenCoroutines[playerIndex]);
            }

            _lowHealthOverlay[playerIndex] = false;

            if (_lowHealthOverlayCoroutines[playerIndex] != null)
            {
                StopCoroutine(_lowHealthOverlayCoroutines[playerIndex]);
            }

            _lowHealthOverlayCoroutines[playerIndex] = StartCoroutine(HideLowHealthOverlayRoutine(playerIndex));
        }

        private IEnumerator HideLowHealthOverlayRoutine(int playerIndex)
        {
            Image damageScreenImage;

            if (_isMultiplayer)
            {
                damageScreenImage = _multiplayerDamageScreens[playerIndex];
            }
            else
            {
                damageScreenImage = _singlePlayerDamageScreen;
            }

            Color currentColor = damageScreenImage.color;
            float startAlpha = currentColor.a;
            float finalAlpha = 0f;
            float newAlpha = startAlpha;
            float t = 0f;

            while (currentColor.a != finalAlpha)
            {
                t += _damageScreenFadeSpeed * Time.deltaTime;

                // Calculate the new alpha based on time
                newAlpha = Mathf.Lerp(startAlpha, finalAlpha, t);

                // Update the image color with the new alpha
                currentColor.a = newAlpha;
                damageScreenImage.color = currentColor;

                yield return null;
            }

            // Make sure the final alpha is exactly the target alpha
            Color finalColor = damageScreenImage.color;
            finalColor.a = finalAlpha;
            damageScreenImage.color = finalColor;
        }
    }
}
