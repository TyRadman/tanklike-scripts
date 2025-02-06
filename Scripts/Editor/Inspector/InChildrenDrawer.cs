using UnityEditor;
using UnityEngine;

namespace TankLike.Attributes
{
    [CustomPropertyDrawer(typeof(InChildren))]
    public class InChildrenDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                GameObject parent = (property.serializedObject.targetObject as MonoBehaviour)?.gameObject;

                if (parent != null)
                {
                    System.Type fieldType = fieldInfo.FieldType;
                    Component component = parent.GetComponentInChildren(fieldType, includeInactive: true);

                    bool forceReference = (attribute as InChildren).ForceReference;

                    if (component != null)
                    {
                        property.objectReferenceValue = component;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    else if (forceReference)
                    {
                        GameObject newChild = new GameObject(fieldType.Name);
                        newChild.transform.parent = parent.transform;
                        Component newComponent = newChild.AddComponent(fieldType);
                        property.objectReferenceValue = newComponent;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }
    }
}
