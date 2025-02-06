using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.SkillTree
{
    using Combat;
    using Combat.Abilities;
    using System;

    public class SwappableCell : UICell, ICellSelectable
    {
        public enum PlayerSelectionState
        {
            Active = 0, NotActive = 1
        }

        public enum HighlightState
        {
            Highlighted = 0, NotHighlighted = 1
        }

        public SkillHolder Skill { get; private set; }

        [field: SerializeField] public List<SkillsConnectedCell> ConnectedCells { get; private set; } = new List<SkillsConnectedCell>();
        [field: SerializeField] public PlayerSelectionState CellState { set; get; }
        [field: SerializeField] public HighlightState CellHighlightState { get; private set; }
        [SerializeField] private Animator _animator;
        [SerializeField] private Image _highlightImage;
        [SerializeField] private Image _activeCellImage;
        [SerializeField] private Image _iconImage;

        private readonly int AnimationSpeedMultiplierKey = Animator.StringToHash("Speed");

        public override void SetUp()
        {
            Unhighlight();
        }

        public void SetSkill(SkillHolder skill)
        {
            if(skill == null)
            {
                return;
            }

            Skill = skill;
            SetIcon(Skill.GetIcon());
        }

        public override void SetIcon(Sprite iconSprite)
        {
            _iconImage.sprite = iconSprite;
        }

        public void SetActiveSkill()
        {
            CellState = PlayerSelectionState.Active;
            _activeCellImage.enabled = true;
        }

        public void SetInactiveSkill()
        {
            CellState = PlayerSelectionState.NotActive;
            _activeCellImage.enabled = false;
        }

        public override void Highlight()
        {
            CellHighlightState = HighlightState.Highlighted;
            _highlightImage.enabled = true;
        }

        public override void Unhighlight()
        {
            CellHighlightState = HighlightState.NotHighlighted;
            _highlightImage.enabled = false;
        }

        public void SetHoldAnimationDurationMultiplier(float animationSpeedMultiplier)
        {
            _animator.SetFloat(AnimationSpeedMultiplierKey, animationSpeedMultiplier);
        }

        public ICellSelectable Navigate(Direction direction)
        {
            throw new NotImplementedException();
        }
    }
}
