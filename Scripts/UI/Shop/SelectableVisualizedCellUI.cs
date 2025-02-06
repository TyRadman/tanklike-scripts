using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class SelectableVisualizedCellUI : SelectableCell
    {
        [Header("Cell Variables")]
        [SerializeField] protected Image _iconImage;
        [SerializeField] protected GameObject _overLayImageObject;
        [SerializeField] protected Image _highlightImage;

        [Header("Extra")]
        [SerializeField] private Sprite _emptyCellSprite;

        public override void ResetCell()
        {
            base.ResetCell();
            _iconImage.sprite = _emptyCellSprite;
        }

        public override void HighLight(bool highlight)
        {
            base.HighLight(highlight);

            if (_highlightImage == null) return;

            _highlightImage.enabled = highlight;
        }

        public virtual void SetIcon(Sprite iconSprite)
        {
            _iconImage.sprite = iconSprite;
            _iconImage.enabled = true;
        }
    }
}
