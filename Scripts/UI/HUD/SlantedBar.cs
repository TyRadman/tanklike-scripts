using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.HUD
{
    using Utils;

    public class SlantedBar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private RectTransform _barTransform;
        [SerializeField] private Vector2 _positionRange = new Vector2(0f, 0f);

        /// <summary>
        /// Set the bar position based of the value.
        /// </summary>
        /// <param name="value">A float value from 0 to 1.</param>
        public void UpdateBar(float currentValue, float maxValue)
        {
            if (_barTransform == null)
            {
                return;
            }

            float value = currentValue / maxValue;
            float xPosition = _positionRange.Lerp(value);
            _barTransform.anchoredPosition = new Vector2(xPosition, _barTransform.anchoredPosition.y); 
        }
    }
}
