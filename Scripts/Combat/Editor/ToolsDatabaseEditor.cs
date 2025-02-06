using System.Collections;
using System.Collections.Generic;
using TankLike.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace TankLike.Combat.Editor
{
    [CustomEditor(typeof(ToolsDatabase))]
    public class ToolsDatabaseEditor : UnityEditor.Editor
    {
        SerializedProperty _allToolsListProperty;

        private void Awake()
        {
            _allToolsListProperty = serializedObject.FindProperty("_tools");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add all tools"))
            {
                _allToolsListProperty.ClearArray();
                var toolsData = AssetUtils.GetAllInstances<ToolInfo>(true, new string[] { @"Assets/Gameplay/Tools" });

                foreach (var t in toolsData)
                {
                    SerializedPropertiesHelper.AddArrayItem(t, _allToolsListProperty);
                }

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
