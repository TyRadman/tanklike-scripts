using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using UnitControllers;
    using UnitControllers.Shields;
    using Combat.SkillTree;
    using TankLike.Combat.SkillTree.Upgrades;

    [CreateAssetMenu(fileName = PREFIX + "Shield", menuName = ROOT_PATH + "Ability")]
    public class ShieldAbility : Ability
    {
        public const string ROOT_PATH = Directories.ABILITIES + "Shield Ability/";

        [field: SerializeField, Header("Special Values")] public float ShieldActivationDuration { get; private set; } = 5f;
        [SerializeField] private Shield _shieldPrefab;
        
        private Shield _shield;
        private WaitForSeconds _shieldActivationWait;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            if (_shieldPrefab == null)
            {
                Debug.LogError($"No shield prefab at {this.name} ability");
                return;
            }

            _shield = Instantiate(_shieldPrefab, _components.transform);

            _shield.SetUp(_components);

            _shield.transform.localPosition = Vector3.zero;

            _shield.SetShieldUser(components.Alignment);
            _shield.SetSize(_components.AdditionalInfo.ShieldScale);

            _duration = ShieldActivationDuration - Shield.FADING_DURATION;
            _shieldActivationWait = new WaitForSeconds(_duration);
        }

        public override void PopulateSkillProperties()
        {
            base.PopulateSkillProperties();

            SkillProperties duration = new SkillProperties()
            {
                Name = "Duration",
                Value = ShieldActivationDuration.ToString(),
                DisplayColor = Colors.Green,
                UnitString = PropertyUnits.SECONDS
            };

            SkillDisplayProperties.Add(duration);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();

            _components.StartCoroutine(ShieldActivationRoutine());
        }

        private IEnumerator ShieldActivationRoutine()
        {
            _shield.ActivateShield();

            yield return _shieldActivationWait;

            _shield.DeactivateShield();
        }

        public override void SetUpIndicatorSpecialValues(AirIndicator indicator)
        {
            if (indicator is not AirIndicator)
            {
                Debug.LogError($"Ability {this.name} is expecting indicator of type {nameof(AirIndicator)}. Wrong indicator.");
                return;
            }

            IndicatorSettings settings = new IndicatorSettings()
            {
                AimRange = Vector2.one,
                EndSize = _components.AdditionalInfo.ShieldScale,
                AimSpeedMultiplier = 0,
                AvoidWalls = false,
                LineHeight = 0,
                IsCrosshairTheParent = false,
                LineState = AirIndicator.IndicatorLineState.None
            };

            indicator.SetValues(settings);
        }

        public Shield GetShield()
        {
            return _shield;
        }

        public override float GetDuration()
        {
            return _duration;
        }

        #region Upgrades
        public override void Upgrade(SkillUpgrade upgrade)
        {
            base.Upgrade(upgrade);

            if(upgrade is not ShieldAbilityUpgrades shieldUpgrades)
            {
                Debug.LogError("No upgrade passed.");
                return;
            }

            ShieldActivationDuration += shieldUpgrades.Duration;
            _duration = ShieldActivationDuration - Shield.FADING_DURATION;
            _shieldActivationWait = new WaitForSeconds(_duration);

            UpgradeDeflectedProjectilesCount(shieldUpgrades);
        }

        private void UpgradeDeflectedProjectilesCount(ShieldAbilityUpgrades upgrades)
        {
            if(upgrades.ProjectilesDeflectedPerProjectile == 0)
            {
                return;
            }

            SIMProjectileDeflector projectileDeflector = _shield.GetShieldOnImpactModule<SIMProjectileDeflector>();

            if (projectileDeflector != null)
            {
                if (projectileDeflector.GetProjectileWeapon() is NormalShot weapon)
                {
                    weapon.ShotsNumber += upgrades.ProjectilesDeflectedPerProjectile;
                }
            }
        }

        #region Upgrade values getters
        private Weapon GetProjectilesDeflectorWeapon()
        {
            SIMProjectileDeflector projectileDeflector = _shield.GetShieldOnImpactModule<SIMProjectileDeflector>();

            if (projectileDeflector != null)
            {
                if (projectileDeflector.GetProjectileWeapon() is NormalShot weapon)
                {
                    return weapon;
                }
            }

            Debug.LogError("No projectile deflector found on the shield.");
            return null;
        }

        public int GetDeflectedProjectilesCount()
        {
            if(GetProjectilesDeflectorWeapon() is NormalShot weapon)
            {
                return weapon.ShotsNumber;
            }

            return 0;
        }
        #endregion
            #endregion

        public override void OnAbilityFinished()
        {

        }

        public override void OnAbilityHoldStart()
        {

        }

        public override void OnAbilityHoldUpdate()
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
