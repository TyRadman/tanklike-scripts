using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.Abilities;
    using UnitControllers;
    using Utils;

    [CreateAssetMenu(fileName = PREFIX + "BrockUpgrade_NAME", menuName = Brock.ROOT_PATH + "Upgrade")]
    public class BrockUpgrades : AbilityUpgrades
    {
        [field : SerializeField, Header(SPECIAL_VALUES_HEADER), Tooltip("The number of projectiles that will be fired in total.")] 
        public int ShotsCount { get; private set; }
        private readonly int _originalShotsCount = 0;

        [field: SerializeField, Tooltip("The area that the landing projectiles will cover.")] 
        public float CoverageArea { get; private set; }
        private readonly float _originalCoverageArea = 0;

        [field: SerializeField, Tooltip("The distance from which the player can perform the ability.")]
        public float Distance { get; private set; }
        private readonly float _originalDistance = 0;

        [field : SerializeField] public int DamagePerProjectile { get; private set; }
        private readonly float _originalDamagePerProjectile = 0;

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            Brock ability = player.SuperAbilities.GetAbilityHolder().Ability as Brock;

            if (ability == null)
            {
                Helper.LogWrongAbilityProperty(typeof(Brock));
                return;
            }

            if (ShotsCount != _originalShotsCount)
            {
                int previousValue = ability.ProjectilesCount;
                int newValue = ShotsCount + previousValue;

                SkillProperties projectileCount = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Projectiles Count",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.PROJECTILES
                };

                _properties.Add(projectileCount);
            }

            if (CoverageArea != _originalCoverageArea)
            {
                float previousValue = ability.Radius;
                float newValue = CoverageArea + previousValue;

                SkillProperties coverageArea = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Coverage Area",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.TILES
                };

                _properties.Add(coverageArea);
            }

            if (Distance != _originalDistance)
            {
                float previousValue = ability.AimRange.y;
                float newValue = Distance + previousValue;

                SkillProperties distance = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Distance",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.TILES
                };

                _properties.Add(distance);
            }

            if (DamagePerProjectile != _originalDamagePerProjectile)
            {
                float previousValue = ability.DamagePerBullet;
                float newValue = DamagePerProjectile + previousValue;

                SkillProperties damage = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Damage per Projectile",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.POINTS
                };

                _properties.Add(damage);
            }

            SaveProperties();
        }
    }
}
