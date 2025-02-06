using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike.Combat.SkillTree
{
    using Utils;

    /// <summary>
    /// Holds a list of skill profiles for the skill tree to use.
    /// </summary>
    [CreateAssetMenu(fileName = "SkillsDatabase_", menuName = Directories.SKILL_TREE + "/Skills Database")]
    public class SkillTreeSkillsDatabase : ScriptableObject
    {
        public List<SkillProfile> Skills;
        public List<SkillProfile> CollectedSkills { get; private set; } = new List<SkillProfile>();

        public SkillProfile GetRandomSkill()
        {
            return Skills.FindAll(s => !CollectedSkills.Contains(s)).RandomItem();
        }

        public SkillProfile GetRandomSkill(SkillProfile skillToExcept)
        {
            return Skills.FindAll(s => !CollectedSkills.Contains(s) && s != skillToExcept).RandomItem();
        }

        public void AddCollectedSkill(SkillProfile skillProfile)
        {
            CollectedSkills.Add(skillProfile);
        }

        public void ClearCollectedList()
        {
            CollectedSkills = new List<SkillProfile>();
        }

        public SkillProfile GetLastSkill()
        {
            return Skills.Find(s => !CollectedSkills.Contains(s));
        }

        public int GetAvailableSkillsCount()
        {
            return Skills.FindAll(s => !CollectedSkills.Contains(s)).Count;
        }
    }
}
