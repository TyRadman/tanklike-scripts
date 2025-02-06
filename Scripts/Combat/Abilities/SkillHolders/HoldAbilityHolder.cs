using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using UnitControllers;
    using Combat.SkillTree;
    using Combat.SkillTree.Upgrades;

    [CreateAssetMenu(fileName = "HA_NAME", menuName = Directories.ABILITIES_HOLDER + "Hold Ability Holder")]
    public class HoldAbilityHolder : SkillHolder
    {
        [field: SerializeField] public Ability Ability { get; private set; }
        [field: SerializeField] public float HoldDownDuration { set; get; } = 1f;
        [field: SerializeField] public AbilityConstraint OnHoldConstraints { get; private set; }
        [field: SerializeField] public AbilityConstraint OnPerformConstraints { get; private set; }
        [field: SerializeField, Header("Perfect Charge")] public Ability PerfectChargeAbility { get; private set; }
        [field: SerializeField] public Vector2 PerfectChargeRange { get; internal set; }

        private const float MIN_CHARGE_DURATION = 0.1f;

        public override void PopulateSkillProperties()
        {
            Ability.SkillDisplayProperties.Clear();

            AddSkillProperty(Ability.SkillDisplayProperties, "Charge duration", HoldDownDuration, Colors.DarkCyan, PropertyUnits.SECONDS);

            Ability.PopulateSkillProperties();
        }

        public override List<SkillProperties> GetProperties()
        {
            return Ability.SkillDisplayProperties;
        }

        public override void ApplySkill(TankComponents components)
        {
            PlayerComponents playerComponents = components as PlayerComponents;

            playerComponents.ChargeAttack.AddSkill(this);
        }

        public override void EquipSkill(TankComponents components)
        {
            base.Equals(components);

            if(components is PlayerComponents playerComponents)
            {
                playerComponents.ChargeAttack.EquipSkill(this);
            }
        }

        public void SetAbility(Ability ability)
        {
            Ability = ability;
        }

        public override void UpgradeSkill(SkillUpgrade upgrade, PlayerComponents player)
        {
            if(upgrade == null)
            {
                Debug.LogError("No upgrade passed.");
                return;
            }

            if(upgrade is ChargeAbilityHolderUpgrade chargeUpgrade)
            {
                HoldDownDuration = Mathf.Max(chargeUpgrade.ChargeDuration + HoldDownDuration, MIN_CHARGE_DURATION);
                
                if (chargeUpgrade.OnChargeConstraints != AbilityConstraint.None)
                {
                    OnPerformConstraints = chargeUpgrade.OnChargeConstraints;
                }

                if (chargeUpgrade.PerfectChargeAbility != null)
                {
                    PerfectChargeAbility = chargeUpgrade.PerfectChargeAbility;
                    player.ChargeAttack.AddPerfectChargeAbility(PerfectChargeAbility);
                }

                if (chargeUpgrade.PerfectChargeRange != Vector2.zero)
                {
                    PerfectChargeRange += chargeUpgrade.PerfectChargeRange;
                    player.ChargeAttack.SetPerfectChargeValues();
                }

                return;
            }

            if (upgrade is not AbilityUpgrades abilityUpgrade)
            {
                Debug.LogError("No upgrade passed.");
                return;
            }

            Ability.Upgrade(abilityUpgrade);
        }

        public void SetPerfectChargeAbility(Ability ability)
        {
            PerfectChargeAbility = ability;
        }
    }
}
