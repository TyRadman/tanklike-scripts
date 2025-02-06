using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using Combat.SkillTree;
    using Combat.SkillTree.Upgrades;
    using Combat.Abilities;
    using UnitControllers;

    [CreateAssetMenu(fileName = "SH_NAME", menuName = Directories.ABILITIES_HOLDER + "Stat Holder")]
    public class StatsHolder : SkillHolder
    {
        [field: SerializeField] public Skill Stats { get; private set; }

        public override void PopulateSkillProperties()
        {
            Stats.PopulateStatProperties();
        }

        public override List<SkillProperties> GetProperties()
        {
            return Stats.StatProperties;
        }

        public override void ApplySkill(TankComponents components)
        {
            Stats.ApplyStats(components);
        }

        public override void EquipSkill(TankComponents components)
        {

        }

        public override void UpgradeSkill(SkillUpgrade upgrade, PlayerComponents player)
        {

        }
    }
}
