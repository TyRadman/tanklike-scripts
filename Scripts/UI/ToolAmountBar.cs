using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike
{
    public class ToolAmountBar : MonoBehaviour
    {
        [SerializeField] private Image _barImage;
        [SerializeField] private List<Graphic> _visuals;

        private void OnEnable()
        {

        }

        public void EnableImage()
        {
            _visuals.ForEach(v => v.enabled = true);
        }

        public void DisableImage()
        {
            _visuals.ForEach(v => v.enabled = false);
        }

        public void SetFillAmount(float t)
        {
            _barImage.fillAmount = t;
        }

        public float GetFillAmount()
        {
            return _barImage.fillAmount;
        }
    }
}
