using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike
{
    using Combat.SkillTree;

    [CustomEditor(typeof(SkillTreeCell))]
    [CanEditMultipleObjects]
    public class SkillTreeCellEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SkillTreeCell cell = (SkillTreeCell)target;

            //if (GUILayout.Button("Copy"))
            //{
            //    cell.ConnectedCells = new List<UI.SkillTree.SkillsConnectedCell>();

            //    for (int i = 0; i < cell.ConnectedCells.Count; i++)
            //    {
            //        var old = cell.ConnectedCellsTemp[i];
            //        var newOne = new UI.SkillTree.SkillsConnectedCell();
            //        cell.ConnectedCells.Add(newOne);
            //        newOne.Cell = old.Cell;
            //        newOne.CellDirection = old.CellDirection;
            //    }

            //    EditorUtility.SetDirty(cell);
            //}

            if (cell.Holder == null)
            {
                return;
            }

            if (cell.LastPosition != cell.RectTransform.anchoredPosition)
            {
                cell.LastPosition = cell.RectTransform.anchoredPosition;
                cell.Holder.GenerateConnectionLines();
            }
        }
    }
}
