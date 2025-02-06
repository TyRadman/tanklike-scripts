using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using Combat;
    using TankLike.Combat.SkillTree.Upgrades;
    using UnitControllers;

    [CreateAssetMenu(fileName = PREFIX + "CustomShot", menuName = Directories.ABILITIES + "Custom shot")]
    public class CustomProjectile : Ability
    {
        [SerializeField] private float _animationDelay = 0.15f;
        [Header("Special Values")]
        [SerializeField] private Weapon _weapon;
        [SerializeField] private Vector2 _indicatorRange;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _components = components;
            _weapon.SetUp(_components);
        }

        public override void SetUpIndicatorSpecialValues(AirIndicator indicator)
        {
            if (indicator == null)
            {
                Debug.LogError($"No indicator passed to ability {name}");
                Debug.Break();
                return;
            }

            IndicatorSettings settings = new IndicatorSettings()
            {
                AimRange = _indicatorRange,
                AimSpeedMultiplier = 1f,
                AvoidWalls = false,
                IsCrosshairTheParent = false,
                LineState = AirIndicator.IndicatorLineState.GroundTrajectory
            };

            indicator.SetValues(settings);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();

            _components.Animation.PlayShootAnimation();
            _components.StartCoroutine(ShootCustomProjectile());
        }

        private IEnumerator ShootCustomProjectile()
        {
            yield return new WaitForSeconds(_animationDelay);
            _weapon.OnShot();
        }

        public override void Upgrade(SkillUpgrade upgrade)
        {
            base.Upgrade(upgrade);


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
