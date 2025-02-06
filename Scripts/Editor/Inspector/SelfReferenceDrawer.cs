using UnityEditor;
using UnityEngine;

namespace TankLike.Attributes
{
    [CustomPropertyDrawer(typeof(InSelf))]
    public class SelfReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                GameObject targetGameObject = (property.serializedObject.targetObject as MonoBehaviour)?.gameObject;

                if (targetGameObject != null)
                {
                    System.Type fieldType = fieldInfo.FieldType;
                    Component component = targetGameObject.GetComponent(fieldType);

                    if(component == null)
                    {
                        InSelf inSelf = attribute as InSelf;

                        if(inSelf.ForceReference)
                        {
                            component = targetGameObject.AddComponent(fieldType);
                        }
                    }

                    if (component != null)
                    {
                        property.objectReferenceValue = component;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
}
