using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.Abilities;
    using Combat.SkillTree;
    using UnitControllers;
    
    [CreateAssetMenu(fileName = "SAHU_NAME", menuName = Directories.HOLDERS_UPGRADES + "Super Ability Holder Upgrade")]
    public class SuperAbilityHolderUpgrade : SkillUpgrade
    {
        [field: SerializeField] public RechargingValues RechargingMethod { get; private set; }
        [field: SerializeField] public AbilityConstraint OnAimConstraints { get; private set; }
        [field: SerializeField] public AbilityConstraint OnPerformConstraints { get; private set; }

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            SuperAbilityHolder holder = player.SuperAbilities.GetAbilityHolder();

            if (RechargingMethod.Time != 0f)
            {
                float previousValue = holder.RechargeInfo.Time;
                float newValue = RechargingMethod.Time + previousValue;

                SkillProperties rechargeTime = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Time to charge",
                    Value = newValue.ToString(),
                    DisplayColor = Colors.Green,
                    UnitString = PropertyUnits.SECONDS
                };

                _properties.Add(rechargeTime);
            }

            if (RechargingMethod.NumberOfEnemyHits != 0)
            {
                int previousValue = holder.RechargeInfo.NumberOfEnemyHits;
                int newValue = RechargingMethod.NumberOfEnemyHits + previousValue;

                SkillProperties numberOfEnemyHits = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Number of enemy hits",
                    Value = newValue.ToString(),
                    DisplayColor = Colors.DarkOrange,
                    UnitString = PropertyUnits.HITS
                };

                _properties.Add(numberOfEnemyHits);
            }

            if (RechargingMethod.NumberOfHits != 0)
            {
                int previousValue = holder.RechargeInfo.NumberOfHits;
                int newValue = RechargingMethod.NumberOfHits + previousValue;

                SkillProperties numberOfHits = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Number hits",
                    Value = newValue.ToString(),
                    DisplayColor = Colors.Red,
                    UnitString = "Hits"
                };

                _properties.Add(numberOfHits);
            }

            SaveProperties();
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            _player.SuperAbilities.GetAbilityHolder().UpgradeSkill(this, _player);
            _player.SuperAbilities.ReEquipSkill();
            _player.Upgrades.GetSuperAbilitySkillProfile().Upgrades.ForEach(u => u.SetUpgradeProperties(_player));
        }
    }
}
