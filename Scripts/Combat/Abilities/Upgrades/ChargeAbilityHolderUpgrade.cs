using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.SkillTree;
    using TankLike.Combat.Abilities;
    using UnitControllers;

    [CreateAssetMenu(fileName = "CAHU_NAME", menuName = Directories.HOLDERS_UPGRADES + "Charge Ability Holder Upgrade")]
    public class ChargeAbilityHolderUpgrade : SkillUpgrade
    {
        [field: SerializeField] public float ChargeDuration { get; private set; }
        [field: SerializeField] public AbilityConstraint OnChargeConstraints { get; private set; }
        [field: SerializeField] public Ability PerfectChargeAbility { get; private set; }
        [field: SerializeField] public Vector2 PerfectChargeRange { get; private set; } = Vector2.zero;

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            HoldAbilityHolder chargeAbilityHolder = player.ChargeAttack.GetAbilityHolder();

            if(chargeAbilityHolder == null)
            {
                Debug.Log("Haven't equipped the charge ability yet.");
                return;
            }

            if (ChargeDuration != 0)
            {
                float previousHoldDuration = chargeAbilityHolder.HoldDownDuration;
                float newHoldDuration = ChargeDuration + previousHoldDuration;

                SkillProperties chargeDuration = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Hold duration",
                    PreviousValue = $"{previousHoldDuration:0.0}",
                    Value = $"{newHoldDuration:0.0}",
                    UnitString = PropertyUnits.SECONDS
                };

                _properties.Add(chargeDuration);
            }

            if (PerfectChargeRange != Vector2.zero)
            {
                Vector2 previousRange = chargeAbilityHolder.PerfectChargeRange;
                Vector2 newRange = PerfectChargeRange + previousRange;
                float perfectChargeWindow = (previousRange.y - previousRange.x) * 100f;
                float newPerfectChargeWindow = (newRange.y - newRange.x) * 100f;

                SkillProperties chargeDuration = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Critical release zone",
                    PreviousValue = $"{perfectChargeWindow:0}",
                    Value = $"{newPerfectChargeWindow:0}",
                    UnitString = PropertyUnits.PERCENTAGE
                };

                _properties.Add(chargeDuration);
            }

            SaveProperties();
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            _player.ChargeAttack.GetAbilityHolder().UpgradeSkill(this, _player);
            _player.ChargeAttack.ReEquipSkill();
            _player.Upgrades.GetChargeAttackSkillProfile().Upgrades.ForEach(u => u.SetUpgradeProperties(_player));
        }
    }
}
