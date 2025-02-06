using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.UI
{
    [CustomEditor(typeof(GridAssigner))]
    public class GridAssignerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GridAssigner assigner = (GridAssigner)target;

            if (GUILayout.Button("Assign grid inputs"))
            {
                assigner.AssignInputs(assigner.Cells);
            }

            if (GUILayout.Button("Dehighlight Cells"))
            {
                assigner.DehighlightCells();
            }

            if (GUILayout.Button("Set User To Cells"))
            {
                assigner.SetCellsUser();
            }

            if (GUILayout.Button("Rename"))
            {
                assigner.Rename();
            }
        }
    }
}
