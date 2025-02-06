using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using UnityEditor;
using UnityEngine;

namespace TankLike
{
    [CustomEditor(typeof(WaveData))]
    [CanEditMultipleObjects]
    public class WaveDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!GUI.changed)
            {
                return;
            }

            WaveData wave = (WaveData)target;

            wave.SetCapacity();
        }
    }
}
