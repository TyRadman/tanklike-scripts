using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike.Testing.Playground
{
    using Combat.SkillTree.Upgrades;
    using Combat.SkillTree;
    using UnitControllers;
    using TankLike.Combat.Abilities;

    public class PlaygroundAbilityUpgradeUIController : MonoBehaviour
    {
        [SerializeField] private List<PlaygroundAbilityUpgradeContainer> _upgradeContainers;

        private PlaygroundAbilitySelectionUIController _abilitySelectionUIController;
        private PlayerComponents _currentPlayer;

        private Dictionary<string, SkillUpgradesContainer> _upgradesDictionary = new Dictionary<string, SkillUpgradesContainer>();

        public void SetUp(PlaygroundAbilitySelectionUIController abilitySelectionUIController)
        {
            _currentPlayer = GameManager.Instance.PlayersManager.GetPlayer(0);
            _abilitySelectionUIController = abilitySelectionUIController;

            _upgradeContainers.ForEach(c => c.SetUp(_currentPlayer));

            for (int i = 0; i < 3; i++)
            {
                List<SkillProfile> profiles = GetSkillsList(i);

                for (int j = 0; j < profiles.Count; j++)
                {
                    List<SkillUpgrade> currentHolderUpgrades = profiles[j].Upgrades;
                    List<SkillUpgradesContainer> upgrades = new List<SkillUpgradesContainer>();

                    for (int k = 0; k < currentHolderUpgrades.Count; k++)
                    {
                        if (currentHolderUpgrades[k] == null)
                        {
                            continue;
                        }

                        SkillUpgradesContainer upgradeClone = currentHolderUpgrades[k].Clone() as SkillUpgradesContainer;
                        upgradeClone.name = currentHolderUpgrades[k].name + "_CLONE";
                        upgradeClone.SetUp(_currentPlayer);
                        _upgradesDictionary.Add(currentHolderUpgrades[k].name, upgradeClone);
                    }
                }
            }

            //SetUpgrades();

            //_buttons.ForEach(b => b.gameObject.SetActive(false));

            //for (int i = 0; i < _specialUpgrades.Count; i++)
            //{
            //    _buttons[i].SetUp(this, _specialUpgrades[i]);
            //    _buttons[i].gameObject.SetActive(true);
            //}
        }

        public void Init(List<SkillProfile> skills, PlaygroundAbilityHolderButton skillHolderButton)
        {
            SkillProfile _skillProfile = null;

            for (int i = 0; i < skills.Count; i++)
            {
                string skillHolderName = skills[i].SkillHolder.name.Replace("(Clone)", "").Trim();
                string equippedSkillName = GetEquippedSkillByHolder(skillHolderButton.GetHolderIndex()).name.Replace("(Clone)", "").Trim();

                if (skillHolderName == equippedSkillName)
                {
                    _skillProfile = skills[i];
                    break;
                }
            }

            if (_skillProfile == null)
            {
                Debug.LogError("Cannot find a skill profile for the currently equipped skill holder!");
                return;
            }

            List<SkillUpgrade> upgrades = new List<SkillUpgrade>();

            foreach (var upgrade in _skillProfile.Upgrades)
            {
                if (_upgradesDictionary.ContainsKey(upgrade.name))
                {
                    upgrades.Add(_upgradesDictionary[upgrade.name]);
                }
            }

            _upgradeContainers.ForEach(b => b.gameObject.SetActive(false));

            for (int i = 0; i < upgrades.Count; i++)
            {
                _upgradeContainers[i].Init(upgrades[i] as SkillUpgradesContainer);
                _upgradeContainers[i].gameObject.SetActive(true);           
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
    }
}
