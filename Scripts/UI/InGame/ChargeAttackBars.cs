using System;
using UnityEngine;

namespace TankLike.UI.InGame
{
    public class ChargeAttackBars : MonoBehaviour
    {
        [SerializeField] private SegmentedBar _bar;
        [SerializeField] private SegmentedBar _perfectBar;
        [SerializeField] private SegmentedBar _highlightedBGBar;
        [SerializeField] private SegmentedBar _BGBar;

        private const float BG_ROTATION_OFFSET = -4f;
        private const float BG_VALUE_OFFSET = 0.02f;

        private const float MIN_ROTATION = 0f;
        private const float MAX_ROTATION = 360f;

        internal void SetUp()
        {
            _bar.SetUp();
            _perfectBar.SetUp();
            _highlightedBGBar.SetUp();
            _BGBar.SetUp();
        }

        internal void SetSize(Vector2 range)
        {
            float rotation = Mathf.Lerp(MIN_ROTATION, MAX_ROTATION, range.x);

            _perfectBar.SetStartRotation(rotation);
            _perfectBar.SetTotalAmount(0f);

            _highlightedBGBar.SetStartRotation(rotation);
            float amount = range.y - range.x;
            _highlightedBGBar.SetTotalAmount(amount);

            _BGBar.SetStartRotation(rotation + BG_ROTATION_OFFSET);
            _BGBar.SetTotalAmount(amount + BG_VALUE_OFFSET);
        }

        internal float GetMainBarAmount()
        {
            return _bar.GetAmount();
        }

        internal void SetMainBarAmount(float amount)
        {
            _bar.SetTotalAmount(amount);
        }

        internal void UpdateBarsValue(float value)
        {
            _bar.SetTotalAmount(value);
        }

        internal void UpdatePerfectChargeBarsValue(float value, Vector2 perfectChargeRange, Vector2 perfectChargeValueRange)
        {
            float perfectProgress = Mathf.InverseLerp(perfectChargeRange.x, perfectChargeRange.y, value);
            float amount = Mathf.Lerp(perfectChargeValueRange.x, perfectChargeValueRange.y, perfectProgress);
            _perfectBar.SetTotalAmount(amount);
        }

        internal void ResetBarsAmount()
        {
            _bar.SetTotalAmount(0f);
            _perfectBar.SetTotalAmount(0f);
        }

        internal void HidePerfectChargeBars()
        {
            _perfectBar.SetTotalAmount(0f);
            _BGBar.SetTotalAmount(0f);
            _highlightedBGBar.SetTotalAmount(0f);
        }
    }
}
