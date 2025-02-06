using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using UnitControllers;
    using Combat.SkillTree;
    using TankLike.Combat.SkillTree.Upgrades;

    [CreateAssetMenu(fileName = PREFIX + "LaserGun", menuName = ROOT_PATH + "Ability")]
    public class LaserGun : Ability
    {
        [SerializeField] private Clonable<LaserWeapon> _weapon;
        [SerializeField] private Vector2 _indicatorRange;
        [SerializeField] private float _onAimCrosshairSensitivityMultiplier = 0.5f;
        [field : SerializeField] public float CrosshairSensitivityMultiplier { get; private set; } = 0.0f;

        [field : SerializeField] public float Duration { get; private set; }

        public const string ROOT_PATH = Directories.ABILITIES + "Laser Gun/";

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            if (components == null)
            {
                Debug.LogError($"No tank components passed to {name}.");
                return;
            }

            _components = components;

            _weapon.Initiate();
            _weapon.Instance.SetUp(components);

            _weapon.Instance.SetDuration(Duration);

            if (_components == null)
            {
                Debug.LogError($"{components.gameObject.name} doesn't have a {_components.GetType()} component.");
                return;
            }
        }

        public override void PopulateSkillProperties()
        {
            base.PopulateSkillProperties();

            SkillProperties damage = new SkillProperties()
            {
                Name = "Damage per second",
                Value = _weapon.GetOriginal().Damage.ToString(),
                DisplayColor = Color.red,
                UnitString = PropertyUnits.POINTS
            };

            SkillDisplayProperties.Add(damage);

            SkillProperties duration = new SkillProperties()
            {
                Name = "Duration",
                Value = _weapon.GetOriginal().GetLaserDuration().ToString(),
                DisplayColor = Colors.Green,
                UnitString = PropertyUnits.SECONDS
            };

            SkillDisplayProperties.Add(duration);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();
            _weapon.Instance.OnShot();

            if (_components is PlayerComponents playerComponents)
            {
                playerComponents.CrosshairController.SetCrosshairSpeedMultiplier(CrosshairSensitivityMultiplier);
                _components.StartCoroutine(ResetCrosshairSensitivityMultiplier());
            }
        }

        private IEnumerator ResetCrosshairSensitivityMultiplier()
        {
            yield return new WaitForSeconds(base.GetDuration());

            (_components as PlayerComponents).CrosshairController.ResetCrosshairSpeedMultiplier();
        }

        public override void SetUpIndicatorSpecialValues(AirIndicator indicator)
        {
            if(indicator == null)
            {
                Debug.LogError($"No indicator passed.");
                return;
            }

            IndicatorSettings settings = new IndicatorSettings()
            {
                AimRange = _indicatorRange,
                AimSpeedMultiplier = _onAimCrosshairSensitivityMultiplier,
                AvoidWalls = false,
                EndSize = 0,
                LineState = AirIndicator.IndicatorLineState.GroundTrajectory,
                IsCrosshairTheParent = false,
            };

            indicator.SetValues(settings);
        }

        public override void Upgrade(SkillUpgrade upgrade)
        {
            base.Upgrade(upgrade);

            if (upgrade is not LaserGunUpgrades newUpgrades)
            {
                Debug.LogError($"Upgrade passed type doesn't match the expected type: {nameof(LaserGunUpgrades)}");
                return;
            }

            Duration += newUpgrades.Duration;
            _weapon.Instance.SetDuration(Duration);

            _weapon.Instance.SetDamage(_weapon.Instance.Damage + newUpgrades.Damage);

            CrosshairSensitivityMultiplier += newUpgrades.RotationMultiplier;
        }

        public float GetLaserDuration()
        {
            return Duration;
        }

        public int GetDamage()
        {
            return _weapon.Instance.Damage;
        }

        public override float GetDuration()
        {
            return _weapon.Instance.GetLaserDuration();
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

        public override void Dispose()
        {

        }
    }
}
