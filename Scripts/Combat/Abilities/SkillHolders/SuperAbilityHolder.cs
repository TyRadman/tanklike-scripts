using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using Combat.SkillTree;
    using Combat.SkillTree.Upgrades;
    using UnitControllers;

    [CreateAssetMenu(fileName = "SAH_NAME", menuName = Directories.ABILITIES_HOLDER + "Super Ability Holder")]
    public class SuperAbilityHolder : SkillHolder
    {
        [field: SerializeField] public Ability Ability { set; get; }
        [field: SerializeField] public RechargingValues RechargeInfo { get; private set; }
        [field: SerializeField] public AbilityConstraint OnAimConstraints { get; private set; }
        [field: SerializeField] public AbilityConstraint OnPerformConstraints { get; private set; }

        public override void PopulateSkillProperties()
        {
            Ability.SkillDisplayProperties.Clear();

            AddSkillProperty(Ability.SkillDisplayProperties, "Shots on enemies to charge", RechargeInfo.NumberOfEnemyHits, Colors.DarkCyan, "hits");

            Ability.PopulateSkillProperties();
        }

        public override List<SkillProperties> GetProperties()
        {
            return Ability.SkillDisplayProperties;
        }

        public override void ApplySkill(TankComponents components)
        {
            components.SuperAbility.AddSkill(this);
        }

        public override void EquipSkill(TankComponents components)
        {
            components.SuperAbility.EquipSkill(this);
        }

        public override void UpgradeSkill(SkillUpgrade upgrade, PlayerComponents player)
        {
            if (upgrade == null)
            {
                Debug.LogError("No upgrade passed.");
                return;
            }

            if (upgrade is SuperAbilityHolderUpgrade superUpgrades)
            {
                RechargeInfo.AddValues(superUpgrades.RechargingMethod);

                if (superUpgrades.OnAimConstraints != 0)
                {
                    OnAimConstraints = superUpgrades.OnAimConstraints;
                }

                if (superUpgrades.OnPerformConstraints != 0)
                {
                    OnPerformConstraints = superUpgrades.OnPerformConstraints;
                }

                return;
            }

            if (upgrade is not AbilityUpgrades abilityUpgrades)
            {
                Debug.LogError("No upgrade passed.");
                return;
            }

            Ability.Upgrade(abilityUpgrades);
        }
    }

    [System.Serializable]
    public class RechargingValues
    {
        public AbilityRechargingMethod RechargingMethod;
        public float Time;
        public int NumberOfEnemyHits;
        public int NumberOfHits;

        public void AddValues(RechargingValues values)
        {
            RechargingMethod |= values.RechargingMethod;

            Time += values.Time;
            
            NumberOfEnemyHits += values.NumberOfEnemyHits;
            
            NumberOfHits += values.NumberOfHits;
        }
    }
}
