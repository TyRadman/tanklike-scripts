using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Combat.SkillTree.SkillSelection
{
    using Utils;

    public class SkillHolderCell : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Transform _rectTransform;
        [SerializeField] private Color _activationColor;

        [Header("Animations")]
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _highlightAnimation;
        [SerializeField] private AnimationClip _changeIconAnimation;
        [SerializeField] private AnimationClip _auraPulseAnimation;
        
        private Sprite _iconSpriteToSet;
        public void SetIconSprite(Sprite sprite)
        {
            _iconImage.sprite = sprite;
        }

        public void UpdatePosition(Vector2 position)
        {
            _rectTransform.position = position;
        }

        public void UpdateLocalPosition(Vector2 position)
        {
            _rectTransform.localPosition = position;
        }

        public void UpdateScale(float scale)
        {
            _rectTransform.localScale = Vector3.one * scale;
        }

        public Vector2 GetPosition()
        {
            return _rectTransform.position;
        }

        public Vector2 GetLocalPosition()
        {
            return _rectTransform.localPosition;
        }

        public void SetNewIcon(Sprite iconSprite)
        {
            _iconSpriteToSet = iconSprite;
            this.PlayAnimation(_animation, _changeIconAnimation);
        }

        public void SetIcon()
        {
            _iconImage.sprite = _iconSpriteToSet;
            _iconImage.color = _activationColor;
        }

        public void PlayHighlightAnimation()
        {
            this.PlayAnimation(_animation, _highlightAnimation);

            Invoke(nameof(PlayPulseAnimation), _highlightAnimation.length + 0.2f);
        }

        private void PlayPulseAnimation()
        {
            this.PlayAnimation(_animation, _auraPulseAnimation);
        }
    }
}
