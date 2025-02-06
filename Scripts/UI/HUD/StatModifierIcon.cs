using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.HUD
{
    public class StatModifierIcon : MonoBehaviour
    {
        public bool IsEnabled { get; private set; } = false;
        public StatIconReference StatModifierSprite { get; private set; }

        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _coverImage;

        public void Enable()
        {
            _iconImage.enabled = true;
            _coverImage.enabled = true;
            IsEnabled = true;
        }

        public void Disable()
        {
            //Debug.Log($"No such thing with {statIcon.name}");
            _iconImage.enabled = false;
            _coverImage.enabled = false;
            IsEnabled = false;
            StatModifierSprite = null;
        }

        public void SetIconSprite(StatIconReference iconSprite)
        {
            _iconImage.sprite = iconSprite.IconSprite;
            StatModifierSprite = iconSprite;
        }
    }
}
