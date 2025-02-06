using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TankLike.UnitControllers;
using TankLike.Utils;
using TankLike.Combat;
using TankLike.Combat.SkillTree;

namespace TankLike.UI.SkillTree
{
    [RequireComponent(typeof(GridAssigner))]
    public class SkillTreeBuilder : MonoBehaviour
    {
        private const string SKILL_TREE_PATH = "Assets/UI/SkillTree/Prefabs/Player Skill Trees/";
        private const int BRANCHES_NUMBER = 4;
        public bool UpdateEditor = false;
        [Header("References")]
        [SerializeField] private Transform[] _skillTreeParents;
        [SerializeField] private SkillTreeCell _cellPrefab;
        [SerializeField] private SkillTreeLine _linePrefab;
        [SerializeField] private Transform _linesParent;
        [SerializeField] private SkillTreeCell _centerCell;                                                                                                                                        
        [Header("Values")]
        [SerializeField] private float _distanceBetweenCells = 50f;
        private List<SkillTreeCell> _cells = new List<SkillTreeCell>();
        private Vector2[] _parentsDirections = new Vector2[] { Vector2.up, Vector2.left, Vector2.down, Vector2.right };
        private List<RectTransform> _lines = new List<RectTransform>();
        [SerializeField] private Color _unavailableCellColor;
        [SerializeField] private Color _lockedCellColor;
        [SerializeField] private Color _unlockedCellColor;
        [Header("Random Branch Variables")]
        [SerializeField] private List<Skill> _mutualSkills;
        [SerializeField] private int _skillsNumber = 10;
        [Header("Saving Loading Variables")]
        [SerializeField] private SkillTreePrefab _skillTree;
        /// <summary>
        /// The holder of the skill tree being built currently
        /// </summary>
        [SerializeField] private List<Path> _paths = new List<Path>();
        private int _index = 0;

        public void BuildSkillTree(SkillTreePrefab skillTreePrefab)
        {
            _skillTree = skillTreePrefab;
            //_paths = skillTreePrefab.Paths;
            CreateSkillTree();
        }

        public List<SelectableCell> GetCells()
        {
            List<SelectableCell> newList = new List<SelectableCell>();
            //_cells.ForEach(c => newList.Add(c));
            return newList;
        }

        public void CreateSkillTreeEditor()
        {
            BuildSkillTree(_skillTree);
        }

        public void CreateSkillTree()
        {
            DeleteCells();
            GenerateRandomBranch();
            SetUpInput(0);
            SetUpProgressionPaths();
            HideRandomBranch(_paths[BRANCHES_NUMBER - 1]);
        }

        private void GenerateRandomBranch()
        {
            // duplicate the mutual skills list to avoid messing up the original list
            List<Skill> skills = _mutualSkills.Duplicate();
            // get the number of skills the first path is going to have
            int firstBranchNumber = Random.Range((int)(_skillsNumber / 2), _skillsNumber);
            // create the first and the main path that will hold the legendary skill
            _paths.Add(GenerateRandomPath(skills, firstBranchNumber));
            // create the secondary path
            _paths[BRANCHES_NUMBER - 1].Paths.Add(GenerateRandomPath(skills, _skillsNumber - firstBranchNumber));
        }

        #region Tree Generation
        private void BuildPaths(Path path, Transform parent, Vector2 lastPosition)
        {
            // make sure there are no more than 3
            if (path.Paths.Count > 3)
            {
                Debug.LogError("Can't have more than 3 sub-branches.");

                for (int i = path.Paths.Count - 1; i >= 3; i--)
                {
                    path.Paths.RemoveAt(i);
                }
            }

            // set the directions automatically
            for (int i = 0; i < path.Paths.Count; i++)
            {
                path.Paths[i].Position = (BranchPosition)i;
            }

            for (int i = 0; i < path.Paths.Count; i++)
            {
                Direction sideWay = path.Paths[i].Position == BranchPosition.Left ? Direction.Left :  Direction.Right;
                List<Direction> directions = GetDirectionsForRoad(path.Paths[i], sideWay, path.Paths[0]);
                CreateRoadToCell(directions, parent, path.Paths[i], lastPosition);
            }
        }

