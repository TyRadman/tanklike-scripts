using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;

namespace TankLike
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((GameManager)target), typeof(GameManager), false);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();

            GameManager gameManager = (GameManager)target;
            Fold(gameManager);

            if (GUILayout.Button("Assign Components"))
            {
                AutoAssignComponents(gameManager);
            }
        }

        private void AutoAssignComponents(GameManager gameManager)
        {
            // Get all fields that are either public or marked with SerializeField
            var fields = typeof(GameManager).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.IsPublic || f.GetCustomAttributes(typeof(SerializeField), true).Length > 0);

            foreach (var field in fields)
            {
                if (field.GetValue(gameManager) != null) continue;

                System.Type fieldType = field.FieldType;
                Component[] allComponents = new Component[0];

                try
                {
                    allComponents = (Component[])FindObjectsOfType(fieldType);
                }
                catch
                {
                    continue;
                }

                Component foundComponent = allComponents.FirstOrDefault(c => c != gameManager);

                if (foundComponent != null)
                {
                    field.SetValue(gameManager, foundComponent);
                }
                else
                {
                    Debug.LogWarning($"GameManager: No script of type {fieldType} found for variable {field.Name}");
                }
            }
        }

        public void Fold(GameManager manager)
        {
            // just updates the visuals
            serializedObject.Update();

            // Get all fields that are either public or marked with [SerializeField]
            FieldInfo[] fields = serializedObject.targetObject.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(f => f.IsPublic || Attribute.IsDefined(f, typeof(SerializeField)))
                .ToArray();

            string currentHeader = null;

            foreach (FieldInfo field in fields)
            {
                HeaderAttribute headerAttribute = (HeaderAttribute)Attribute.GetCustomAttribute(field, typeof(HeaderAttribute));

                if (headerAttribute != null)
                {
                    currentHeader = headerAttribute.header;

                    if (!manager.FoldoutStates.ContainsKey(currentHeader))
                    {
                        manager.FoldoutStates[currentHeader] = false;
                    }

                    // Toggling the foldout state
                    manager.FoldoutStates[currentHeader] = EditorGUILayout.Foldout(manager.FoldoutStates[currentHeader], currentHeader);
                }

                // Checking if the foldout is open or if there's no header
                if (currentHeader == null || manager.FoldoutStates[currentHeader])
                {
                    SerializedProperty prop = serializedObject.FindProperty(field.Name);
                    if (prop != null)
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }
                    else
                    {
                        // Handle the case where the property is not found or is null
                        EditorGUILayout.LabelField("Property not found or not serializable: " + field.Name);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
