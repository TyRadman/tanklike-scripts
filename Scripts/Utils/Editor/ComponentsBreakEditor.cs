using UnityEditor;
using UnityEngine;

namespace TankLike
{
    [CustomEditor(typeof(ComponentsBreak))]
    public class ComponentsBreakEditor : UnityEditor.Editor
    {
        private string text;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Get a reference to the target script
            ComponentsBreak.Values t = ((ComponentsBreak)target).Value;

            Rect rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.Height(t.Size));
            EditorGUI.DrawRect(rect, t.BoxColor);

            GUIStyle style = new GUIStyle(EditorStyles.textField);

            // Modify the text field style properties as needed
            style.fontSize = t.FontSize; // Set the font size
            style.normal.textColor = t.TextColor;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            Rect textFieldRect = new Rect(rect.x + 5, rect.y + (rect.height - 20) * 0.5f, rect.width - 10, 20);
            t.Text = EditorGUI.TextField(textFieldRect, t.Text, style);
        }

        // Helper function to create a colored texture
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }

}
