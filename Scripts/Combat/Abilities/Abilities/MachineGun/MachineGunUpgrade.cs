using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.Abilities;
    using UnitControllers;
    using Utils;

    [CreateAssetMenu(fileName = PREFIX + "MachineGun_NAME", menuName = MachineGun.ROOT_PATH + "Upgrade")]
    public class MachineGunUpgrade : AbilityUpgrades
    {
        [field: SerializeField, Header(SPECIAL_VALUES_HEADER), Tooltip("The number of projectiles that will be fired in total. The value is added to the original.")]
        public int ShotsCount { get; private set; }
        private readonly int _originalShotsCount = 0;

        [field: SerializeField, Tooltip("The degree at which the shot projectiles scatter. The value is subtracted from the original.")]
        public float AimingAccuracy { get; private set; }
        private readonly float _originalAimingAccuracy = 0;

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            MachineGun ability = player.ChargeAttack.GetAbilityHolder().Ability as MachineGun;

            if (ability == null)
            {
                Helper.LogWrongAbilityProperty(typeof(MachineGun));
                return;
            }

            if (ShotsCount != _originalShotsCount)
            {
                SkillProperties projectileCount = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Projectiles Count",
                    PreviousValue = ability.ShotsNumber.ToString(),
                    Value = (ShotsCount + ability.ShotsNumber).ToString(),
                    DisplayColor = Colors.Red,
                    UnitString = PropertyUnits.PROJECTILES
                };

                _properties.Add(projectileCount);
            }

            if (AimingAccuracy != _originalAimingAccuracy)
            {
                float newAngles = Mathf.Max(0f, AimingAccuracy + ability.MaxRandomAngle);

                SkillProperties aimingAccuracy = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Aiming Accuracy",
                    PreviousValue = ability.MaxRandomAngle.ToString(),
                    Value = newAngles.ToString(),
                    DisplayColor = Colors.DarkOrange,
                    UnitString = PropertyUnits.DEGREES
                };

                _properties.Add(aimingAccuracy);
            }

            if (_properties.Count > 0)
            {
                UpgradeProperties = GetSkillPropertiesString(_properties);
            }
        }
    }
}