        public void CreateCell(SkillTreeCell cellPrefab, Vector2 parentPosition, Transform parent, Vector2 previousPosition, Path path)
        {
            float angle = Mathf.Atan2(parentPosition.y - previousPosition.y, parentPosition.x - previousPosition.x) * Mathf.Rad2Deg + 90;
            Vector2 position = parentPosition;
            SkillTreeLine line = Instantiate(_linePrefab, position, Quaternion.identity, _linesParent);
            path.Lines.Add(line);
            
            if (cellPrefab == null) return;

            SkillTreeCell cell = Instantiate(cellPrefab, parentPosition, Quaternion.identity, parent);
            cell.name = $"Skill {_index++}";
            _cells.Add(cell);
            path.Cell = cell;
            //cell.SetUp(path.Skill);
        }

        // creates one cell with the road leading to it
        public void CreateRoadToCell(List<Direction> directions, Transform parent, Path path, Vector2 lastPosition)
        {
            Vector2 position = lastPosition;
            Vector2 previousPosition;
            path.Lines.Clear();
            Direction lastDirection = Direction.Up;

            // create the empty cells that will be placed at the corners
            for (int i = 0; i < directions.Count - 1; i++)
            {
                previousPosition = position;
                position += _distanceBetweenCells * GetDirectionVector(directions[i], parent);
                // update the last position
                CreateCell(null, position, parent, previousPosition, path);
            }

            // just to draw the line
            previousPosition = position;
            // create the main cell
            position += _distanceBetweenCells * GetDirectionVector(lastDirection, parent);
            CreateCell(_cellPrefab, position, parent, previousPosition, path);

            // create the next path
            if(path.Paths.Count > 0)
            {
                BuildPaths(path, parent, position);
            }
        }
        #endregion

        #region Helper Methods
        public Vector2 GetDirectionVector(Direction direction, Transform parent)
        {
            switch (direction)
            {
                case Direction.Down:
                    {
                        return -parent.up;
                    }

                case Direction.Left:
                    {
                        return -parent.right;
                    }

                case Direction.Right:
                    {
                        return parent.right;
                    }

                case Direction.Up:
                    {
                        return parent.up;
                    }
            }

            return Vector2.up;
        }

        public List<Direction> GetDirectionsForRoad(Path path, Direction sideWayDirection, Path centerPath)
        {
            List<Direction> directions = new List<Direction>();
            int sideWayPushes = 0;

            if (path.Position != BranchPosition.Center)
            {
                directions.Add(sideWayDirection);

                sideWayPushes = GetAllPossibleSideWays(path);
                sideWayPushes += GetAllPossibleSideWays(centerPath);

                for (int i = 0; i < sideWayPushes; i++)
                {
                    directions.Add(sideWayDirection);
                }
            }

            directions.Add(Direction.Up);

            string list = $"{sideWayPushes}. Branch: {path.Position}, Direction: {sideWayDirection}. List: ";

            for (int i = 0; i < directions.Count; i++)
            {
                list += directions[i].ToString().Color(Color.red) + ", ";
            }

            //print(list);
            return directions;
        }

        private int GetAllPossibleSideWays(Path path)
        {
            int counter = 0;

            if (path.Paths.Count <= 1) return 0;

            if (path.Position == BranchPosition.Center && path.Paths.Count >= 2)
            {
                counter++;
            }
            else if (path.Position == BranchPosition.Right && path.HasLeftPath())
            {
                counter++;
            }
            else if (path.Position == BranchPosition.Left && path.HasRightPath())
            {
                counter++;
            }

            for (int i = 0; i < path.Paths.Count; i++)
            {
                counter += GetAllPossibleSideWays(path.Paths[i]);
            }

            //print($"Got here {path.Position}. Number of paths is {path.Paths.Count}, and we got {counter} ".Color(Color.green));
            return counter;
        }

        public void DeleteCells()
        {
            for (int j = 0; j < _skillTreeParents.Length; j++)
            {
                int childCount = _skillTreeParents[j].childCount;

                for (int i = childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(_skillTreeParents[j].GetChild(i).gameObject);
                }

                _cells.Clear();
            }

            if(_paths.Count > 3) _paths.RemoveAt(3);
            
            _paths.ForEach(p => p.Lines.Clear());

            DeleteExtraPaths();

            _lines.Clear();
            int linesCount = _linesParent.childCount;

            for (int i = linesCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_linesParent.GetChild(i).gameObject);
            }

            SkillTreeCell.LOCKED_PATH_COLOR = _lockedCellColor;
            SkillTreeCell.UNLOCKED_PATH_COLOR = _unlockedCellColor;
            SkillTreeCell.UNAVAILABLE_PATH_COLOR = _unavailableCellColor;
        }

