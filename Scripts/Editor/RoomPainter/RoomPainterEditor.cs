using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using static Environment.LevelGeneration.RoomPainter;

    [CustomEditor(typeof(RoomPainter), true)]
    public class RoomPainterEditor : Editor
    {
        private ReorderableList paintingRulesList;

        private void Boot()
        {
            // Access the serialized property for the list
            SerializedProperty rulesProperty = serializedObject.FindProperty("PaintingRules");

            // Initialize the ReorderableList
            paintingRulesList = new ReorderableList(serializedObject, rulesProperty, true, true, true, true);

            // Set up the header
            paintingRulesList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Painting Rules");
            };

            // Render each element
            paintingRulesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = rulesProperty.GetArrayElementAtIndex(index);
                rect.y += 4;

                // Display FilterType dropdown at the top
                SerializedProperty filterType = element.FindPropertyRelative("FilterType");
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    filterType,
                    new GUIContent("Filter Type")
                );

                // Render relevant fields based on FilterType
                FilterType selectedFilter = (FilterType)filterType.enumValueIndex;
                rect.y += EditorGUIUtility.singleLineHeight + 4;

                switch (selectedFilter)
                {
                    case FilterType.IsOfType:
                        DrawIsOfTypeFields(element, rect, "TileType");
                        break;

                    case FilterType.IsOfTag:
                        DrawIsOfTypeFields(element, rect, "DestructableTag");
                        break;

                    case FilterType.NeighbourOfTypeWithinDepth:
                    case FilterType.NeighbourOfTypeNotWithinDepth:
                        DrawNeighbourWithinDepthFields(element, rect, "TileType");
                        break;

                    case FilterType.NeighbourOfTagWithinDepth:
                    case FilterType.NeighbourOfTagNotWithinDepth:
                        DrawNeighbourWithinDepthFields(element, rect, "DestructableTag");
                        break;

                    case FilterType.NeighbourOfTypeWithinDepthRange:
                    case FilterType.NeighbourOfTypeNotWithinDepthRange:
                        DrawNeighbourWithinRangeFields(element, rect, "TileType");
                        break;

                    case FilterType.NeighbourOfTagWithinDepthRange:
                    case FilterType.NeighbourOfTagNotWithinDepthRange:
                        DrawNeighbourWithinRangeFields(element, rect, "DestructableTag");
                        break;
                }
            };

            paintingRulesList.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = paintingRulesList.serializedProperty.GetArrayElementAtIndex(index);
                FilterType selectedFilter = (FilterType)element.FindPropertyRelative("FilterType").enumValueIndex;

                float baseHeight = EditorGUIUtility.singleLineHeight * 2; // Base height
                float extraHeight = 0;

                switch (selectedFilter)
                {
                    case FilterType.IsOfType:
                    case FilterType.IsOfTag:
                        extraHeight = EditorGUIUtility.singleLineHeight * 2; // For TileType
                        break;

                    case FilterType.NeighbourOfTagWithinDepth:
                    case FilterType.NeighbourOfTagNotWithinDepth:
                    case FilterType.NeighbourOfTypeWithinDepth:
                    case FilterType.NeighbourOfTypeNotWithinDepth:
                        extraHeight = EditorGUIUtility.singleLineHeight * 3; // For Depth + TileType/Tag
                        break;

                    case FilterType.NeighbourOfTypeWithinDepthRange:
                    case FilterType.NeighbourOfTypeNotWithinDepthRange:
                    case FilterType.NeighbourOfTagWithinDepthRange:
                    case FilterType.NeighbourOfTagNotWithinDepthRange:
                        extraHeight = EditorGUIUtility.singleLineHeight * 4; // For MinDepth, MaxDepth, and TileType
                        break;
                }

                return baseHeight + extraHeight;
            };
        }

        public override void OnInspectorGUI()
        {
            if(serializedObject == null || paintingRulesList == null || serializedObject == null)
            {
                Boot();
            }

            DrawDefaultInspector();
            GUILayout.Space(10);
            serializedObject.Update();
            paintingRulesList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawIsOfTypeFields(SerializedProperty element, Rect rect, string uniqueIdentifierName)
        {
            SerializedProperty performOpposite = element.FindPropertyRelative("PerformOpposite");
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                performOpposite,
                new GUIContent("Perform Opposite")
            );

            rect.y += EditorGUIUtility.singleLineHeight + 4;

            // TileType dropdown
            SerializedProperty tileType = element.FindPropertyRelative(uniqueIdentifierName);
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                tileType,
                new GUIContent("Type / Tag")
            );
        }


        private void DrawNeighbourWithinDepthFields(SerializedProperty element, Rect rect, string uniqueIdentifierName)
        {
            // Depth
            SerializedProperty depth = element.FindPropertyRelative("Depth");
            //rect.y += EditorGUIUtility.singleLineHeight + 4;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                depth,
                new GUIContent("Depth")
            );

            // TileType or DestructableTag dropdown
            SerializedProperty typeProperty = element.FindPropertyRelative(uniqueIdentifierName);
            rect.y += EditorGUIUtility.singleLineHeight + 4;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                typeProperty,
                new GUIContent("Type or tag")
            );
        }

        private void DrawNeighbourWithinRangeFields(SerializedProperty element, Rect rect, string uniqueIdentifierName)
        {  
            // MinDepth and MaxDepth
            SerializedProperty minDepth = element.FindPropertyRelative("MinDepth");
            SerializedProperty maxDepth = element.FindPropertyRelative("MaxDepth");
            //rect.y += EditorGUIUtility.singleLineHeight + 4;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width / 2 - 4, EditorGUIUtility.singleLineHeight),
                minDepth,
                new GUIContent("Min Depth")
            );
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width / 2 + 4, rect.y, rect.width / 2 - 4, EditorGUIUtility.singleLineHeight),
                maxDepth,
                new GUIContent("Max Depth")
            );

            // TileType dropdown
            SerializedProperty tileType = element.FindPropertyRelative(uniqueIdentifierName);
            rect.y += EditorGUIUtility.singleLineHeight + 4;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                tileType,
                new GUIContent("Type or Tag")
            );
        }
    }

}
