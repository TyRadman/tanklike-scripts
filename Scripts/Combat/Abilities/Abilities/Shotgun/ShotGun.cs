using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using Combat.SkillTree;
    using Combat;
    using UnitControllers;
    using TankLike.Combat.SkillTree.Upgrades;

    [CreateAssetMenu(fileName = PREFIX + "Shotgun", menuName = ROOT_PATH + "Ability")]
    public class ShotGun : Ability
    {
        [Header("Special Values")]
        [SerializeField] private Clonable<Weapon> _weapon;
        [field : SerializeField] public int ProjectileCount { get; private set; } = 7;
        [field : SerializeField] public float SpreadAngle { get; private set; } = 45f;
        private TankShooter _shooter;

        internal const string ROOT_PATH = Directories.ABILITIES + "Shotgun/";

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            _shooter = components.Shooter;

            _weapon.Initiate();
            _weapon.Instance.SetUp(components);
        }

        public override void PopulateSkillProperties()
        {
            base.PopulateSkillProperties();

            SkillProperties projectileCount = new SkillProperties()
            {
                Name = "Projectiles Count",
                Value = ProjectileCount.ToString(),
                DisplayColor = Colors.Red
            };

            SkillProperties damagePerProjectile = new SkillProperties()
            {
                Name = "Damage per projectile",
                Value = _weapon.GetOriginal().Damage.ToString(),
                DisplayColor = Colors.Red,
                UnitString = PropertyUnits.POINTS
            };

            SkillProperties spreadAngle = new SkillProperties()
            {
                Name = "Spread angle",
                Value = (SpreadAngle).ToString(),
                DisplayColor = Colors.Red,
                UnitString = PropertyUnits.DEGREES
            };

            SkillDisplayProperties.Add(projectileCount);
            SkillDisplayProperties.Add(damagePerProjectile);
            SkillDisplayProperties.Add(spreadAngle);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();

            float startingAngle = -SpreadAngle / 2;
            float angleBetweenShots = SpreadAngle / (ProjectileCount - 1);

            for (int i = 0; i < ProjectileCount; i++)
            {
                _shooter.Shoot(_weapon, startingAngle + angleBetweenShots * i, false);
            }
        }

        public override void Upgrade(SkillUpgrade upgrade)
        {
            base.Upgrade(upgrade);

            if (upgrade is not ShotGunUpgrades newUpgrades)
            {
                Debug.LogError($"Upgrade passed type doesn't match the expected type: {nameof(BrockUpgrades)}");
                return;
            }

            ProjectileCount += newUpgrades.ProjectilesCount;

            SpreadAngle += newUpgrades.SpreadAngle;
            //_angleBetweenShots = newUpgrades.SpreadAngle / (ProjectileCount - 1);
            //_startingAngle = -newUpgrades.SpreadAngle / 2;
        }

        public override float GetDuration()
        {
            return 0f;
        }

        public override void OnAbilityHoldStart()
        {

        }

        public override void OnAbilityHoldUpdate()
        {

        }

        public override void OnAbilityFinished()
        {

        }

        public override void OnAbilityInterrupted()
        {

        }

        public override void SetUpIndicatorSpecialValues(AirIndicator indicator)
        {

        }

        public override void Dispose()
        {

        }
    }
}
