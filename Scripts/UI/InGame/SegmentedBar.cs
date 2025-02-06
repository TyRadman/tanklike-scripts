using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.InGame
{
    public class SegmentedBar : MonoBehaviour
    {
        [System.Serializable]
        public class BarLayer
        {
            public SpriteRenderer Sprite;
            [HideInInspector] public Material Material;
            public bool Fill;

            public void SetUp()
            {
                Material = Sprite.material;
            }

            public Material GetMaterial()
            {
                if(Material == null)
                {
                    return Sprite.material;
                }

                return Material;
            }
        }

        [SerializeField] private List<BarLayer> _bars;
        
        private int _segmentsCount = 0;

        private const string LINE_WIDTH = "_LineWidth";
        private const string COLOR = "_Color";
        private const string ROTATION = "_Rotation";
        private const string VALUE = "_Value";
        private const string SPACING = "_SegmentSpacing";
        private const string COUNT = "_SegmentCount";
        private const string ALPHA = "_Alpha";
        private const string INNER_RADIUS = "_InnerRadius";
        private const string OUTER_RADIUS = "_OuterRadius";

        public void SetUp()
        {
            _bars.ForEach(b => b.SetUp());
            _segmentsCount = _bars[0].GetMaterial().GetInt(COUNT);
        }

        public void SetColor(Color color)
        {
            _bars.ForEach(b => b.GetMaterial().SetColor(COLOR, color));
        }

        public void SetTotalAmount(float amount)
        {
            List<BarLayer> bars = _bars.FindAll(b => b.Fill);

            bars.ForEach(b => b.GetMaterial().SetFloat(VALUE, amount)); 

            if (bars[0].GetMaterial().GetFloat(VALUE) == 0f)
            {
                bars.ForEach(b => b.GetMaterial().SetFloat(ALPHA, 0));
            }
            else
            {
                bars.ForEach(b => b.GetMaterial().SetFloat(ALPHA, 1));
            }
        }

        public void AddAmountToSegments(float amount)
        {
            List<BarLayer> bars = _bars.FindAll(b => b.Fill);
            amount /= _segmentsCount;
            float newAmount = bars[0].GetMaterial().GetFloat(VALUE) + amount;
            newAmount = Mathf.Clamp01(newAmount);
            bars.ForEach(b => b.GetMaterial().SetFloat(VALUE, newAmount));

            if (bars[0].GetMaterial().GetFloat(VALUE) == 0f)
            {
                bars.ForEach(b => b.GetMaterial().SetFloat(ALPHA, 0));
            }
            else
            {
                bars.ForEach(b => b.GetMaterial().SetFloat(ALPHA, 1));
            }
        }

        public float GetAmount()
        {
            return _bars.Find(b => b.Fill).GetMaterial().GetFloat(VALUE);
        }

        public void SetCount(int barsCount)
        {
            if (_bars == null || _bars.Exists(b => b == null) || _bars.Exists(b => b.GetMaterial() == null))
            {
                Debug.Log(gameObject.name);
            }

            _bars.ForEach(b => b.GetMaterial().SetInt(COUNT, barsCount));
            _segmentsCount = _bars[0].GetMaterial().GetInt(COUNT);
        }

        internal void SetStartRotation(float rotation)
        {
            _bars.ForEach(b => b.GetMaterial().SetFloat(ROTATION, rotation));
        }
    }
}
