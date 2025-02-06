using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree
{
    // TODO: explore removing it
    [CreateAssetMenu(fileName = "SkillTreeData_Player_NAME", menuName = Directories.SKILL_TREE + "Skill Tree Prefab")]
    public class SkillTreePrefab : ScriptableObject
    {
        [field: SerializeField] public SkillTreeHolder SkillTree { get; private set; }
    }
}
