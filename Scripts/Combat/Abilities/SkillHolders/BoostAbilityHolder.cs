using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using Combat.SkillTree;
    using Combat.SkillTree.Upgrades;
    using UnitControllers;

    [CreateAssetMenu(fileName = "BA_NAME", menuName = Directories.ABILITIES_HOLDER + "Boost Ability Holder")]
    public class BoostAbilityHolder : SkillHolder
    {
        [field: SerializeField] public Ability Ability { get; set; }

        [Tooltip("How far should the tank travel before the ability is activated again (only if it's on update.)")]
        [field: SerializeField] public float DistancePerActivation { get; private set; } = 1f;

        [Tooltip("How much more fuel will be consumed on start when boosting with this ability equipped.")]
        [field: SerializeField, Header("Modifiers")] public float FuelStartConsumptionRate { get; private set; } = 10f;
        [field: SerializeField] public float FuelConsumptionRate { get; private set; } = 10f;
        [field: SerializeField] public float SpeedMultiplier { get; private set; } = 1.5f;

        public override void PopulateSkillProperties()
        {
            Ability.SkillDisplayProperties.Clear();

            AddSkillProperty(Ability.SkillDisplayProperties, "Speed increase", SpeedMultiplier * 100f, Colors.Red, "%");
            float consumptionRate = (FuelStartConsumptionRate + FuelConsumptionRate) / 2f * 100f;
            AddSkillProperty(Ability.SkillDisplayProperties, "Fuel consumption", consumptionRate, Colors.DodgerBlue, "FP");

            Ability.PopulateSkillProperties();
        }

        public override List<SkillProperties> GetProperties()
        {
            return Ability.SkillDisplayProperties;
        }

        public override void ApplySkill(TankComponents components)
        {
            PlayerComponents playerComponents = components as PlayerComponents;

            playerComponents.BoostAbility.AddSkill(this);
        }

        public override void EquipSkill(TankComponents components)
        {
            PlayerComponents playerComponents = components as PlayerComponents;

            playerComponents.BoostAbility.SetActiveAbility(this);
        }

        public override void UpgradeSkill(SkillUpgrade upgrade, PlayerComponents player)
        {
            if(upgrade == null)
            {
                Debug.LogError("No upgrade passed.");
            }

            if(upgrade is BoostAbilityHolderUpgrades boostUpgrades)
            {
                FuelConsumptionRate += boostUpgrades.FuelConsumptionRate;
                FuelStartConsumptionRate += boostUpgrades.StartFuelConsumptionAmount;
                SpeedMultiplier += boostUpgrades.SpeedMultiplier;

                return;
            }

            if (upgrade is not AbilityUpgrades abilityUpgrades)
            {
                Debug.LogError("No upgrade passed.");
                return;
            }

            Ability.Upgrade(abilityUpgrades);
        }

        public void SetAbility(Ability ability)
        {
            Ability = ability;
        }
    }
}
