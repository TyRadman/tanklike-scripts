using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankLike.Utils.Editor;

namespace TankLike.UnitControllers.Editor
{
    [CustomEditor(typeof(EnemiesDatabase))]
    public class EnemiesDatabaseEditor : UnityEditor.Editor
    {
        SerializedProperty _allEnemiesListProperty;

        private void Awake()
        {
            _allEnemiesListProperty = serializedObject.FindProperty("_enemies");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add all enemies"))
            {
                _allEnemiesListProperty.ClearArray();
                var enemiesData = AssetUtils.GetAllInstances<EnemyData>(true, new string[] { @"Assets/Gameplay/Enemies" });

                foreach (var e in enemiesData)
                {
                    SerializedPropertiesHelper.AddArrayItem(e, _allEnemiesListProperty);
                }
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
