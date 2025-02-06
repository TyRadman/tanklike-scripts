using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree
{
    using UnitControllers;
    using Upgrades;

    /// <summary>
    /// Responsible for retrieving and providing the upgrades of the skills that the player is equipping.
    /// </summary>
    public class SkillTreeUpgradesController : MonoBehaviour
    {
        [SerializeField] private List<UpgradesList> _upgrades = new List<UpgradesList>()
        {
            new UpgradesList(){UpgradeType = UpgradeTypes.BaseWeapon, Upgrades = new List<SkillUpgrade>(), PlayerIndex = 0},
            new UpgradesList(){UpgradeType = UpgradeTypes.ChargeAttack, Upgrades = new List<SkillUpgrade>(), PlayerIndex = 0},
            new UpgradesList(){UpgradeType = UpgradeTypes.SuperAbility, Upgrades = new List<SkillUpgrade>(), PlayerIndex = 0},
            new UpgradesList(){UpgradeType = UpgradeTypes.StatsUpgrade, Upgrades = new List<SkillUpgrade>(), PlayerIndex = 0},
            new UpgradesList(){UpgradeType = UpgradeTypes.SpecialUpgrade, Upgrades = new List<SkillUpgrade>(), PlayerIndex = 0}
        };

        public void SetUp(PlayerComponents player)
        {
            SetUpgrades(UpgradeTypes.BaseWeapon, player.Upgrades.GetBaseWeaponSkillProfile().Upgrades, player);
            SetUpgrades(UpgradeTypes.ChargeAttack, player.Upgrades.GetChargeAttackSkillProfile().Upgrades, player);
            SetUpgrades(UpgradeTypes.SuperAbility, player.Upgrades.GetSuperAbilitySkillProfile().Upgrades, player);
            SetUpgrades(UpgradeTypes.StatsUpgrade, player.Upgrades.GetStatUpgrades(), player);
            SetUpgrades(UpgradeTypes.SpecialUpgrade, player.Upgrades.GetSpecialUpgrades(), player);
        }

        public void SetUpgrades(UpgradeTypes type, List<SkillUpgrade> upgrades, PlayerComponents player)
        {
            UpgradesList list = _upgrades.Find(u => u.UpgradeType == type);
            list.Upgrades.Clear();

            List<SkillUpgrade> upgradesCopy = new List<SkillUpgrade>();
            list.PlayerIndex = player.PlayerIndex;

            for (int i = 0; i < upgrades.Count; i++)
            {
                if (upgrades[i] == null)
                {
                    continue;
                }

                SkillUpgrade upgradeClone = upgrades[i].Clone();
                upgradeClone.name = upgrades[i].name + "_CLONE";
                //Debug.Log($"Cloned upgrade: {upgradeClone.GetInstanceID()}, and the original is {upgrades[i].GetInstanceID()}");
                upgradeClone.SetUp(player);
                upgradesCopy.Add(upgradeClone);
            }

            list.Upgrades.AddRange(upgradesCopy);

            //Debug.Log($"List of type {type} has {list.Upgrades.Count} upgrades. Non-clones status: {list.Upgrades.Exists(u => !u.IsRuntimeInstance())}. Player: {player.PlayerIndex}");
        }

        public List<SkillUpgrade> GetUpgrades(UpgradeTypes type, int playerIndex)
        {
            UpgradesList list = _upgrades.Find(u => u.UpgradeType == type && u.PlayerIndex == playerIndex);

            if (list == null)
            {
                Debug.LogError($"No upgrades of type {type}");
                return null;
            }

            List<SkillUpgrade> upgrades = list.Upgrades.FindAll(u => u.IsAvailable);

            if (upgrades.Count == 0)
            {
                Debug.LogError($"No available upgrades of type {type}");
                return null;
            }

            return upgrades;
        }
    }

    [System.Serializable]
    public class UpgradesList
    {
        public List<SkillUpgrade> Upgrades;
        public UpgradeTypes UpgradeType;
        public int PlayerIndex;
    }
}
