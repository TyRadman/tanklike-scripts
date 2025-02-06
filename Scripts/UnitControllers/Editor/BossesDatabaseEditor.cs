using System.Collections;
using System.Collections.Generic;
using TankLike.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace TankLike.UnitControllers.Editor
{
    [CustomEditor(typeof(BossesDatabase))]
    public class BossesDatabaseEditor : UnityEditor.Editor
    {
        SerializedProperty _allBossesListProperty;

        private void Awake()
        {
            _allBossesListProperty = serializedObject.FindProperty("_bosses");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add all bosses"))
            {
                _allBossesListProperty.ClearArray();
                var BossesData = AssetUtils.GetAllInstances<BossData>(true, new string[] { @"Assets/Gameplay/Bosses" });

                foreach (var b in BossesData)
                {
                    SerializedPropertiesHelper.AddArrayItem(b, _allBossesListProperty);
                }
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
