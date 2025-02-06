using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Combat.SkillTree
{
    using Upgrades;
    using UnitControllers;
    using UI.SkillTree;
    using System;

    public class SkillTreeUpgradeCell : UICell, ICellSelectable
    {
        public SkillUpgrade OnUnlockedUpgrade { get; private set; }
        [field: SerializeField] public UpgradeTypes UpgradeType { get; set; }
        public List<SkillsConnectedCell> ConnectedCells = new List<SkillsConnectedCell>();

        [field: SerializeField, Header("References")] public RectTransform RectTransform { get; private set; }
        [SerializeField] private Image _cellIconImage;
        [SerializeField] private Image _highlightImage;
        [SerializeField] private Image _coverImage;

        [Header("Animations")]
        [SerializeField] private Animator _animator;

        private readonly int _onSelectedParamHash = Animator.StringToHash("OnSelected");
        private readonly int _onDeselectedParamHash = Animator.StringToHash("OnDeselected");

        public static Color LOCKED_PATH_COLOR = Color.white;
        public static Color UNLOCKED_PATH_COLOR = Color.white;
        public static Color UNAVAILABLE_PATH_COLOR = Color.white;

        public override void SetUp()
        {

        }

        public override void Highlight()
        {
            _highlightImage.enabled = true;
            _animator.SetTrigger(_onSelectedParamHash);
        }

        public override void Unhighlight()
        {
            _highlightImage.enabled = false;
            _animator.SetTrigger(_onDeselectedParamHash);
        }

        public void SetUpgrade(SkillUpgrade upgrade)
        {
            OnUnlockedUpgrade = upgrade;
            SetIcon(upgrade.GetUpgradeMainInfo().Icon);
        }

        public override void SetIcon(Sprite iconSprite)
        {
            _cellIconImage.sprite = iconSprite;
        }

        #region Data Getters
        public string GetName()
        {
            if(OnUnlockedUpgrade == null)
            {
                return string.Empty;
            }

            return OnUnlockedUpgrade.GetUpgradeMainInfo().Name;
        }

        public string GetDescription()
        {
            if (OnUnlockedUpgrade == null)
            {
                return string.Empty;
            }

            string description = OnUnlockedUpgrade.GetUpgradeMainInfo().Description;
            description += $"\n\n" + OnUnlockedUpgrade.GetUpgradeProperties().Trim();

            return description;
        }

        internal void SetIconColor(Color color)
        {
            _cellIconImage.color = color;
        }

        public ICellSelectable Navigate(Direction direction)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
