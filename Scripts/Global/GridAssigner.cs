using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TankLike.UI
{
    /// <summary>
    /// Assigns the input for the grid selectable cells (any object that has a child script of SelectableCell) based on their relative positions to each other.
    /// </summary>
    public class GridAssigner : MonoBehaviour
    {
        public enum SideCellsAssignMethods
        {
            ConnectSidesX, ConnectSidesY, ConnectSidesAll
        }

        [SerializeField] private SideCellsAssignMethods _sideCellsConnection;
        public List<SelectableCell> Cells;
        [SerializeField] private SelectableCell.User AllCellsUser;
        [SerializeField] List<float> xPositions = new List<float>();
        [SerializeField] List<float> yPositions = new List<float>();
        private const float THRESHOLD = 10f;

        public void DehighlightCells()
        {
            Cells.ForEach(c => c.HighLight(false));
        }

        public void AssignInputs(List<SelectableCell> cells)
        {
            xPositions.Clear();
            yPositions.Clear();
            xPositions = new List<float>();
            yPositions = new List<float>();

            cells = cells.OrderByDescending(s => s.transform.position.x).ToList();

            for (int i = 0; i < cells.Count; i++)
            {
                if (!xPositions.Exists(f => Mathf.Abs(f - cells[i].transform.position.x) < THRESHOLD)) xPositions.Add(cells[i].transform.position.x);
            }

            cells = cells.OrderByDescending(s => s.transform.position.y).ToList();

            for (int i = 0; i < cells.Count; i++)
            {
                if (!yPositions.Exists(f => Mathf.Abs(f - cells[i].transform.position.y) < THRESHOLD)) yPositions.Add(cells[i].transform.position.y);
            }

            xPositions.Reverse();

            for (int i = 0; i < cells.Count; i++)
            {
                var c = cells[i];
                float xValue = xPositions.Find(v => Mathf.Abs(v - c.transform.position.x) < THRESHOLD);
                float yValue = yPositions.Find(v => Mathf.Abs(v - c.transform.position.y) < THRESHOLD);
                c.Location = new Vector2Int(xPositions.IndexOf(xValue), yPositions.IndexOf(yValue));
            }

            // get the max dimensions of the skill tree
            Vector2Int xAxis = new Vector2Int(0, xPositions.Count);
            Vector2Int yAxis = new Vector2Int(0, yPositions.Count);

            // set connection
            for (int i = xAxis.x; i <= xAxis.y; i++)
            {
                for (int j = yAxis.x; j <= yAxis.y; j++)
                {
                    var cell = cells.Find(c => c.Location.x == i && c.Location.y == j);

                    if (cell != null)
                    {
                        cell.NextCellInputs = new List<SelectableCell.NextCell>();
                        cell.CreateNextCellsHolders();

                        // horizontal, left
                        SelectableCell.NextCell nextCell = cell.NextCellInputs.Find(c => c.CellDirection == Direction.Left);
                        List<SelectableCell> leftCells = cells.FindAll(c => c.Location.y == cell.Location.y && c.Location.x < cell.Location.x);

                        if (leftCells.Count > 0) nextCell.Cell = leftCells.OrderBy(c => c.Location.x).Last();

                        // right
                        nextCell = cell.NextCellInputs.Find(c => c.CellDirection == Direction.Right);
                        var rightCells = cells.FindAll(c => c.Location.y == cell.Location.y && c.Location.x > cell.Location.x);

                        if (rightCells.Count > 0) nextCell.Cell = rightCells.OrderBy(c => c.Location.x).First();

                        // up
                        nextCell = cell.NextCellInputs.Find(n => n.CellDirection == Direction.Up);
                        var upCells = cells.FindAll(c => c.Location.x == cell.Location.x && c.Location.y < cell.Location.y);

                        if (upCells.Count > 0) nextCell.Cell = upCells.OrderBy(c => c.Location.y).Last();

                        // down
                        nextCell = cell.NextCellInputs.Find(n => n.CellDirection == Direction.Down);
                        var downCells = cells.FindAll(c => c.Location.x == cell.Location.x && c.Location.y > cell.Location.y);

                        if (downCells.Count > 0) nextCell.Cell = downCells.OrderBy(c => c.Location.y).First();
                    }
                }
            }

            switch (_sideCellsConnection)
            {
                case SideCellsAssignMethods.ConnectSidesX:
                    {
                        ConnectOppositeSidesX(cells);
                        break;
                    }
                case SideCellsAssignMethods.ConnectSidesY:
                    {
                        ConnectOppositeSidesY(cells); 
                        break;
                    }
                case SideCellsAssignMethods.ConnectSidesAll:
                    {
                        ConnectOppositeSidesAll(cells);
                        break;
                    }
            }
        }

        private void ConnectOppositeSidesX(List<SelectableCell> cells)
        {
            return;
            int minYValue = cells.OrderBy(c => c.Location.y).FirstOrDefault().Location.y;
            int maxYValue = cells.OrderByDescending(c => c.Location.y).FirstOrDefault().Location.y;

            int loops = Mathf.Abs(minYValue) + maxYValue + 1;

            for (int i = 0; i < loops; i++)
            {
                List<SelectableCell> rowCells = cells.FindAll(c => c.Location.y == i + minYValue);
                int minXValue = rowCells.OrderBy(c => c.Location.x).FirstOrDefault().Location.x;
                int maxXValue = rowCells.OrderByDescending(c => c.Location.x).FirstOrDefault().Location.x;

                SelectableCell leftCell = rowCells.Find(c => c.Location.x == minXValue);
                SelectableCell rightCell = rowCells.Find(c => c.Location.x == maxXValue);

                leftCell.NextCellInputs.Find(c => c.CellDirection == Direction.Left).Cell = rightCell;
                rightCell.NextCellInputs.Find(c => c.CellDirection == Direction.Right).Cell = leftCell;
            }
        }

        private void ConnectOppositeSidesY(List<SelectableCell> cells)
        {
            int minXValue = cells.OrderBy(c => c.Location.x).FirstOrDefault().Location.x;
            int maxXValue = cells.OrderByDescending(c => c.Location.x).FirstOrDefault().Location.x;

            int loops = Mathf.Abs(minXValue) + maxXValue + 1;

            for (int i = 0; i < loops; i++)
            {
                List<SelectableCell> columnsCells = cells.FindAll(c => c.Location.x == i + minXValue);
                int minYValue = columnsCells.OrderBy(c => c.Location.y).FirstOrDefault().Location.y;
                int maxYValue = columnsCells.OrderByDescending(c => c.Location.y).FirstOrDefault().Location.y;

                SelectableCell bottomCell = columnsCells.Find(c => c.Location.y == minYValue);
                SelectableCell topCell = columnsCells.Find(c => c.Location.y == maxYValue);

                // we're not following a logical order in here because the top cells of the skill tree have a lower y index
                bottomCell.NextCellInputs.Find(c => c.CellDirection == Direction.Up).Cell = topCell;
                topCell.NextCellInputs.Find(c => c.CellDirection == Direction.Down).Cell = bottomCell;
            }
        }

        private void ConnectOppositeSidesAll(List<SelectableCell> cells)
        {
            ConnectOppositeSidesX(cells);
            ConnectOppositeSidesY(cells);
        }

        public void SetCellsUser()
        {
            Cells.ForEach(c => c.NextCellInputs.ForEach(nc => nc.CellUser = AllCellsUser));
        }

        public void Rename()
        {
            Cells.ForEach(c => c.name = $"{Cells.IndexOf(c)}_ShopItem");
        }
    }
}
