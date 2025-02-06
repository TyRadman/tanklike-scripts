using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TankLike.EditorTools
{
    public class EnemiesGameEditorPage : BaseGameEditorPage
    {
        private SerializedObject _dataObject;
        private SerializedProperty _enemyWavesProperty;
        private ReorderableList _reorderableList;

        public override EGameEditorPageTag PageTag()
        {
            return EGameEditorPageTag.Enemies;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            _data = GetData();

            SetUpEnemyWavesList();
        }

        private void SetUpEnemyWavesList()
        {
            if (_data != null)
            {
                // cache the data asset and the list property
                _dataObject = new SerializedObject(_data);
                _enemyWavesProperty = _dataObject.FindProperty(nameof(_data.EnemyTutorialWaves));

                // Initialize the reorderable list
                _reorderableList = new ReorderableList(_dataObject, _enemyWavesProperty, true, true, true, true)
                {
                    drawHeaderCallback = DrawHeader,
                    drawElementCallback = DrawElement,
                    onAddCallback = AddElement,
                    onRemoveCallback = RemoveElement
                };
            }
        }

        public override void OnGUI()
        {
            RenderHeader("Game Editor - Enemies Data", 30, 20, 20, true);

            RenderSection("Enemies", RenderGameDifficultyAndSpawn);
            RenderSection("Enemy Waves", RenderEnemyWaves);

            EditorUtility.SetDirty(_data);
        }

        private void RenderGameDifficultyAndSpawn()
        {
            GUILayout.Label("Spawn Enemies");

            _data.SpawnEnemies = GUILayout.Toggle(_data.SpawnEnemies, "");
            _data.Difficulty = EditorGUILayout.Slider("Difficulty", _data.Difficulty, 0f, 1f);
        }

        #region Enemy Waves
        private void RenderEnemyWaves()
        {
            if (_reorderableList == null)
            {
                return;
            }
         
            // Update and draw the reorderable list
            _dataObject.Update();
            _reorderableList.DoLayoutList();
            _dataObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Enemy Waves");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _enemyWavesProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element,
                GUIContent.none
            );
        }

        private void AddElement(ReorderableList list)
        {
            _enemyWavesProperty.arraySize++;
            _dataObject.ApplyModifiedProperties();
        }

        private void RemoveElement(ReorderableList list)
        {
            if (list.index >= 0 && list.index < _enemyWavesProperty.arraySize)
            {
                _enemyWavesProperty.DeleteArrayElementAtIndex(list.index);
                _dataObject.ApplyModifiedProperties();
            }
        }
        #endregion
    }
}
