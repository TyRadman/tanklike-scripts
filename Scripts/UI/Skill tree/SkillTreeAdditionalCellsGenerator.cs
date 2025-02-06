using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree
{
    using Utils;
    using UnitControllers;

    public class SkillTreeAdditionalCellsGenerator : MonoBehaviour
    {
        [SerializeField] private SkillTreeCell _specialCellPrefab;
        [SerializeField] private SkillTreeCell _statCellPrefab;
        [SerializeField] private Transform _specialCellsParent;

        private List<SkillTreeCell> _upperCells = new List<SkillTreeCell>();
        private List<SkillTreeCell> _lowerCells = new List<SkillTreeCell>();
        private List<SkillTreeCell> _leftCells = new List<SkillTreeCell>();
        private List<SkillTreeCell> _rightCells = new List<SkillTreeCell>();

        private List<SkillTreeCell> _specialCells = new List<SkillTreeCell>();
        private SkillTreeHolder _skillTreeHolder;

        private const int SPECIAL_CELL_SKILL_POINTS_COST = 2;
        private const int FURTHEST_BRANCH_INDEX = 5;

        public void SetUp(SkillTreeHolder skillTreeHolder)
        {
            _skillTreeHolder = skillTreeHolder;
        }

        public void GenerateSpecialSkills()
        {
            List<int> indices = GenerateIndices();

            GenerateCell(_rightCells, _upperCells, indices);
            GenerateCell(_rightCells, _lowerCells, indices);
            GenerateCell(_leftCells, _lowerCells, indices);
            GenerateCell(_leftCells, _upperCells, indices);
        }

        private void GenerateCell(List<SkillTreeCell> firstRow, List<SkillTreeCell> secondRow, List<int> indices)
        {
            SkillTreeCell cell = Instantiate(_specialCellPrefab, _specialCellsParent);
            int xIndex = Random.Range(0, indices.Count);
            int yIndex = Random.Range(0, indices.Count - 1);

            try
            {
                cell.SetParentsToUnlockCount(2);
                cell.SetState(CellState.Unavailable);
                cell.UpgradeType = UpgradeTypes.SpecialUpgrade;
                cell.SetSkillPointCost(SPECIAL_CELL_SKILL_POINTS_COST);
                cell.HoldValue = 0.5f;

                int elementXIndex = indices[xIndex];
                SkillTreeCell xCell = firstRow[elementXIndex];
                float xPosition = xCell.RectTransform.localPosition.x;
                indices.RemoveAt(xIndex);

                int elementYIndex = indices[yIndex];
                SkillTreeCell yCell = secondRow[elementYIndex];
                float yPosition = yCell.RectTransform.localPosition.y;
                indices.RemoveAt(yIndex);

                cell.RectTransform.localPosition = new Vector2(xPosition, yPosition);

                _specialCells.Add(cell);

                // allow input between cells
                SetUpSpecialCellInput(elementYIndex, secondRow, cell, yCell, true);
                SetUpSpecialCellInput(elementXIndex, firstRow, cell, xCell, false);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError($"second row count {secondRow.Count} trying to get index {indices[yIndex]}. Indices list size is {indices.Count}");
                string log = string.Empty;
                _log.ForEach(i => log += i + ", ") ;
                Debug.LogError(log);
            }
            // create the extra cells
            //Direction inputDirection = xCell.RectTransform.position.y > cell.RectTransform.position.y ? Direction.Down : Direction.Up;
            //CreateStatCells(inputDirection, cell, firstRow, elementXIndex, true);

                //inputDirection = yCell.RectTransform.position.x > cell.RectTransform.position.x ? Direction.Left : Direction.Right;
                //CreateStatCells(inputDirection, cell, secondRow, elementYIndex, false);
        }

        private void SetUpSpecialCellInput(int elementIndex, List<SkillTreeCell> row, SkillTreeCell specialCell, SkillTreeCell adjacentCell, bool isHorizontal)
        {
            adjacentCell.AddNextCell(specialCell);
            _skillTreeHolder.CreateLineBetweenCells(adjacentCell, specialCell);

            int startingIndex = Mathf.Max(0, elementIndex - 1);
            int maxIndex = Mathf.Min(elementIndex + 2, row.Count);
            Direction inputDirection;

            if (isHorizontal)
            {
                inputDirection = adjacentCell.RectTransform.position.x > specialCell.RectTransform.position.x ? Direction.Left : Direction.Right;
            }
            else
            {
                inputDirection = adjacentCell.RectTransform.position.y > specialCell.RectTransform.position.y ? Direction.Down : Direction.Up;
            }

            specialCell.AddConnectedCell(Helper.GetOppositeDirection(inputDirection), adjacentCell);

            for (int i = startingIndex; i < maxIndex; i++)
            {
                row[i].AddConnectedCell(inputDirection, specialCell);
            }
        }

        private void CreateStatCells(Direction inputDirection, SkillTreeCell specialCell, List<SkillTreeCell> row, int specialIndex, bool isHorizontal)
        {
            SkillTreeCell previousCell = specialCell;

            for (int i = Mathf.Min(specialIndex + 1, row.Count); i < row.Count; i++)
            {
                SkillTreeCell statCell = Instantiate(_statCellPrefab, _specialCellsParent);

                statCell.UpgradeType = UpgradeTypes.SpecialUpgrade;

                SkillTreeCell adjacentCell = row[i];
                Direction adjacentInputDirection;

                if (isHorizontal)
                {
                    statCell.RectTransform.localPosition = new Vector2(adjacentCell.RectTransform.localPosition.x, specialCell.RectTransform.localPosition.y);
                    adjacentInputDirection = specialCell.RectTransform.transform.localPosition.x > statCell.RectTransform.localPosition.x ? Direction.Left : Direction.Right;
                }
                else
                {
                    statCell.RectTransform.localPosition = new Vector2(specialCell.RectTransform.localPosition.x, adjacentCell.RectTransform.localPosition.y);
                    adjacentInputDirection = specialCell.RectTransform.transform.localPosition.y > statCell.RectTransform.localPosition.y ? Direction.Down : Direction.Up;
                }

                // connect the cell to the previous cell
                previousCell.AddNextCell(statCell);
                previousCell.AddConnectedCell(adjacentInputDirection, statCell);

                statCell.AddConnectedCell(Helper.GetOppositeDirection(adjacentInputDirection), previousCell);

                _skillTreeHolder.CreateLineBetweenCells(previousCell, statCell);

                previousCell = statCell;

                // connect cell to adjacent cells
                adjacentCell.AddConnectedCell(inputDirection, statCell);
                statCell.AddConnectedCell(Helper.GetOppositeDirection(inputDirection), adjacentCell);
            }
        }

        private List<int> _log = new List<int>();

        private List<int> GenerateIndices()
        {
            int branchesCount = 8;
            int branchLength = FURTHEST_BRANCH_INDEX - 1;
            int requiredSum = FURTHEST_BRANCH_INDEX / 2 * branchesCount;
            // no index should be less than 2
            int minAllowedValue = 2; 
            List<int> selectedIndices = new List<int>();
            int remainingSum = requiredSum;

            for (int i = 0; i < branchesCount; i++)
            {
                // limits upper bound
                int maxSelectable = Mathf.Min(branchLength, remainingSum - (branchesCount - i - 1));
                // limits lower bound and enforces minAllowedValue
                int minSelectable = Mathf.Max(minAllowedValue, remainingSum - (branchLength) * (branchesCount - i - 1)); 

                // randomly selects an index within the narrowed range
                int selectedIndex = Random.Range(minSelectable, maxSelectable + 1);
                selectedIndices.Add(selectedIndex);

                // adjusts remaining sum
                remainingSum -= selectedIndex;

                _log.Add(selectedIndex);
            }

            return selectedIndices;
        }

        public void SetDownBranch(List<SkillTreeCell> cells)
        {
            _lowerCells = cells;
        }

        public void SetUpBranch(List<SkillTreeCell> cells)
        {
            _upperCells = cells;
        }

        public void SetLeftBranch(List<SkillTreeCell> cells)
        {
            _leftCells = cells;
        }

        public void SetRightBranch(List<SkillTreeCell> cells)
        {
            _rightCells = cells;
        }
    }
}
