using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using Combat.SkillTree;
    using Combat.SkillTree.Upgrades;
    using UnitControllers;

    [CreateAssetMenu(fileName = "WH_NAME", menuName = Directories.ABILITIES_HOLDER + "Weapon Holder")]
    public class WeaponHolder : SkillHolder
    {
        [field: SerializeField] public Weapon Weapon { get; private set; }

        public List<SkillProperties> WeaponProperties { get; private set; } = new List<SkillProperties>();

        public override void PopulateSkillProperties()
        {
            WeaponProperties.Clear();

            SkillProperties ammo = new SkillProperties()
            {
                Name = "Ammo",
                Value = Weapon.AmmoCapacity.ToString(),
                DisplayColor = Colors.Gray,
                UnitString = PropertyUnits.SHOTS
            };

            SkillProperties damage = new SkillProperties()
            {
                Name = "Damage per projectile",
                Value = Weapon.Damage.ToString(),
                DisplayColor = Colors.Red,
                UnitString = PropertyUnits.POINTS
            };

            SkillProperties cooldown = new SkillProperties()
            {
                Name = "Cool down time",
                Value = Weapon.CoolDownTime.ToString(),
                DisplayColor = Colors.DarkOrange,
                UnitString = PropertyUnits.SECONDS
            };

            WeaponProperties.Add(ammo);
            WeaponProperties.Add(damage);
            WeaponProperties.Add(cooldown);
        }

        public override List<SkillProperties> GetProperties()
        {
            return WeaponProperties;
        }

        public override void ApplySkill(TankComponents components)
        {
            if(components.Shooter is not TankShooter shooter)
            {
                Debug.LogError("No upgradable shooter found in the components.");
                return;
            }

            shooter.AddSkill(this);
        }

        public override void EquipSkill(TankComponents components)
        {
            if (components.Shooter is not TankShooter shooter)
            {
                Debug.LogError("No upgradable shooter found in the components.");
                return;
            }

            shooter.EquipSkill(this);
        }

        public void SetWeapon(Weapon weapon)
        {
            Weapon = weapon;
        }

        public override void UpgradeSkill(SkillUpgrade upgrade, PlayerComponents player)
        {
            if (upgrade == null || upgrade is not BaseWeaponUpgrade weaponUpgrade)
            {
                Debug.LogError("No upgrade passed.");
                return;
            }

            if (player.Shooter is not TankShooter shooter)
            {
                Debug.LogError("No upgradable shooter found in the components.");
                return;
            }

            shooter.UpgradeSkill(weaponUpgrade);
            Weapon.Upgrade(weaponUpgrade);
        }
    }
}
