using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TankLike.Tutorial
{
    using Utils;

    public class TutorialInstructions : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textBox;
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _displayClip;
        [SerializeField] private AnimationClip _hideClip;

        private bool _isDisplayed = false;

        public void Display()
        {
            if (_isDisplayed)
            {
                return;
            }

            _isDisplayed = true;
            this.PlayAnimation(_animation, _displayClip);
        }

        public void Hide()
        {
            if (!_isDisplayed)
            {
                return;
            }

            _isDisplayed = false;
            this.PlayAnimation(_animation, _hideClip);
        }

        public void SetText(string text)
        {
            _textBox.text = text;
        }
    }
}
