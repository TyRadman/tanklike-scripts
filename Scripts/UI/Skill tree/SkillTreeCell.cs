using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Combat.SkillTree
{
    using UI.SkillTree;
    using Combat.SkillTree.Upgrades;
    using UnitControllers;
    using System;
    using Attributes;

    public class SkillTreeCell : UICell, ICellSelectable, IPresetable
    {
        public int RequiredSkillPoints { get; private set; } = 1;
        public SkillProfile SkillProfile { get; private set; }
        public List<SkillTreeLine> ConnectionLines { get; set; } = new List<SkillTreeLine>();
        
        [Tooltip("Unlocked: the player already has the skill.\nLocked: the player can unlock the skill with skill points.\nUnavailable: the player can unlock the ability only when the ability before it in the skill tree is unlocked.\nNone: can never be unlocked (mostly non-skill cells.)")]
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        [field: SerializeField] public List<SkillTreeCell> NextCells { get; private set; } = new List<SkillTreeCell>();
        [field: SerializeField] public UpgradeTypes UpgradeType { get; set; }
        [field: SerializeField] public CellState CellState { get; private set; } = CellState.None;
        [field: SerializeField] public float HoldValue { get; set; } = 1.25f;

        public List<SkillsConnectedCell> ConnectedCells = new List<SkillsConnectedCell>();
        public SkillUpgrade OnUnlockedUpgrade { get; private set; }
        public bool IsRandomSkill { get; set; } = true;

        [SerializeField] private Transform _content;
        [SerializeField] private GameObject _overlayImage;
        [SerializeField] private Image _cellIconImage;
        [SerializeField] private Image _highlightImage;
        [SerializeField] private Image _coverImage;
        [SerializeField] private Image _progressBar;
        [SerializeField] private Image _unavailableCover;
        [SerializeField] private Image _lockImage;
        [SerializeField, Required] private SkillTreeCellPreset _preset;
        [SerializeField] private List<SkillTreeLine> _previousLines = new List<SkillTreeLine>();

        [Header("Animations")]
        [SerializeField] private Animator _animator;

        private string _cellName;
        private string _cellDescription;
        private int _parentsToUnlockCount = 0;

        private readonly int _onSelectedParamHash = Animator.StringToHash("OnSelected");
        private readonly int _onDeselectedParamHash = Animator.StringToHash("OnDeselected");
        private readonly int _onUnlockedParamHash = Animator.StringToHash("OnUnlocked");
        private readonly int _onWarningParamHash = Animator.StringToHash("OnWarning");
        private readonly int _onHoldStartedParamHash = Animator.StringToHash("OnHoldStarted");
        private readonly int _onHoldStoppedParamHash = Animator.StringToHash("OnHoldStopped");
        private readonly int _holdDurationMultiplierParamHash = Animator.StringToHash("HoldDurationMultiplayer");

        public static Color LOCKED_PATH_COLOR = Color.white;
        public static Color UNLOCKED_PATH_COLOR = Color.white;
        public static Color UNAVAILABLE_PATH_COLOR = Color.white;

#if UNITY_EDITOR
        public Vector2 LastPosition { get; set; }
        public SkillTreeHolder Holder { get; set; }
#endif

        public void SetColor(Color iconColor)
        {
            _cellIconImage.color = iconColor;
        }

        public override void SetUp()
        {
            if (SkillProfile != null)
            {
                SkillProfile.SkillHolder.PopulateSkillProperties();
                SetIcon(SkillProfile.SkillHolder.GetIcon());
            }

            SetUpRandomSkill();
            NextCells.ForEach(c => c.AddParentToUnlock());

            if(CellState is CellState.Unlocked or CellState.Locked)
            {
                _coverImage.enabled = false;
            }
            else
            {
                _coverImage.enabled = true;
            }
        }

        public void SetUpRandomSkill()
        {
            _overlayImage.SetActive(true);
        }

        public void SetUpLockIcon(Sprite icon)
        {
            _lockImage.sprite = icon;
        }

        public void RemoveAllOverlays()
        {
            _overlayImage.SetActive(false);
            _unavailableCover.enabled = false;
        }

        #region Cell State Change
        public void ChangeCellState(CellState newState)
        {
            if (CellState == newState)
            {
                return;
            }

            CellState = newState;

            switch (CellState)
            {
                case CellState.Unavailable:
                    OnUnavailableCell();
                    break;
                case CellState.Locked:
                    OnLockedCell();
                    break;
                case CellState.Unlocked:
                    OnUnlockedCell();
                    break;
            }
        }

        public void OnUnavailableCell()
        {
            _coverImage.enabled = true;
            _unavailableCover.enabled = true;
        }

        public void OnLockedCell()
        {
            _coverImage.enabled = false;
            _unavailableCover.enabled = false;

            _previousLines?.ForEach(l => l.PlayLockedAnimation());
        }

        private void OnUnlockedCell()
        {
            RemoveAllOverlays();

            _coverImage.enabled = false;
            _previousLines?.ForEach(l => l.PlayUnlockedAnimation());

            // set the available cells that are connected to this cell as locked
            for (int i = 0; i < ConnectedCells.Count; i++)
            {
                SkillsConnectedCell nextCell = ConnectedCells[i];

                if(NextCells.Exists(c => c == nextCell.Cell))
                {
                    ((SkillTreeCell)nextCell.Cell).OnParentUnlocked();
                }
            }
        }
        #endregion

        #region UI related methods
        public override void SetIcon(Sprite icon)
        {
            _cellIconImage.sprite = icon;
        }

        public override void Highlight()
        {
            _highlightImage.enabled = true;
            _animator.SetTrigger(_onSelectedParamHash);
        }

        public override void Unhighlight()
        {
            _highlightImage.enabled = false;
            _animator.SetTrigger(_onDeselectedParamHash);
        }

        public void SetProgressAmount(float amount)
        {
            _progressBar.fillAmount = amount;
        }

        public float GetFillAmount()
        {
            return _progressBar.fillAmount;
        }

        public void PlayOnUnlockedAnimation()
        {
            _animator.SetTrigger(_onUnlockedParamHash);
        }
        
        public void PlayOnWarningAnimation()
        {
            _animator.SetTrigger(_onWarningParamHash);
        }

        public void PlayOnHoldStartedAnimation()
        {
            _animator.SetTrigger(_onHoldStartedParamHash);
        }

        public void PlayOnHoldStoppedAnimmation()
        {
            _animator.SetTrigger(_onHoldStoppedParamHash);
        }

        public void SetHoldAnimationDurationMultiplier(float holdDuration)
        {
            _animator.SetFloat(_holdDurationMultiplierParamHash, holdDuration);
        }

        [SerializeField] private Image _iconMaskImage;
        [SerializeField] private Image _overlayImageComponent;
        [SerializeField] private Image _outlineImage;
        [SerializeField] private Image _coloredOutline;

        public void SetOutline(Sprite icon)
        {
            _highlightImage.sprite = icon;
            _progressBar.sprite = icon;
            _iconMaskImage.sprite = icon;
            _coverImage.sprite = icon;
            _overlayImageComponent.sprite = icon;
            _outlineImage.sprite = icon;
            _coloredOutline.sprite = icon;
        }

        public void SetColoredOutlineColor(Color color)
        {
            _coloredOutline.color = color;
        }
        #endregion

        #region Data Getters and Setters
        public void SetName(string name)
        {
            _cellName = name;
        }

        public string GetName()
        {
            return _cellName;
        }

        public void SetDescription(string description)
        {
            _cellDescription = description;
        }

        public string GetDescription()
        {
            return _cellDescription;
        }
        #endregion

        public void AddPreviousCellAndLine(SkillTreeLine line)
        {
            _previousLines.RemoveAll(l => l == null);

            _previousLines.Add(line);
        }

        public void SetLockedImageColor(Color color)
        {
            _lockImage.color = color;
        }

        #region Utilities
        public void SetParentsToUnlockCount(int unlocksRequiredCount)
        {
            _parentsToUnlockCount = unlocksRequiredCount;
        }

        public void OnParentUnlocked()
        {
            _parentsToUnlockCount--;

            if(_parentsToUnlockCount <= 0)
            {
                ChangeCellState(CellState.Locked);
            }
        }

        public void AddParentToUnlock()
        {
            _parentsToUnlockCount++;
        }

        public void AddNextCell(SkillTreeCell cell)
        {
            NextCells.Add(cell);
        }

        public void AddConnectedCell(Direction direction, SkillTreeCell cell)
        {
            SkillsConnectedCell connectedCell = ConnectedCells.Find(c => c.CellDirection == direction);

            if (connectedCell != null)
            {
                connectedCell.Cell = cell;
            }
            else
            {
                ConnectedCells.Add(new SkillsConnectedCell()
                {
                    Cell = cell,
                    CellDirection = direction
                });
            }
        }

        public void SetState(CellState state)
        {
            CellState = state;
        }

        public void SetSkillPointCost(int cost)
        {
            RequiredSkillPoints = cost;
        }

        internal void SetOutline(object outlineIcon)
        {
            throw new NotImplementedException();
        }

        public ICellSelectable Navigate(Direction direction)
        {
            throw new NotImplementedException();
        }

        internal float GetProgressAmount()
        {
            return _progressBar.fillAmount;
        }
        #endregion

        #region Preset
        public void ApplyPreset()
        {
            ApplyPreset(_preset);
        }

        public void ApplyPreset(IPreset preset)
        {
            if(preset is SkillTreeCellPreset cellPreset)
            {
                _content.localScale = Vector3.one * cellPreset.ContentScale;
                _progressBar.fillAmount = cellPreset.ProgressAmount;
            }
        }
        #endregion

        public void SetUpgrade(SkillUpgrade upgrade)
        {
            if(upgrade == null)
            {
                Debug.LogError("Upgrade is null.");
                return;
            }

            OnUnlockedUpgrade = upgrade;

            SetName(OnUnlockedUpgrade.GetUpgradeMainInfo().Name);
            SetDescription(OnUnlockedUpgrade.GetUpgradeMainInfo().Description);
            SetIcon(OnUnlockedUpgrade.GetUpgradeMainInfo().Icon);
        }
    }

    public enum CellState
    {
        Unavailable = 0, Locked = 1, Unlocked = 2, None = 3
    }
}