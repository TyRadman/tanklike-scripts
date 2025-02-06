using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike
{
    [CustomEditor(typeof(Break))]
    public class BreakEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            Break.Data d = ((Break)target).Info;
            Break b = (Break)target;

            Rect r = (Rect)EditorGUILayout.BeginVertical();
            GUI.color = d.BoxColor * 5;
            GUI.Box(r, "");
            GUI.color = d.FontColor;
            GUILayout.Space(d.BoxSize);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = d.FontColor;
            style.fontSize = d.FontSize;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            b._message = GUILayout.TextField(b._message, style);
            GUILayout.Space(d.BoxSize);
            EditorGUILayout.EndVertical();

        }
    }
}
