using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.Attributes
{
    [CustomPropertyDrawer(typeof(Required))]
    public class RequiredPropertyDrawer : PropertyDrawer
    {
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Code/Core/Attributes/Icons/T_WarningIcon.png");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            Rect fieldRect;
            bool isEmpty = IsFieldEmpty(property);

            if (isEmpty)
            {
                GUI.color = Color.red;
                fieldRect = new Rect(position.x, position.y, position.width - 20, position.height);
                EditorGUI.PropertyField(fieldRect, property, label, true);
                GUI.color = Color.white;
            }
            else
            {
                fieldRect = new Rect(position.x, position.y, position.width - 20, position.height);
                EditorGUI.PropertyField(fieldRect, property, label, true);
            }

            if (isEmpty)
            {
                GUI.color = Color.red;
                Rect iconRect = new Rect(fieldRect.xMax, fieldRect.y, 20, 20);
                GUI.Label(iconRect, new GUIContent(icon, "This field must have a value"));
                GUI.color = Color.white; // Reset the icon color
            }

            if(EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }

        private bool IsFieldEmpty(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference when property.objectReferenceValue != null:
                case SerializedPropertyType.ExposedReference when property.exposedReferenceValue != null:
                case SerializedPropertyType.String when string.IsNullOrEmpty(property.stringValue) == false:
                case SerializedPropertyType.ArraySize when property.arraySize > 0:
                case SerializedPropertyType.AnimationCurve when property.animationCurveValue is { length: > 0 }:
                    return false;
                default:
                    return true;
            }
        }
    }
}
