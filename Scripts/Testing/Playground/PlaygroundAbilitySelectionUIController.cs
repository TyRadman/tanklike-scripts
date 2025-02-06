using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Testing.Playground
{
    using TankLike.Combat.Abilities;
    using TankLike.Combat.SkillTree;
    using TankLike.Combat.SkillTree.Upgrades;
    using UnitControllers;

    public class PlaygroundAbilitySelectionUIController : MonoBehaviour
    {
        [Header("Abilities Buttons")]
        [SerializeField] private PlaygroundAbilityHolderButton _weaponButton;
        [SerializeField] private PlaygroundAbilityHolderButton _superAbilityButton;
        [SerializeField] private PlaygroundAbilityHolderButton _chargedAbilityButton;

        [Header("Selection Popup")]
        [SerializeField] private PlaygroundAbilitySelectionPopup _selectionPopup;

        [Header("Selection Popup")]
        [SerializeField] private PlaygroundAbilityUpgradeUIController _upgradePanel;

        private PlayerComponents _currentPlayer;

        public void SetUp(PlaygroundManager playgroundManager)
        {
            _currentPlayer = GameManager.Instance.PlayersManager.GetPlayer(0);
            _selectionPopup.SetUp(this);
            _upgradePanel.SetUp(this);
        }

        public void Init()
        {
            _currentPlayer.SuperAbilities.Unlock();
            _currentPlayer.ChargeAttack.Unlock();
            _weaponButton.SetUp(this, _currentPlayer.Shooter.GetWeaponHolder(), 0);
            _superAbilityButton.SetUp(this, _currentPlayer.SuperAbilities.GetAbilityHolder(), 1);
            _chargedAbilityButton.SetUp(this, _currentPlayer.ChargeAttack.GetAbilityHolder(), 2);
        }

        public void Close()
        {
            CloseSelectionPanel();
        }

        private List<SkillProfile> GetSkillsList(int holderIndex)
        {
            switch (holderIndex)
            {
                case 0:
                    return _currentPlayer.Upgrades.BaseWeaponSkills;
                case 1:
                    return _currentPlayer.Upgrades.SuperAbilitySkills;
                case 2:
                    return _currentPlayer.Upgrades.ChargeAbilitySkills;
                default:
                    return null;
            }
        }

        private PlaygroundAbilityHolderButton GetHolderButton(int holderIndex)
        {
            switch (holderIndex)
            {
                case 0:
                    return _weaponButton;
                case 1:
                    return _superAbilityButton;
                case 2:
                    return _chargedAbilityButton;
                default:
                    return null;
            }
        }

        public void OpenSelectionPanel(int holderIndex)
        {
            _selectionPopup.gameObject.SetActive(true);
            _selectionPopup.Init(GetSkillsList(holderIndex), GetHolderButton(holderIndex));
            _upgradePanel.gameObject.SetActive(true);
            _upgradePanel.Init(GetSkillsList(holderIndex), GetHolderButton(holderIndex));
        }

        public void CloseSelectionPanel()
        {
            _selectionPopup.gameObject.SetActive(false);
            _upgradePanel.gameObject.SetActive(false);
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
