using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.Attributes
{
    [CustomPropertyDrawer(typeof(AllowCreationIfNull))]
    public class AllowCreationIfNullPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // check if the referenced field is a ScriptableObject and is null
            bool isValidScriptableObjectField =
                property.propertyType == SerializedPropertyType.ObjectReference &&
                fieldInfo.FieldType.IsSubclassOf(typeof(ScriptableObject)) &&
                property.objectReferenceValue == null &&
                !fieldInfo.FieldType.IsAbstract;

            if (isValidScriptableObjectField)
            {
                Rect fieldRect = new Rect(position.x, position.y, position.width - 70, position.height);

                EditorGUI.PropertyField(fieldRect, property, label);

                Rect buttonRect = new Rect(position.x + position.width - 65, position.y, 65, position.height);

                if (GUI.Button(buttonRect, "Create"))
                {
                    CreateScriptableObjectInstance(property);
                }
            }
            else
            {
                // Draw only the property field without the button
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private void CreateScriptableObjectInstance(SerializedProperty property)
        {
            // Ensure the field type is a ScriptableObject
            System.Type fieldType = fieldInfo.FieldType;

            if (!typeof(ScriptableObject).IsAssignableFrom(fieldType))
            {
                Debug.LogWarning($"{fieldType.Name} is not a ScriptableObject.");
                return;
            }

            // Determine the path based on the object type
            string assetPath = DetermineTargetPath(property);

            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Failed to determine a valid path for the new ScriptableObject.");
                return;
            }

            // Generate a unique path for the new ScriptableObject
            assetPath = AssetDatabase.GenerateUniqueAssetPath($"{assetPath}/New {fieldType.Name}.asset");

            // Create and save the ScriptableObject
            ScriptableObject newInstance = ScriptableObject.CreateInstance(fieldType);
            AssetDatabase.CreateAsset(newInstance, assetPath);
            AssetDatabase.SaveAssets();

            // Assign the new instance to the property
            property.objectReferenceValue = newInstance;
            property.serializedObject.ApplyModifiedProperties();

            Debug.Log($"Created new {fieldType.Name} at {assetPath}");
        }

        private string DetermineTargetPath(SerializedProperty property)
        {
            // Check if the serialized object belongs to an asset in the Assets folder
            var targetObject = property.serializedObject.targetObject;

            string assetPath = AssetDatabase.GetAssetPath(targetObject);
            if (!string.IsNullOrEmpty(assetPath) && AssetDatabase.IsValidFolder(System.IO.Path.GetDirectoryName(assetPath)))
            {
                // Return the folder containing the asset
                return System.IO.Path.GetDirectoryName(assetPath).Replace("\\", "/");
            }

            // If the object is in the scene, default to the Assets folder
            Debug.LogWarning("The object is in the Scene. Defaulting to the Assets folder.");
            return "Assets";
        }

        //private void CreateScriptableObjectInstance(SerializedProperty property)
        //{
        //    // Ensure the property is a ScriptableObject reference
        //    if (property.propertyType != SerializedPropertyType.ObjectReference)
        //    {
        //        Debug.LogWarning("CreateIfNull only works with ScriptableObject fields.");
        //        return;
        //    }

        //    // Get the type of the field
        //    System.Type fieldType = fieldInfo.FieldType;

        //    // Ensure the type is a ScriptableObject
        //    if (!typeof(ScriptableObject).IsAssignableFrom(fieldType))
        //    {
        //        Debug.LogWarning($"{fieldType.Name} is not a ScriptableObject.");
        //        return;
        //    }

        //    // Prompt the user to save the ScriptableObject in the currently selected folder
        //    string folderPath = GetCurrentProjectFolder();
        //    string assetPath = EditorUtility.SaveFilePanelInProject(
        //        "Save ScriptableObject",
        //        $"New {fieldType.Name}.asset",
        //        "asset",
        //        $"Create a new {fieldType.Name} ScriptableObject",
        //        folderPath
        //    );

        //    if (string.IsNullOrEmpty(assetPath)) return;

        //    // Create the instance and save it
        //    ScriptableObject newInstance = ScriptableObject.CreateInstance(fieldType);
        //    AssetDatabase.CreateAsset(newInstance, assetPath);
        //    AssetDatabase.SaveAssets();

        //    // Assign the newly created instance to the field
        //    property.objectReferenceValue = newInstance;
        //    property.serializedObject.ApplyModifiedProperties();
        //}

        private string GetCurrentProjectFolder()
        {
            // Get the currently selected folder in the Project tab
            Object[] selectedObjects = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);
            if (selectedObjects.Length > 0)
            {
                string path = AssetDatabase.GetAssetPath(selectedObjects[0]);
                if (AssetDatabase.IsValidFolder(path))
                    return path;
            }

            // Default to the Assets folder if no folder is selected
            return "Assets";
        }
    }
}
