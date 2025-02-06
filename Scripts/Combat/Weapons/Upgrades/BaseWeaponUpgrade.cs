using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using TankLike.Combat;
    using UnitControllers;

    [CreateAssetMenu(fileName = PREFIX + "BrockUpgrade_NAME", menuName = NormalShot.ROOT_PATH + "Upgrade")]
    public class BaseWeaponUpgrade : SkillUpgrade
    {
        [field: SerializeField, Header(SPECIAL_VALUES_HEADER), Tooltip("The cool down time between shots.")]
        public float CoolDownTime { get; private set; }
        private readonly float _originalCoolDownTime = 0;

        [field: SerializeField, Tooltip("The maximum number of bullets the player can have.")]
        public int Ammo { get; private set; }
        private readonly int _originalAmmo = 0;

        [field: SerializeField, Tooltip("The damage a single projectile deals.")]
        public int Damage { get; private set; }
        private readonly int _originalDamage = 0;

        [field: SerializeField, Tooltip("The damage a single projectile deals.")]
        public float ProjectileSpeed { get; private set; } = 0f;
        private readonly float _originalProjectileSpeed = 0f;

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
                    PreviousValue = $"{previousValue:0.0}",
                    Value = $"{newValue:0.0}",
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

            if (Damage != _originalDamage)
            {
                int previousValue = player.Shooter.GetWeaponHolder().Weapon.Damage;
                int newValue = Damage + previousValue;

                SkillProperties damage = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Damage",
                    PreviousValue = previousValue.ToString(),
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.POINTS
                };

                _properties.Add(damage);
            }

            if (ProjectileSpeed != _originalProjectileSpeed)
            {
                NormalShot normalShotWeapon = player.Shooter.GetWeaponHolder().Weapon as NormalShot;

                if(normalShotWeapon == null)
                {
                    Debug.LogError("The weapon is not a normal shot weapon.");
                    return;
                }

                float previousValue = normalShotWeapon.BulletSpeed;
                float newValue = ProjectileSpeed + previousValue;

                SkillProperties speed = new SkillProperties()
                {
                    IsComparisonValue = true,
                    Name = "Projectile Speed",
                    PreviousValue = $"{previousValue:0}",
                    Value = $"{newValue:0}",
                    UnitString = ""
                };

                _properties.Add(speed);
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
