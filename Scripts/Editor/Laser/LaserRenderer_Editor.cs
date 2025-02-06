using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike
{
    [CustomEditor(typeof(LaserRenderer))]
    public class LaserRenderer_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LaserRenderer laser = (LaserRenderer)target;
            laser.RenderLaser();
        }
    }
}
