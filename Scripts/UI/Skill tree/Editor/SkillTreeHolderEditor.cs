using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike
{
    using Combat.SkillTree;

    [CustomEditor(typeof(SkillTreeHolder))]
    public class SkillTreeHolderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SkillTreeHolder skillTree = (SkillTreeHolder)target;

            if(GUILayout.Button("Generate Lines"))
            {
                skillTree.GenerateConnectionLines();
            }
            
            if(GUILayout.Button("Generate Branches"))
            {
                skillTree.BuildBranches();
            }

            if (GUILayout.Button("Clear Lines"))
            {
                skillTree.ClearConnectionLines();
            }
        }
    }
}
