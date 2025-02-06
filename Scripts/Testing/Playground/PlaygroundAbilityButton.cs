using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Testing.Playground
{
    using Combat.Abilities;

    public class PlaygroundAbilityButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _skillIconImage;
        [SerializeField] private Image _selectedImageIcon;

        private PlaygroundAbilitySelectionUIController _uiController;
        private SkillHolder _currentSkill;
        private PlaygroundAbilityHolderButton _skillHolderButton;

        public void SetUp(PlaygroundAbilitySelectionUIController uiController, SkillHolder skill, PlaygroundAbilityHolderButton skillHolderButton)
        {
            _uiController = uiController;
            _currentSkill = skill;
            _skillHolderButton = skillHolderButton;

            _skillIconImage.sprite = skill.GetIcon();

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                _uiController.CloseSelectionPanel();
                _currentSkill.EquipSkill(GameManager.Instance.PlayersManager.GetPlayer(0));
                _skillHolderButton.UpdateCurrentSkill(_currentSkill);
            });
        }

        public void ToggleSelectedIcon(bool value)
        {
            _selectedImageIcon.gameObject.SetActive(value);
            Debug.Log("Toggle is " + value);
        }
    }
}