        private void DeleteExtraPaths()
        {
            if (_paths.Count > 3)
            {
                for (int i = _paths.Count - 1; i > 2; i--)
                {
                    _paths.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Input
        public void SetUpInput(int playerIndex)
        {
            GetComponent<GridAssigner>().AssignInputs(GetCells());
        }

        public void SetUpProgressionPaths()
        {
            for (int i = 0; i < _paths.Count; i++)
            {
                SetBranchesNextSkills(_paths[i], null);
            }
        }

        private void SetBranchesNextSkills(Path path, SkillTreeCell previousCell)
        {
            // add the lines to the cell
            //path.Lines.ForEach(l => path.Cell.AddLine(l));
            
            // set the cells to unavailable if it was set to NONE. If not, then it's probably a locked cell which we don't want to change
            if (path.Cell.CellState != CellState.Locked)
            {
                path.Cell.ChangeCellState(CellState.Unavailable);
            }

            if (path.Paths.Count == 0)
            {
                return;
            }

            for (int i = 0; i < path.Paths.Count; i++)
            {
                // set the next branch for the path
                //path.Cell.AddNextCell(path.Paths[i].Cell);
                // look for next branches for the current next branch (if that makes any sense)
                SetBranchesNextSkills(path.Paths[i], path.Cell);
            }
        }
        #endregion

        #region Random Branch Generation
        private Path GenerateRandomPath(List<Skill> skills, int counter)
        {
            counter--;

            if (counter < 0) return null;

            Path path = new Path();
            path.IsHidden = true;
            path.Skill = skills.RandomItem(true);

            if (counter > 0)
            {
                path.Paths.Add(GenerateRandomPath(skills, counter));
            }

            return path;
        }

        private void HideRandomBranch(Path path)
        {
            // hide it
            //path.Cell.ShowOverlayImage(true);

            if (path.Paths.Count == 0) return;

            path.Paths.ForEach(p => HideRandomBranch(p));
        }
        #endregion

        #region Loading and Saving Trees
        public void SaveSkillTree()
        {
            if (_skillTree == null)
            {
                Debug.LogError("No skill tree prefab is assigned");
                return;
            }

            //_skillTree.CopyPaths(_paths);
        }

        public void SaveAsNewSkillTree(string name)
        {
            if(name.Length == 0) name = "Skill tree";

            _skillTree = ScriptableObject.CreateInstance<SkillTreePrefab>();
            //_skillTree.CopyPaths(_paths);
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(_skillTree, SKILL_TREE_PATH + name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        public void LoadSkillTree()
        {
            if(_skillTree == null)
            {
                print($"No skill tree scriptable Object attached");
                return;
            }

            //_paths = _skillTree.Paths;

            if(_paths.Count < 3)
            {
                int count = _paths.Count;

                for (int i = 0; i < 3 - count; i++)
                {
                    _paths.Add(new Path());
                }
            }
            else if(_paths.Count > 3)
            {
                DeleteExtraPaths();
            }

            CreateSkillTree();
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SkillTreeBuilder))]
    public class SkillTreeBuilderEditor : Editor
    {
        private string _name;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SkillTreeBuilder tree = (SkillTreeBuilder)target;

            if (GUI.changed && tree.UpdateEditor)
            {
                tree.CreateSkillTree();
            }

            if (GUILayout.Button("Create Cell"))
            {
                tree.CreateSkillTreeEditor();
            }

            if (GUILayout.Button("Save Skill Tree"))
            {
                tree.SaveSkillTree();
            }

            if (GUILayout.Button("Save As New"))
            {
                tree.SaveAsNewSkillTree(_name);
            }

            _name = EditorGUILayout.TextField("File Name", _name);

            if (GUILayout.Button("Load Skill Tree"))
            {
                tree.LoadSkillTree();
            }

            if (GUILayout.Button("Delete Paths"))
            {
                tree.DeleteCells();
            }
        }
    }
#endif

    public enum BranchPosition
    {
        Center = 0, Left = 1, Right = 2
    }

    [System.Serializable]
    public class Path
    {
        public Skill Skill;
        [HideInInspector] public SkillTreeCell Cell;
        public List<Path> Paths = new List<Path>();
        [HideInInspector] public BranchPosition Position;
        [HideInInspector] public List<SkillTreeLine> Lines = new List<SkillTreeLine>();
        [HideInInspector] public bool IsHidden = false;

        public bool HasRightPath()
        {
            return Paths.Count == 3;
        }

        public bool HasLeftPath()
        {
            return Paths.Count > 1;
        }
    }
}
