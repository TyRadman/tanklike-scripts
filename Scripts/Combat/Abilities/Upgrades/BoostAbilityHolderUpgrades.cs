using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.SkillTree;
    using TankLike.Combat.Abilities;
    using UnitControllers;

    [CreateAssetMenu(fileName = "BAHU_NAME", menuName = Directories.HOLDERS_UPGRADES + "Boost Ability Holder Upgrade")]
    public class BoostAbilityHolderUpgrades : SkillUpgrade
    {
        [field: SerializeField] public float SpeedMultiplier { get; private set; }
        [field: SerializeField] public int StartFuelConsumptionAmount { get; private set; }
        [field: SerializeField] public int FuelConsumptionRate { get; private set; }

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            BoostAbilityHolder holder = player.BoostAbility.GetAbilityHolder();

            if(holder == null )
            {
                Debug.Log("Holder is null");
                return;
            }

            if (SpeedMultiplier != 0)
            {
                float originalSpeedMultiplier = holder.SpeedMultiplier;

                SkillProperties speedMultiplier = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Speed Increment",
                    PreviousValue = (originalSpeedMultiplier * 100f).ToString(),
                    Value = ((SpeedMultiplier + originalSpeedMultiplier) * 100f).ToString(),
                    DisplayColor = Colors.DarkOrange,
                    UnitString = PropertyUnits.PERCENTAGE
                };

                _properties.Add(speedMultiplier);
            }

            if (FuelConsumptionRate != 0)
            {
                float originalFuelConsumptionRate = holder.FuelConsumptionRate;
                float newFuelConsumtpionRate = Mathf.Max(0f, originalFuelConsumptionRate + FuelConsumptionRate);

                SkillProperties speedMultiplier = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Fuel Consumption Rate",
                    PreviousValue = originalFuelConsumptionRate.ToString(),
                    Value = (newFuelConsumtpionRate).ToString(),
                    DisplayColor = Colors.DodgerBlue,
                    UnitString = PropertyUnits.POINTS
                };

                _properties.Add(speedMultiplier);
            }

            SaveProperties();
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            _player.BoostAbility.GetAbilityHolder().UpgradeSkill(this, _player);
            _player.BoostAbility.ReEquipSkill();
            _player.Upgrades.GetBoostAbilitySkillProfile().Upgrades.ForEach(u => u.SetUpgradeProperties(_player));
        }
    }
}
