using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using TankLike.Combat;
    using UnitControllers;

    [CreateAssetMenu(fileName = PREFIX + "SplitShot_NAME", menuName = Directories.ABILITIES + "Normal Shot/Upgrade")]
    public class SplitShotUpgrades : SkillUpgrade
    {
        [field: SerializeField, Header(SPECIAL_VALUES_HEADER), Tooltip("The cool down time between shots.")]
        public float CoolDownTime { get; private set; }
        private readonly float _originalCoolDownTime = 0;

        [field: SerializeField, Tooltip("The maximum number of bullets the player can have.")]
        public int Ammo { get; private set; }
        private readonly int _originalAmmo = 0;

        public override void SetUp(PlayerComponents player)
        {
            base.SetUp(player);

            SetUpgradeProperties(player);
        }

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            if (CoolDownTime != _originalCoolDownTime)
            {
                float previousValue = player.Shooter.GetWeaponHolder().Weapon.CoolDownTime;
                float newValue = CoolDownTime + previousValue;

                SkillProperties coolDownTime = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Cooldown time",
                    PreviousValue = previousValue.ToString(),
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.SECONDS
                };

                _properties.Add(coolDownTime);
            }

            if (Ammo != _originalAmmo)
            {
                int previousValue = player.Shooter.GetWeaponHolder().Weapon.AmmoCapacity;
                int newValue = Ammo + previousValue;

                SkillProperties ammo = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Ammo",
                    PreviousValue = previousValue.ToString(),
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.PROJECTILES
                };

                _properties.Add(ammo);
            }

            SaveProperties();
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();

            _player.Shooter.GetWeaponHolder().UpgradeSkill(this, _player);
            _player.Shooter.ReEquipSkill();
            _player.Upgrades.GetBaseWeaponSkillProfile().Upgrades.ForEach(u => u.SetUpgradeProperties(_player));
        }
    }
}
