using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using UnitControllers;
    using Combat.SkillTree;

    public abstract class Skill : ScriptableObject
    {
        [SerializeField] private Sprite _icon;

        public List<SkillProperties> StatProperties { get; protected set; } = new List<SkillProperties>();

        protected const string FILE_NAME_PREFIX = "Stat_";
        protected const string MENU_ROOT = Directories.SKILLS + "Stats/";

        public abstract void ApplyStats(TankComponents components);

        public abstract void PopulateStatProperties();
    }
}
