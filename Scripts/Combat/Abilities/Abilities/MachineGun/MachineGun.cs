using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using Combat.SkillTree;
    using TankLike.Combat.SkillTree.Upgrades;

    [CreateAssetMenu(fileName = PREFIX + "MachineGun", menuName = ROOT_PATH + "Ability")]
    public class MachineGun : Ability
    {
        public const string ROOT_PATH = Directories.ABILITIES + "Machine Gun/";

        [field: SerializeField, Header("Special Values")] public int ShotsNumber { get; private set; } = 7;
        [SerializeField] private float _shootingDuration = 3f;
        [SerializeField] private float _crosshairSpeedMultiplier = 0.5f;
        [field: SerializeField, Range(0f, 90f)] public float MaxRandomAngle { get; private set; } = 5f;
        [SerializeField] private Clonable<Weapon> _weapon;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            _weapon.Initiate();
            _weapon.Instance.SetUp(components);
        }

        public override void PopulateSkillProperties()
        {
            base.PopulateSkillProperties();

            SkillProperties projectileCount = new SkillProperties()
            {
                Name = "Projectiles Count",
                Value = ShotsNumber.ToString(),
                DisplayColor = Colors.Red
            };

            SkillProperties damagePerProjectile = new SkillProperties()
            {
                Name = "Damage per projectile",
                Value = _weapon.GetOriginal().Damage.ToString(),
                DisplayColor = Colors.Red,
                UnitString = PropertyUnits.POINTS
            };

            SkillDisplayProperties.Add(projectileCount);
            SkillDisplayProperties.Add(damagePerProjectile);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();
            _components.StartCoroutine(ShootingProcess());
        }

        private IEnumerator ShootingProcess()
        {
            // TODO: should be move to the player's ability performer instead
            ((PlayerComponents)_components).CrosshairController.SetCrosshairSpeedMultiplier(_crosshairSpeedMultiplier);

            float shotDuration = _shootingDuration / ShotsNumber;
            WaitForSeconds wait = new WaitForSeconds(shotDuration);
            float halfAngle = MaxRandomAngle / 2;

            for (int i = 0; i < ShotsNumber; i++)
            {
                float angle = Random.Range(-halfAngle, halfAngle);
                _components.Shooter.Shoot(_weapon, angle, false, false);
                yield return wait;
            }

            // TODO: should be move to the player's ability performer instead
            ((PlayerComponents)_components).CrosshairController.ResetCrosshairSpeedMultiplier();
        }

        public override void Upgrade(SkillUpgrade upgrade)
        {
            base.Upgrade(upgrade);

            if(upgrade is not MachineGunUpgrade machineGunUpgrade)
            {
                return;
            }

            ShotsNumber += machineGunUpgrade.ShotsCount;
            MaxRandomAngle += machineGunUpgrade.AimingAccuracy;
        }

        public override float GetDuration()
        {
            return _shootingDuration;
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
