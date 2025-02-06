using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.Abilities;
    using System.Text;
    using UnitControllers;
    using Utils;
    using static TankLike.Combat.Abilities.ExplosiveTeleportationAbility;

    [CreateAssetMenu(fileName = PREFIX + "ExplosiveTeleportationUpgrade_NAME", menuName = ExplosiveTeleportationAbility.ROOT_PATH + "Upgrade")]
    public class ExplosiveTeleportationAbilityUpgrades : AbilityUpgrades
    {
        /// <summary>
        /// The distance the player will teleport to.
        /// </summary>
        [field: SerializeField, Header(SPECIAL_VALUES_HEADER), Tooltip("The distance the player will teleport to.")]
        public float TeleporationDistance { get; private set; }
        private readonly float _originalTeleporationDistance = 0f;

        [field: SerializeField, Tooltip("The area that the landing projectiles will cover.")]
        public TeleportationExplosionMode ExplosionMode { get; private set; } = TeleportationExplosionMode.None;
        private readonly TeleportationExplosionMode _originalExplosionMode = TeleportationExplosionMode.None;

        private Dictionary<TeleportationExplosionMode, string> _explosionModeDescriptions = new Dictionary<TeleportationExplosionMode, string>()
        {
            { TeleportationExplosionMode.OnStart, "start" },
            { TeleportationExplosionMode.OnLand, "end" },
            { TeleportationExplosionMode.AlongWay, "along the way" },
        };

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            ExplosiveTeleportationAbility ability = player.SuperAbilities.GetAbilityHolder().Ability as ExplosiveTeleportationAbility;

            if (ability == null)
            {
                Helper.LogWrongAbilityProperty(typeof(ExplosiveTeleportationAbility));
                return;
            }

            if (TeleporationDistance != _originalTeleporationDistance)
            {
                float previousValue = ability.TeleportDistance;
                float newValue = TeleporationDistance + previousValue;

                SkillProperties damage = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue.ToString(),
                    Name = "Teleportation distance",
                    Value = newValue.ToString(),
                    UnitString = PropertyUnits.TILES
                };

                _properties.Add(damage);
            }

            if (ExplosionMode != _originalExplosionMode)
            {
                string previousValue = GetExplosionModeDescription(ability.ExplosionMode);

                string newValue = GetExplosionModeDescription(ability.ExplosionMode | ExplosionMode);

                SkillProperties damage = new SkillProperties()
                {
                    IsComparisonValue = true,
                    PreviousValue = previousValue,
                    Name = "Explosion Mode",
                    Value = newValue,
                    UnitString = PropertyUnits.PROJECTILES
                };

                _properties.Add(damage);
            }

            SaveProperties();
        }

        private string GetExplosionModeDescription(TeleportationExplosionMode mode)
        {
            StringBuilder description = new StringBuilder("Explosions are performed on ");

            List<string> modifiers = new List<string>();

            if (mode.HasFlag(TeleportationExplosionMode.OnStart))
            {
                modifiers.Add(_explosionModeDescriptions[TeleportationExplosionMode.OnStart]);
            }

            if (mode.HasFlag(TeleportationExplosionMode.OnLand))
            {
                modifiers.Add(_explosionModeDescriptions[TeleportationExplosionMode.OnLand]);
            }

            if (mode.HasFlag(TeleportationExplosionMode.AlongWay))
            {
                modifiers.Add(_explosionModeDescriptions[TeleportationExplosionMode.AlongWay]);
            }

            if (modifiers.Count == 1)
            {
                description.Append(modifiers[0]);
            }
            else if (modifiers.Count > 1)
            {
                for (int i = 0; i < modifiers.Count; i++)
                {
                    description.Append(modifiers[i]);
                    
                    if(i < modifiers.Count - 2)
                    {
                        description.Append(", ");
                    }

                    if (i == modifiers.Count - 2)
                    {
                        description.Append(" and ");
                    }
                }
            }

            return description.ToString();
        }
    }
}
