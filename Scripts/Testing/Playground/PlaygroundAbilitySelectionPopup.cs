using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Testing.Playground
{
    using Combat.SkillTree;
    using TankLike.Combat.Abilities;
    using UnitControllers;

    public class PlaygroundAbilitySelectionPopup : MonoBehaviour
    {
        [SerializeField] private List<PlaygroundAbilityButton> _buttons;
        
        private PlaygroundAbilitySelectionUIController _abilitySelectionUIController;
        private PlaygroundAbilityHolderButton _skillHolderButton;
        private PlayerComponents _currentPlayer;

        public void SetUp(PlaygroundAbilitySelectionUIController abilitySelectionUIController)
        {
            _abilitySelectionUIController = abilitySelectionUIController;
            _currentPlayer = GameManager.Instance.PlayersManager.GetPlayer(0);
        }

        public void Init(List<SkillProfile> skills, PlaygroundAbilityHolderButton skillHolderButton)
        {
            _skillHolderButton = skillHolderButton;

            _buttons.ForEach(b => b.gameObject.SetActive(false));

            for (int i = 0; i < skills.Count; i++)
            {
                _buttons[i].SetUp(_abilitySelectionUIController, skills[i].SkillHolder, _skillHolderButton);
                _buttons[i].gameObject.SetActive(true);

                string skillHolderName = skills[i].SkillHolder.name.Replace("(Clone)", "").Trim();
                string equippedSkillName = GetEquippedSkillByHolder(skillHolderButton.GetHolderIndex()).name.Replace("(Clone)", "").Trim();

                //Debug.Log("currentHolder " + equippedSkillName);
                //Debug.Log("skills[i].SkillHolder " + skillHolderName);

                if (skillHolderName == equippedSkillName)
                {
                    _buttons[i].ToggleSelectedIcon(true);
                }
                else
                {
                    _buttons[i].ToggleSelectedIcon(false);
                }
            }
        }

        private SkillHolder GetEquippedSkillByHolder(int holderIndex)
        {
            switch (holderIndex)
            {
                case 0:
                    return _currentPlayer.Shooter.GetWeaponHolder();
                case 1:
                    return _currentPlayer.SuperAbilities.GetAbilityHolder();
                case 2:
                    return _currentPlayer.ChargeAttack.GetAbilityHolder();
                default:
                    return null;
            }
        }
    }
}
