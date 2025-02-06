using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.PauseMenu
{
    /// <summary>
    /// Handles all the functionality related to the pause menu's settings.
    /// </summary>
    public class PauseMenuSettings : MonoBehaviour
    {
        [SerializeField] private Image _aimSensitivityBar;
        private const float SENSITIVITY_ADD_AMOUNT = 0.05f;
        [SerializeField] private float _aimSensitivityAmout;
        private float _minSensitivity;
        private float _maxSensitivity;
        private int _currentPlayerIndex = -1;

        private void Awake()
        {
            _minSensitivity = PlayerCrosshairController.MIN_AIM_SENSITIVITY;
            _maxSensitivity = PlayerCrosshairController.MAX_AIM_SENSITIVITY;
        }

        public void OnPanelOpened()
        {
            float amount = GameManager.Instance.PlayersManager.GetGameplaySettings(_currentPlayerIndex).AimSensitivity;
            _aimSensitivityAmout = Mathf.InverseLerp(_minSensitivity, _maxSensitivity, amount);
            _aimSensitivityBar.fillAmount = _aimSensitivityAmout;
        }

        public void SetAimSensitivity(bool add)
        {
            // add to the sensitivity value
            _aimSensitivityAmout = Mathf.Clamp01(_aimSensitivityAmout + (add ? SENSITIVITY_ADD_AMOUNT : -SENSITIVITY_ADD_AMOUNT));
            // update the bar
            _aimSensitivityBar.fillAmount = _aimSensitivityAmout;
            // set the value for the player's sensitivity
            float amount = Mathf.Lerp(_minSensitivity, _maxSensitivity, _aimSensitivityAmout);
            GameManager.Instance.PlayersManager.SetAimSensitivity(amount, _currentPlayerIndex);
        }

        public void SetPlayerIndex(int playerIndex)
        {
            _currentPlayerIndex = playerIndex;
        }
    }
}
