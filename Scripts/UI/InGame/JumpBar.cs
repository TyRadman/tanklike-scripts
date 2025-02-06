using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike
{
    using Utils;

    public class JumpBar : MonoBehaviour
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _showClip;
        [SerializeField] private AnimationClip _hideClip;
        [SerializeField] private Image _bar;

        public void SetAmount(float amount)
        {
            _bar.fillAmount = amount;
        }

        public void PlayShowAnimation()
        {
            this.PlayAnimation(_animation, _showClip);
        }

        public void PlayHideAnimation()
        {
            this.PlayAnimation(_animation, _hideClip);
        }

        public void ForceHide()
        {
            transform.localScale = Vector3.zero; // TODO: dirty, but does the job for now
        }
    }
}
