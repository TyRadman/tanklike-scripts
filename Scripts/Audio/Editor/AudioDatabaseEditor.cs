using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankLike.Utils.Editor;

namespace TankLike.Sound
{
    [CustomEditor(typeof(AudioDatabase))]
    public class AudioDatabaseEditor : Editor
    {
        SerializedProperty _allToolsListProperty;

        private void OnEnable()
        {
            _allToolsListProperty = serializedObject.FindProperty("_audios");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add Audios"))
            {
                _allToolsListProperty.ClearArray();
                List<Audio> toolsData = AssetUtils.GetAllInstances<Audio>(true, new string[] { @"Assets/Gameplay/Audio" });

                foreach (Audio t in toolsData)
                {
                    SerializedPropertiesHelper.AddArrayItem(t, _allToolsListProperty);
                }

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
