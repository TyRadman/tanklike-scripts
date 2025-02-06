using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.Abilities;
    using UnitControllers;
    using UnitControllers.Shields;
    using Utils;

    [CreateAssetMenu(fileName = PREFIX + "ShieldUpgrade_NAME", menuName = ShieldAbility.ROOT_PATH + "Upgrade")]
    public class ShieldAbilityUpgrades : AbilityUpgrades
    {
        [field: SerializeField, Header(SPECIAL_VALUES_HEADER), Tooltip("The duration of the shield activation.")]
        public float Duration { get; private set; }
        private readonly float _originalDuration = 0f;

        [field: SerializeField, Tooltip("The area that the landing projectiles will cover.")]
        public int ProjectilesDeflectedPerProjectile { get; private set; }
        private readonly int _originalProjectilesDeflectedPerProjectile = 0;

        public override void SetUpgradeProperties(PlayerComponents player)
        { 
            base.SetUpgradeProperties(player);

            ShieldAbility ability = player.SuperAbilities.GetAbilityHolder().Ability as ShieldAbility;

            if (ability == null)
            {
                Helper.LogWrongAbilityProperty(typeof(ShieldAbility));
                return;
            }

            Shield shield = ability.GetShield();

            if (Duration != _originalDuration)
            {
                float previousValue = ability.ShieldActivationDuration;
                float newValue = Duration + previousValue;

                SkillProperties damage = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Shield Duration",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.SECONDS
                };

                _properties.Add(damage);
            }

            if (ProjectilesDeflectedPerProjectile != _originalProjectilesDeflectedPerProjectile)
            {
                int previousValue = ability.GetDeflectedProjectilesCount();
                int newValue = ProjectilesDeflectedPerProjectile + previousValue;

                SkillProperties damage = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Projectiles deflected count",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.PROJECTILES
                };

                _properties.Add(damage);
            }

            SaveProperties();
        }
    }
}
