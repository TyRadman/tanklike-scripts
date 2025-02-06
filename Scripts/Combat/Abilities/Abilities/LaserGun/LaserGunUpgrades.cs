using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.Abilities;
    using UnitControllers;
    using Utils;

    [CreateAssetMenu(fileName = PREFIX + "LaserGun_NAME", menuName = LaserGun.ROOT_PATH + "Upgrade")]
    public class LaserGunUpgrades : AbilityUpgrades
    {
        [field: SerializeField, Header(SPECIAL_VALUES_HEADER), Tooltip("The duration of the laser ability performance.")]
        public float Duration { get; private set; }
        private readonly int _originalDuration = 0;

        [field: SerializeField] public int Damage { get; private set; }
        private readonly float _originalDamage = 0;

        [field: SerializeField] public float RotationMultiplier { get; private set; }
        private readonly float _originalRotationMultiplier = 0;

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            LaserGun ability = player.SuperAbilities.GetAbilityHolder().Ability as LaserGun;

            if (ability == null)
            {
                Helper.LogWrongAbilityProperty(typeof(Brock));
                return;
            }

            if (Duration != _originalDuration)
            {
                float previousValue = ability.GetLaserDuration();
                float newValue = Duration + previousValue;

                SkillProperties projectileCount = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Laser duration",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.SECONDS
                };

                _properties.Add(projectileCount);
            }

            if (Damage != _originalDamage)
            {
                int previousValue = ability.GetDamage();
                int newValue = Damage + previousValue;

                SkillProperties projectileCount = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Damage",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.POINTS
                };

                _properties.Add(projectileCount);
            }

            if (RotationMultiplier != _originalRotationMultiplier)
            {
                float previousValue = ability.CrosshairSensitivityMultiplier;
                float newValue = RotationMultiplier + previousValue;

                SkillProperties projectileCount = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = (previousValue * 100f).ToString(),
                    Name = "Speed factor",
                    Value = (newValue * 100f).ToString(),
                    UnitString = PropertyUnits.PERCENTAGE
                };

                _properties.Add(projectileCount);
            }

            SaveProperties();
        }
    }
}
