using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TankLike.Combat.SkillTree
{
    using UI;
    using UI.Workshop;
    using UnitControllers;
    using Utils;
    using UI.SkillTree;
    using Upgrades;
    using Attributes;
    using TankLike.Sound;
    using TankLike.UI.Signifiers;

    public class SkillTreeHolder : Navigatable
    {
        [SerializeField] private Audio _navigationSFX;
        [SerializeField] private Audio _selectionSFX;
        [SerializeField] private Audio _errorSFX;
        [Header("Caches")]
        [SerializeField] private SkillTreeCell _centerCell;
        [SerializeField] private List<SkillTreeCell> _cells;

        [Header("Line References")]
        [SerializeField] private SkillTreeLine _linePrefab;
        [SerializeField] private Transform _linesParent;

        [Header("Movement References")]
        [SerializeField] private RectTransform _skillTreeRect;
        [SerializeField] private Transform _parent;

        [Header("Components")]
        [SerializeField, InSelf, ReadOnly] private SkillTreeUIDisplayer _UIDisplayer;
        [SerializeField, InSelf, ReadOnly] private SkillTreeUpgradesController _upgradesController;
        [SerializeField, InChildren, ReadOnly] private SkillTreeRandomSkillMenuController _randomSkillsMenu;
        [SerializeField, InSelf, ReadOnly] private SkillTreeAdditionalCellsGenerator _specialCellsGenerator;
        [SerializeField, InSelf, ReadOnly] private SkillTreeBranchBuilder _branchBuilder;
        [SerializeField, InSelf(true), ReadOnly] private SkillTreeInput _input;


        [SerializeField] private UnlockSuperAbilityUpgrade _unlockSuperAbilityUpgrade;
        [SerializeField] private UnlockChargeAttackUpgrade _unlockChargedAttackUpgrade;
        [SerializeField] private Sprite _squareOutlineSprite;

        private Dictionary<Direction, Coroutine> _navigationRoutines = new Dictionary<Direction, Coroutine>()
        {
            {Direction.Up, null},
            {Direction.Down, null},
            {Direction.Left, null},
            {Direction.Right, null }
        };

        private PlayerComponents _currentPlayer;
        private SkillTreeCell _activeSkillCell;
        private WorkShopTabsNavigatable _workShopNavigatable;
        private SkillTreeCell _lastSelectedCell;
        private SkillTreeViewModel _viewModel;
        private Vector2 _destinationToMoveTo;
        private float _skillTreeMovementThreshold;
        private bool _canMove = false;
        private bool _isHolding = false;
        private bool _isInControl = false;

        #region Constants
        private readonly WaitForSeconds _holdUpdateWait = new WaitForSeconds(0.2f);

        private const string EMPTY_NAME_TEXT = "Skill name";
        private const string EMPTY_DESCRIPTION_TEXT = "The skill description";
        private const string EMPTY_POINTS_REQUIRED_TEXT = "--";
        private const string RANDOM_NAME_TEXT = "Random Skill";
        private const string RANDOM_DESCRIPTION_TEXT = "Choose one of two random skills";
        private const string RANDOM_POINTS_REQUIRED_TEXT = "1";
        private const float SELECTION_HOLD_EMPTYING_SPEED_MULTIPLIER = 4f;
        private const int RANDOM_SKILL_POINTS_REQUIRED_AMOUNT = 1;
        private const float SKILL_TREE_MOVEMENT_SPEED = 0.2f;

        private readonly float _navigationCoolDown = 0.05f;

        private readonly Dictionary<UpgradeTypes, string> _upgradeDescriptioins = new Dictionary<UpgradeTypes, string>()
        {
            {UpgradeTypes.BaseWeapon, $"Unlock one of two {"Base Weapon".Color(_upgradeColors[UpgradeTypes.BaseWeapon])}" },
            {UpgradeTypes.ChargeAttack, $"Unlock one of two {"Charge Attack".Color(_upgradeColors[UpgradeTypes.ChargeAttack])}" },
            {UpgradeTypes.SuperAbility, $"Unlock one of two {"Super Ability".Color(_upgradeColors[UpgradeTypes.SuperAbility])}" },
            {UpgradeTypes.StatsUpgrade, $"Unlock one of two {"Tank Upgrades".Color(_upgradeColors[UpgradeTypes.StatsUpgrade])}" },
            {UpgradeTypes.SpecialUpgrade, $"Unlock one of two {"Special Tank Upgrades".Color(_upgradeColors[UpgradeTypes.SpecialUpgrade])}" },
            {UpgradeTypes.None, "Base Cell" }
        };

        private static readonly Dictionary<UpgradeTypes, Color> _upgradeColors = new Dictionary<UpgradeTypes, Color>()
        {
            {UpgradeTypes.BaseWeapon, new Color(0.3f, 0.7f, 1f)},     // Azure Blue
            {UpgradeTypes.ChargeAttack, new Color(0.5f, 0.3f, 0.9f)}, // Bold Purple
            {UpgradeTypes.SuperAbility, new Color(1f, 0.2f, 0.2f)},   // Stellar Violet
            {UpgradeTypes.StatsUpgrade, new Color(0.376f, 0.882f, 0.878f)}, // Teal Blue
            {UpgradeTypes.SpecialUpgrade, new Color(0.933f, 0.380f, 0.137f)},
            {UpgradeTypes.None,  Colors.White}
        };

        private readonly Dictionary<UpgradeTypes, string> _upgradeNames = new Dictionary<UpgradeTypes, string>()
        {
            {UpgradeTypes.BaseWeapon,  "Base Weapon".Color(_upgradeColors[UpgradeTypes.BaseWeapon]) + " Upgrade"},
            {UpgradeTypes.ChargeAttack,  "Charge Attack".Color(_upgradeColors[UpgradeTypes.ChargeAttack]) + " Upgrade"},
            {UpgradeTypes.SuperAbility,  "Super Ability".Color(_upgradeColors[UpgradeTypes.SuperAbility]) + " Upgrade" },
            {UpgradeTypes.StatsUpgrade,  "Tank Upgrade".Color(_upgradeColors[UpgradeTypes.StatsUpgrade])},
            {UpgradeTypes.SpecialUpgrade,  "Special Upgrade".Color(_upgradeColors[UpgradeTypes.SpecialUpgrade])},
            {UpgradeTypes.None,  "Base Cell"}
        };
        #endregion

        #region Set up
        public void BuildBranches()
        {
            _cells.AddRange(_branchBuilder.BuildBranches());
        }

        public override void SetUp()
        {
            base.SetUp();

            _cells.AddRange(_branchBuilder.BuildBranches());
            _isInControl = true;

            _workShopNavigatable = GameManager.Instance.WorkshopController.WorkshopUI;

            SetCellColors();

            _activeSkillCell = _centerCell;

            CancelInvoke();
            _canMove = false;

            _skillTreeMovementThreshold = _branchBuilder.GetCellsDistance() * _parent.localScale.x;

            if (_specialCellsGenerator != null)
            {
                _specialCellsGenerator.SetUp(this);
                _specialCellsGenerator.GenerateSpecialSkills();
            }

            _upgradesController.SetUp(_currentPlayer);

            _viewModel = new SkillTreeViewModel(_currentPlayer, _UIDisplayer);

            GameManager.Instance.WorkshopController.OnWorkshopSpawned += OnWorkshopSpawned;

            SetupUnlockAbilitiesCells();
        }

        private void SetupUnlockAbilitiesCells()
        {

            _unlockSuperAbilityUpgrade.SetUp(_currentPlayer);
            SkillTreeCell superAbilityCell = _centerCell.ConnectedCells.Find(c => c.CellDirection == Direction.Left).Cell as SkillTreeCell;

            if(superAbilityCell != null)
            {
                superAbilityCell.IsRandomSkill = false;
                superAbilityCell.SetUpgrade(_unlockSuperAbilityUpgrade);
                superAbilityCell.SetUpLockIcon(_unlockSuperAbilityUpgrade.GetUpgradeMainInfo().Icon);
                superAbilityCell.SetOutline(_squareOutlineSprite);
            }

            _unlockChargedAttackUpgrade.SetUp(_currentPlayer);
            SkillTreeCell chargeAttackCell = _centerCell.ConnectedCells.Find(c => c.CellDirection == Direction.Right).Cell as SkillTreeCell;

            if (chargeAttackCell != null)
            {
                chargeAttackCell.IsRandomSkill = false;
                chargeAttackCell.SetUpgrade(_unlockChargedAttackUpgrade);
                chargeAttackCell.SetUpLockIcon(_unlockChargedAttackUpgrade.GetUpgradeMainInfo().Icon);
                chargeAttackCell.SetOutline(_squareOutlineSprite);
            }
        }

        public override void SetUpActionSignifiers(ISignifierController signifierController)
        {
            base.SetUpActionSignifiers(signifierController);

            if (signifierController is UIActionSignifiersController actionSignifiersController)
            {
                _input.SetUpActionSignifiers(actionSignifiersController);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            GameManager.Instance.WorkshopController.OnWorkshopSpawned -= OnWorkshopSpawned;
        }

        private void SetCellColors()
        {
            _cells.ForEach(c => c.SetUp());
            _centerCell.SetColor(Colors.White);

            for (int i = 0; i < _cells.Count; i++)
            {
                _cells[i].SetLockedImageColor(_upgradeColors[_cells[i].UpgradeType]);
            }
        }

        public void SetPlayer(PlayerComponents player)
        {
            _currentPlayer = player;
        }

        private void OnWorkshopSpawned()
        {
            SetActiveCell(_centerCell);
            _destinationToMoveTo = Vector3.zero;
            _skillTreeRect.localPosition = Vector3.zero;
        }
        #endregion

        #region Open, Close and Load
        public override void Open(int playerIndex = 0)
        {
            base.Open(playerIndex);
            _activeSkillCell.Highlight();
            _canMove = true;
            _input.SetUpInput(PlayerIndex);
            GameManager.Instance.InputManager.EnableUIInput();

            _centerCell.ChangeCellState(CellState.Unlocked);
            UpdateSkillTreeUI();

        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsOpened)
            {
                return;
            }

            if(_activeSkillCell.GetProgressAmount() != 0f)
            {
                _activeSkillCell.SetProgressAmount(0f);
            }

            // reset the size and effects of the highlighted cell
            _activeSkillCell.ApplyPreset();
            base.Close(playerIndex);
            CancelInvoke();
            _input.DisposeInput(PlayerIndex);

        }
        #endregion

        #region Input
        public void StartSelection()
        {
            if (!_isInControl)
            {
                return;
            }

            StopAllCoroutines();

            StartCoroutine(SelectionHoldProcess());
        }

        public void EndSelection()
        {
            if (!_isHolding || !_isInControl)
            {
                return;
            }

            StopAllCoroutines();
            StartCoroutine(SelectionReleaseHoldProcess());
        }

        public void NavigateByDirection(Direction direction)
        {
            if (!_isInControl)
            {
                return;
            }

            this.StopCoroutineSafe(_navigationRoutines[direction]);

            _navigationRoutines[direction] = StartCoroutine(NavigationRoutinte(direction));
        }

        public void StopNavigationByDirection(Direction direction)
        {
            this.StopCoroutineSafe(_navigationRoutines[direction]);
        }

        private IEnumerator NavigationRoutinte(Direction direction)
        {
            while (IsOpened)
            {
                Navigate(direction);
                yield return _holdUpdateWait;
            }
        }
        #endregion

        #region Input methods

        #region Navigation
        public override void Navigate(Direction direction)
        {
            if (!IsOpened || !_canMove)
            {
                return;
            }

            base.Navigate(direction);

            SkillsConnectedCell nextCell = _activeSkillCell.ConnectedCells.Find(c => c.CellDirection == direction);

            if (nextCell == null || nextCell.Cell == null)
            {
                return;
            }

            _activeSkillCell.Unhighlight();
            _activeSkillCell = nextCell.Cell as SkillTreeCell;
            _activeSkillCell.Highlight();

            Vector2 cellLocalPosition = _activeSkillCell.RectTransform.localPosition;
            float xPosition = Mathf.Max(0f, Mathf.Abs(cellLocalPosition.x) - _skillTreeMovementThreshold) * Mathf.Sign(cellLocalPosition.x);
            float yPosition = Mathf.Max(0f, Mathf.Abs(cellLocalPosition.y) - _skillTreeMovementThreshold) * Mathf.Sign(cellLocalPosition.y);
            _destinationToMoveTo = -1 * new Vector2(xPosition, yPosition);

            UpdateSkillTreeUI();

            _canMove = false;
            Invoke(nameof(EnableMovement), _navigationCoolDown);
            GameManager.Instance.AudioManager.Play(_navigationSFX);
        }

        private void Update()
        {
            _skillTreeRect.localPosition = Vector2.Lerp(_skillTreeRect.localPosition, _destinationToMoveTo, SKILL_TREE_MOVEMENT_SPEED);
        }

        #endregion

        #region Update UI
        private void UpdateSkillTreeUI()
        {
            UpdateSkillName();
            UpdateSkillDescription();
            UpdatePreviewVideo();
            UpdateSkillPoints();
            UpdateSkillState();
            _viewModel.Update();
            //_UIDisplayer.UpdatePlayerSkillPointsCount(_currentPlayer.Upgrades.GetSkillPoints());
        }

        private void UpdateSkillName()
        {
            string name;

            if (_activeSkillCell.CellState == CellState.Unlocked || !_activeSkillCell.IsRandomSkill)
            {
                Color color = _upgradeColors[_activeSkillCell.UpgradeType];
                name = _activeSkillCell.GetName().Color(color);
            }
            else
            {
                name = _upgradeNames[_activeSkillCell.UpgradeType];
            }

            _UIDisplayer.UpdateName(name);
        }

        private void UpdateSkillDescription()
        {
            string description;

            if (_activeSkillCell.CellState == CellState.Unlocked || !_activeSkillCell.IsRandomSkill)
            {
                description = _activeSkillCell.GetDescription();
            }
            else
            {
                description = _upgradeDescriptioins[_activeSkillCell.UpgradeType];
            }

            _UIDisplayer.UpdateDescription(description);
        }

        private void UpdatePreviewVideo()
        {
            VideoClip videoClip = null;

            if (_activeSkillCell.SkillProfile != null && _activeSkillCell.SkillProfile.PreviewVideo != null)
            {
                videoClip = _activeSkillCell.SkillProfile.PreviewVideo;
            }

            _UIDisplayer.UpdatePreviewVideo(videoClip);
        }

        private void UpdateSkillPoints()
        {
            bool displaySkillPoints = _activeSkillCell.CellState != CellState.Unlocked;
            int playerPoints = _currentPlayer.Upgrades.GetSkillPoints();
            int requiredPoints = _activeSkillCell.RequiredSkillPoints;

            _UIDisplayer.UpdateSkillPoints(displaySkillPoints, playerPoints, requiredPoints);
        }

        private void UpdateSkillState()
        {
            _UIDisplayer.UpdateSkillState(_activeSkillCell.CellState);
        }
        #endregion

        private void OneClickSelect()
        {
            if (!IsCellSelectable())
            {
                _activeSkillCell.PlayOnWarningAnimation();
                return;
            }

            Select();
        }

        private IEnumerator SelectionHoldProcess()
        {
            if (!IsCellSelectable())
            {
                yield break;
            }


            GameManager.Instance.AudioManager.Play(_selectionSFX);
            _isHolding = true;

            float holdDuration = 1 / _activeSkillCell.HoldValue;
            float holdValue = _activeSkillCell.HoldValue;

            // get the last point the progress bar was at to start incrementing it from there
            float time = Mathf.Lerp(0f, holdDuration, 
                Mathf.InverseLerp(0f, holdDuration, _activeSkillCell.GetFillAmount()));

            // TODO: we don't need to do this every time an ability unlocks since the duration is a constant and won't change (do it through the animation)
            _activeSkillCell.SetHoldAnimationDurationMultiplier(holdDuration);

            // play scale up animation
            _activeSkillCell.PlayOnHoldStartedAnimation();

            _canMove = false;

            while (time < holdDuration)
            {
                time += Time.deltaTime;
                float progressBarAmount = time * holdValue;
                _activeSkillCell.SetProgressAmount(progressBarAmount);
                yield return null;
            }

            _activeSkillCell.PlayOnHoldStoppedAnimmation();

            _isHolding = false;
            _canMove = true;
            Select();
        }

        private IEnumerator SelectionReleaseHoldProcess()
        {
            if(_activeSkillCell.GetFillAmount() == 0f)
            {
                yield break;
            }

            float holdDuration = 1 / _activeSkillCell.HoldValue;
            float holdValue = _activeSkillCell.HoldValue;

            _activeSkillCell.PlayOnHoldStoppedAnimmation();

            // get the point the progress bar reached before the input stopped to backtrack from there
            float progress = Mathf.InverseLerp(0f, holdDuration, _activeSkillCell.GetFillAmount());
            float time = Mathf.Lerp(0f, holdDuration, progress);

            _canMove = false;

            while (time > 0)
            {
                time -= Time.deltaTime * SELECTION_HOLD_EMPTYING_SPEED_MULTIPLIER;
                float progressBarAmount = time * holdValue;
                _activeSkillCell.SetProgressAmount(progressBarAmount);
                yield return null;
            }

            _canMove = true;
        }

        private bool IsCellSelectable()
        {
            //bool isNotSelectable = _activeSkillCell.CellType == CellType.None;
            bool isUnlocked = _activeSkillCell.CellState == CellState.Unlocked;

            if (!IsOpened || isUnlocked)
            {
                GameManager.Instance.AudioManager.Play(_errorSFX);
                return false;
            }

            bool cellIsUnavailable = _activeSkillCell.CellState == CellState.Unavailable;
            bool playerHasEnoughSkillPoints = _currentPlayer.Upgrades.GetSkillPoints() >= _activeSkillCell.RequiredSkillPoints;

            if (cellIsUnavailable)
            {
                GameManager.Instance.AudioManager.Play(_errorSFX);
                //print($"Can't unlock this skill yet. Unlock the previous one first.");
                return false;
            }

            if (!playerHasEnoughSkillPoints)
            {
                GameManager.Instance.AudioManager.Play(_errorSFX);
                return false;
            }

            return true;
        }

        public override void Select()
        {
            _activeSkillCell.PlayOnUnlockedAnimation();

            if (_activeSkillCell.IsRandomSkill)
            {
                OnRandomSkillSelected();
            }
            else
            {
                _activeSkillCell.OnUnlockedUpgrade.GetUpgrade().ApplyUpgrade();
                GameManager.Instance.AudioManager.Play(_selectionSFX);

                _currentPlayer.Upgrades.AddSkillPoints(-_activeSkillCell.RequiredSkillPoints);
                _UIDisplayer.UpdatePlayerSkillPointsCount(_currentPlayer.Upgrades.GetSkillPoints());

                EnableSkillTreeHolder();
            }
        }

        private void OnRandomSkillSelected()
        {
            _activeSkillCell.Unhighlight();
         
            // disable input from the workshop manager (changing tabs or exiting the menu)
            _workShopNavigatable.IsOpened = false;

            PopulatePopUpWindowSkills();

            _randomSkillsMenu.SetCellIconColor(_upgradeColors[_activeSkillCell.UpgradeType]);

            _randomSkillsMenu.Open(PlayerIndex);

            _isInControl = false;

            _currentPlayer.Upgrades.AddSkillPoints(-_activeSkillCell.RequiredSkillPoints);
            _UIDisplayer.UpdatePlayerSkillPointsCount(_currentPlayer.Upgrades.GetSkillPoints());
        }

        private void PopulatePopUpWindowSkills()
        {
            List<SkillUpgrade> upgrades = _upgradesController.GetUpgrades(_activeSkillCell.UpgradeType, PlayerIndex);

            SkillUpgrade firstSkill = upgrades.RandomItem();
            SkillUpgrade secondSkill = upgrades.FindAll(s => s != firstSkill).RandomItem();

            _randomSkillsMenu.SetSkillProfiles(firstSkill, secondSkill);
        }

        public void EnableSkillTreeHolder()
        {
            _isInControl = true;

            _workShopNavigatable.IsOpened = true;

            _activeSkillCell.ChangeCellState(CellState.Unlocked);

            if (_activeSkillCell.UpgradeType == UpgradeTypes.SpecialUpgrade)
            {
                _activeSkillCell.SetColor(_upgradeColors[_activeSkillCell.UpgradeType]);
            }

            _activeSkillCell.SetColoredOutlineColor(_upgradeColors[_activeSkillCell.UpgradeType]);

            _activeSkillCell.Highlight();

            UpdateSkillTreeUI();
        }

        private void SetActiveCell(SkillTreeCell newCell, bool dehighlightPreviousCell = true)
        {
            if (dehighlightPreviousCell && _activeSkillCell != null)
            {
                _activeSkillCell.Unhighlight();
            }

            _activeSkillCell = newCell;
            _activeSkillCell.Highlight();
        }
        #endregion

        private void EnableMovement()
        {
            _canMove = true;
        }

        public override void DehighlightCells()
        {
            base.DehighlightCells();
            _activeSkillCell.Unhighlight();
        }

        public override void SetActiveCellAsFirstCell()
        {
            base.SetActiveCellAsFirstCell();

            _activeSkillCell = _centerCell;
            _activeSkillCell.Highlight();
        }

        public void CreateLineBetweenCells(SkillTreeCell firstCell, SkillTreeCell secondCell)
        {
            SkillTreeLine newLine = Instantiate(_linePrefab, _linesParent);
            firstCell.ConnectionLines.Add(newLine);
            newLine.ConnectCells(firstCell, secondCell);
        }

        public SkillTreeCell GetActiveCell()
        {
            return _activeSkillCell;
        }

        #region Editor Methods
#if UNITY_EDITOR
        public void GenerateConnectionLines()
        {
            ClearConnectionLines();
            CreateLinesForNextCells(_centerCell);
        }

        private void CreateLinesForNextCells(SkillTreeCell cell)
        {
            cell.Holder = this;

            if(cell.NextCells.Count == 0)
            {
                return;
            }

            if(cell.NextCells.Exists(c => c == null))
            {
                cell.NextCells.RemoveAll(c => c == null);
            }

            cell.ConnectionLines.Clear();

            for (int i = 0; i < cell.NextCells.Count; i++)
            {
                SkillTreeCell nextCell = cell.NextCells[i];

                SkillTreeLine newLine = (SkillTreeLine)PrefabUtility.InstantiatePrefab(_linePrefab, _linesParent);
                cell.ConnectionLines.Add(newLine);

                newLine.ConnectCells(cell, nextCell);

                CreateLinesForNextCells(cell.NextCells[i]);

                // TODO: remember that this is important!
                EditorUtility.SetDirty(newLine.gameObject);
            }
        }

        public void ClearConnectionLines()
        {
            for (int i = _linesParent.childCount - 1; i >= 0 ; i--)
            {
                DestroyImmediate(_linesParent.GetChild(i).gameObject);
            }
        }
#endif
        #endregion
    }
}
