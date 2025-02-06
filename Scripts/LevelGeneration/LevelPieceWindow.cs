#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace TankLike
{
    public class LevelPieceWindow : EditorWindow
    {
        private void CreateGUI()
        {
            AddGraphView();
        }

        private void AddGraphView()
        {
            LevelPiecesGraphView graph = new LevelPiecesGraphView();
            graph.StretchToParentSize();
            rootVisualElement.Add(graph);
        }
    }
}
#endif