using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace TankLike
{
    [CustomPropertyDrawer(typeof(StartFoldAttribute))]
    public class FoldoutPropertyDrawer : PropertyDrawer
    {
        private static Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Create a unique key for this foldout based on the object instance and property path
            string foldoutKey = $"{property.serializedObject.targetObject.GetInstanceID()}_{property.propertyPath}";

            // Ensure the foldout state is initialized
            if (!foldoutStates.ContainsKey(foldoutKey))
            {
                foldoutStates[foldoutKey] = true; // Default to open
            }

            // Draw the foldout header and update its state
            foldoutStates[foldoutKey] = EditorGUI.Foldout(position, foldoutStates[foldoutKey], ((StartFoldAttribute)attribute).Title, true);

            // Only display properties if the foldout is open
            if (foldoutStates[foldoutKey])
            {
                // Traverse through subsequent properties
                SerializedProperty currentProperty = property.Copy();

                while (currentProperty.NextVisible(false)) // Continue to the next property
                {
                    // Stop at the EndFold attribute
                    if (HasEndFoldAttribute(currentProperty))
                    {
                        break;
                    }

                    // Draw the current property
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(currentProperty, true);
                    EditorGUI.indentLevel--;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight; // Only show the header for StartFold
        }

        private bool HasEndFoldAttribute(SerializedProperty property)
        {
            // Check if the property has the EndFold attribute
            var fieldInfo = property.serializedObject.targetObject.GetType()
                               .GetField(property.name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            return fieldInfo != null && System.Attribute.IsDefined(fieldInfo, typeof(EndFoldAttribute));
        }
    }
}
