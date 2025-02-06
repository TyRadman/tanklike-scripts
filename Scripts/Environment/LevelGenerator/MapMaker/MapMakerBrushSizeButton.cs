using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Environment.MapMaker
{
    public class MapMakerBrushSizeButton : MonoBehaviour
    {
        private MapMakerSelector _selector;

        [System.Serializable]
        public class BrushSize
        {
            public int Value;
            public Sprite Icon;
        }

        [SerializeField] private List<BrushSize> _brushSizes = new List<BrushSize>();
        [SerializeField] private Image _iconImage;
        private int _currentIndex = 0;

        public void SetUp(MapMakerSelector selector)
        {
            if(selector == null)
            {
                Debug.LogError("No selector passed");
                return;
            }

            _selector = selector;
        }

        public void OnClicked()
        {
            BrushSize selectedBrushSize = _brushSizes[++_currentIndex % _brushSizes.Count];

            _iconImage.sprite = selectedBrushSize.Icon;

            _selector.UpdateBrushSize(selectedBrushSize.Value);
        }
    }
}
