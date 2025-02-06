using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TankLike.Utils.Editor;

namespace TankLike.UnitControllers.Editor
{
    [CustomEditor(typeof(PlayersDatabase))]
    public class PlayersDatabaseEditor : UnityEditor.Editor
    {
        SerializedProperty _allPlayersListProperty;

        private void Awake()
        {
            _allPlayersListProperty = serializedObject.FindProperty("_players");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add all players"))
            {
                _allPlayersListProperty.ClearArray();
                var playersData = AssetUtils.GetAllInstances<PlayerData>(true, new string[] { @"Assets/Gameplay/Players" });

                foreach (var p in playersData)
                {
                    SerializedPropertiesHelper.AddArrayItem(p, _allPlayersListProperty);
                }
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
