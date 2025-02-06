using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class ShotBar : MonoBehaviour
    {
        [SerializeField] private Image _barImage;
        private float _currentAmount;
        public bool IsFull = false;

        public void AddShots(float amount)
        {
            _currentAmount += amount;
            _currentAmount = Mathf.Clamp01(_currentAmount);
            _barImage.fillAmount = _currentAmount;
        }

        public void SetShots(float amount)
        {
            _currentAmount = amount;
            _currentAmount = Mathf.Clamp01(_currentAmount);
            _barImage.fillAmount = _currentAmount;
        }

        public float GetAmount()
        {
            return _currentAmount;
        }
    }
}
