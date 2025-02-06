using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankLike.Environment;

namespace TankLike.Testing.LevelDesign
{
    [CustomEditor(typeof(LevelDesignManager))]
    public class LevelDesignEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LevelDesignManager manager = (LevelDesignManager)target;

            if (GUILayout.Button("Generate Map"))
            {
                Room newRoom = Instantiate(manager.RoomReference);
                manager.BuildRoom(manager.MapToBuild, manager.LevelData, newRoom, manager.Configs);
            }
        }
    }
}
