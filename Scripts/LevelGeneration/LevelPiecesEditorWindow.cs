#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TankLike.LevelGeneration
{
    public class LevelPiecesEditorWindow : EditorWindow
    {
        [MenuItem("Window/Level Pieces Editor Window")]
        public static void ShowExample()
        {
            LevelPiecesEditorWindow wnd = GetWindow<LevelPiecesEditorWindow>();
            wnd.titleContent = new GUIContent("Level Pieces Window");
        }

        private void CreateGUI()
        {
            AddGraphView();
            AddStyles();
        }

        private void AddGraphView()
        {
            LevelPiecesGraphView graph = new LevelPiecesGraphView();
            graph.StretchToParentSize();
            rootVisualElement.Add(graph);
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("LPVariables.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
}
#endif