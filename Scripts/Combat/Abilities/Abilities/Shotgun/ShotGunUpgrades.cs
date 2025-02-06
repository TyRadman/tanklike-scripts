using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.Abilities;
    using UnitControllers;
    using Utils;

    [CreateAssetMenu(fileName = PREFIX + "ShotGunUpgrade_NAME", menuName = ShotGun.ROOT_PATH + "Upgrade")]
    public class ShotGunUpgrades : AbilityUpgrades
    {
        [field: SerializeField, Header(SPECIAL_VALUES_HEADER), Tooltip("The number of projectiles that will be fired in total.")]
        public int ProjectilesCount { get; private set; }
        private readonly int _originalProjectilesCount = 0;

        [field: SerializeField, Tooltip("The angle at which the projectiles will travel.")]
        public int SpreadAngle { get; private set; }
        private readonly int _originalSpreadAngle = 0;

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            ShotGun ability = player.ChargeAttack.GetAbilityHolder().Ability as ShotGun;

            if (ability == null)
            {
                Helper.LogWrongAbilityProperty(typeof(ShotGun));
                return;
            }

            if (ProjectilesCount != _originalProjectilesCount)
            {
                int previousValue = ability.ProjectileCount;
                int newValue = ProjectilesCount + previousValue;

                SkillProperties projectileCount = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Projectiles Count",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.PROJECTILES
                };

                _properties.Add(projectileCount);
            }

            if (SpreadAngle != _originalSpreadAngle)
            {
                float previousValue = ability.SpreadAngle;
                float newValue = SpreadAngle + previousValue;

                SkillProperties projectileCount = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Spread angle",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.DEGREES
                };

                _properties.Add(projectileCount);
            }

            SaveProperties();
        }
    }
}
