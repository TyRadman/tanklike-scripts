using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.UI.Inventory
{
    using Utils;

    public class TabUI : SelectableCell
    {
        [SerializeField] private TextMeshProUGUI _tabText;
        [SerializeField] private Image _tabBG;
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _highlightAnimationClip;
        [SerializeField] private AnimationClip _dehighlightAnimationClip;

        public void HighLight()
        {
            this.PlayAnimation(_animation, _highlightAnimationClip);
        }

        public void Dehighlight()
        {
            this.PlayAnimation(_animation, _dehighlightAnimationClip);
        }

        public void SetName(string name)
        {
            _tabText.text = name;
        }
    }
}
