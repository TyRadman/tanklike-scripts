using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Testing.Playground
{
    using Combat.Abilities;

    public class PlaygroundAbilityHolderButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _skillIconImage;

        private PlaygroundAbilitySelectionUIController _uiController;
        private SkillHolder _currentSkill;
        private int _holderIndex;

        public void SetUp(PlaygroundAbilitySelectionUIController uiController, SkillHolder skill, int holderIndex)
        {
            _uiController = uiController;
            _currentSkill = skill;
            _holderIndex = holderIndex;

            _skillIconImage.sprite = skill.GetIcon();

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                _uiController.OpenSelectionPanel(_holderIndex);
            });
        }

        public void UpdateCurrentSkill(SkillHolder skill)
        {
            _currentSkill = skill;
            _skillIconImage.sprite = skill.GetIcon();
        }

        public int GetHolderIndex()
        {
            return _holderIndex;
        }
    }
}
