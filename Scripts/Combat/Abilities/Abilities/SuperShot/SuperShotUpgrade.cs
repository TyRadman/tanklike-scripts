using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.Abilities;
    using UnitControllers;
    using Utils;

    [CreateAssetMenu(fileName = PREFIX + "SuperShot_NAME", menuName = SuperShot.ROOT_PATH + "Upgrade")]

    public class SuperShotUpgrade : AbilityUpgrades
    {
        [field: SerializeField, Header(SPECIAL_VALUES_HEADER), Tooltip("The damage the super shot deals. The value is added to the original.")]
        public int Damage { get; private set; }
        private readonly int _originalDamage = 0;

        [field: SerializeField, Tooltip("The impact of the super shot projectile (single target or AOE).")]
        public OnImpact Impact { get; private set; }

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            SuperShot ability = player.ChargeAttack.GetAbilityHolder().Ability as SuperShot;

            if (ability == null)
            {
                Helper.LogWrongAbilityProperty(typeof(SuperShot));
                return;
            }

            if (Damage != _originalDamage)
            {
                SkillProperties damage = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Damage",
                    PreviousValue = ability.GetWeapon().Damage.ToString(),
                    Value = (Damage + ability.GetWeapon().Damage).ToString(),
                    DisplayColor = Colors.Red,
                    UnitString = PropertyUnits.POINTS
                };

                _properties.Add(damage);
            }

            if(Impact != null)
            {
                SkillProperties impact = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Impact type",
                    PreviousValue = "Single Target",
                    Value = "AOE",
                    DisplayColor = Colors.DarkOrange,
                    UnitString = ""
                };

                _properties.Add(impact);
            }

            SaveProperties();
        }
    }
}
