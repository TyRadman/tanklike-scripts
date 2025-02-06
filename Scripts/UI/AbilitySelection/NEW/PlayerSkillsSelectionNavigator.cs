using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TankLike.Combat.SkillTree
{
    using Attributes;
    using System.Linq.Expressions;
    using TankLike.Sound;
    using UI;
    using UI.Signifiers;
    using UnitControllers;
    using Utils;

    public class PlayerSkillsSelectionNavigator : Navigatable, IInput
    {
        [SerializeField, AllowCreationIfNull(Path = "Assets")] private PlayerStartingSkillsPackage _playerStartingSkillsPackage;

        public System.Action OnPlayerReady { get; internal set; }
        public System.Action OnPlayerNotReady { get; internal set; }
        public System.Action<int> OnPlayerExitted { get; internal set; }

        [SerializeField] private Audio _navigationSFX;
        [SerializeField] private Audio _selectionSFX;
        [SerializeField] private Audio _errorSFX;
        [SerializeField] private GameObject _content;
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private SingleSkillSelectionSection _weaponSelectionSection;
        [SerializeField] private SingleSkillSelectionSection _chargeAttackSelectionSection;
        [SerializeField] private SingleSkillSelectionSection _superAbilitySelectionSection;
        [SerializeField] private PlayerSkillsSelectionReadyButton _readyButton;
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private TextMeshProUGUI _skillDescriptionText;
        [SerializeField] private TextMeshProUGUI _skillCategoryDescriptionText;
        [SerializeField] private TextMeshProUGUI _upNavigationSignifiersText;
        [SerializeField] private TextMeshProUGUI _downNavigationSignifiersText;
        [SerializeField] private TextMeshProUGUI _signifierText;
        [SerializeField] private Image _deviceIcon;

        private Dictionary<SingleSkillSelectionSection, UpgradeTypes> _skillSelectorTypes;

        private string _skillChangeSignifierText;
        private string _skillNavigationSignifierText;
        private string _readyButtonSignifierText;

        private ICellSelectable _activeSelectionSection;
        private bool _isReady = false;

        private const int MAX_DESCRIPTION_LINES_COUNT = 5;

        #region IManager
        public override void SetUp()
        {
            base.SetUp();

            _skillSelectorTypes = new Dictionary<SingleSkillSelectionSection, UpgradeTypes>
            {
                { _weaponSelectionSection, UpgradeTypes.BaseWeapon },
                { _chargeAttackSelectionSection, UpgradeTypes.ChargeAttack },
                { _superAbilitySelectionSection, UpgradeTypes.SuperAbility }
            };

            for (int i = 0; i < _playerStartingSkillsPackage.WeaponSkills.Count; i++)
            {
                SkillProfile skill = _playerStartingSkillsPackage.WeaponSkills[i];
                skill.SkillHolder.PopulateSkillProperties();
                _weaponSelectionSection.AddSkillProfile(skill);
            }

            for (int i = 0; i < _playerStartingSkillsPackage.ChargeAttackSkills.Count; i++)
            {
                SkillProfile skill = _playerStartingSkillsPackage.ChargeAttackSkills[i];
                skill.SkillHolder.PopulateSkillProperties();
                _chargeAttackSelectionSection.AddSkillProfile(skill);
            }

            for (int i = 0; i < _playerStartingSkillsPackage.SuperAbilitySkills.Count; i++)
            {
                SkillProfile skill = _playerStartingSkillsPackage.SuperAbilitySkills[i];
                skill.SkillHolder.PopulateSkillProperties();
                _superAbilitySelectionSection.AddSkillProfile(skill);
            }

            _weaponSelectionSection.Initiate();
            _chargeAttackSelectionSection.Initiate();
            _superAbilitySelectionSection.Initiate();
            _readyButton.Initiate();

            _activeSelectionSection = _weaponSelectionSection;

            _content.SetActive(false);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #region Open and close
        public override void Open(int playerIndex)
        {
            base.Open(playerIndex);

            _content.SetActive(true);
            //_lobbyPrompt.SetActive(false);

            GameManager.Instance.InputManager.EnableUIInput();
            SetUpInput(playerIndex);

            if (_activeSelectionSection is not SingleSkillSelectionSection skillsSelectionSection || skillsSelectionSection != _weaponSelectionSection)
            {
                _activeSelectionSection.Unhighlight();
                _activeSelectionSection = _weaponSelectionSection;
            }

            _activeSelectionSection.Highlight();

            _playerNameText.text = $"Player {playerIndex + 1}";
            UpdateDetails(_playerStartingSkillsPackage.GetStartingWeapon(PlayerIndex));
            DisplayChangeSkillSignifiers();
            _skillCategoryDescriptionText.text = _weaponSelectionSection.GetSkillCategoryDescription();

            SetInitialSkills();
        }

        public override void Close(int playerIndex)
        {
            base.Close(playerIndex);

            _content.SetActive(false);
            //_lobbyPrompt.SetActive(true);

            DisposeInput(playerIndex);
        }
        #endregion

        #region IInput
        private InputActionMap _UIMap;

        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            _UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            _UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;
            _UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;
            _UIMap.FindAction(c.UI.Navigate_Up.name).performed += NavigateUp;
            _UIMap.FindAction(c.UI.Navigate_Down.name).performed += NavigateDown;

            _UIMap.FindAction(c.UI.Cancel.name).performed += PerformCancellation;
            _UIMap.FindAction(c.UI.Submit.name).performed += PerformSelection;

            SetUpActionSignifiers(null);
        }

        public override void SetUpActionSignifiers(ISignifierController signifierController)
        {
            base.SetUpActionSignifiers(signifierController);

            PlayerInputActions c = InputManager.Controls;

            int schemeIndex = GameManager.Instance.InputManager.GetInputSchemeIndex(PlayerIndex);

            // left arrow signifier
            int leftIconIndex = GameManager.Instance.InputManager.GetSpriteIndexByScheme(c.UI.Navigate_Left.name, schemeIndex);
            string leftInputText = Helper.GetInputIcon(leftIconIndex);

            _weaponSelectionSection.SetLeftArrowInputText(leftInputText);
            _chargeAttackSelectionSection.SetLeftArrowInputText(leftInputText);
            _superAbilitySelectionSection.SetLeftArrowInputText(leftInputText);

            // right arrow signifier
            int rightIconIndex = GameManager.Instance.InputManager.GetSpriteIndexByScheme(c.UI.Navigate_Right.name, schemeIndex);
            string rightInputText = Helper.GetInputIcon(rightIconIndex);

            _weaponSelectionSection.SetRightArrowInputText(rightInputText);
            _chargeAttackSelectionSection.SetRightArrowInputText(rightInputText);
            _superAbilitySelectionSection.SetRightArrowInputText(rightInputText);

            // ready button signifier
            int readyButtonIconIndex = GameManager.Instance.InputManager.GetSpriteIndexByScheme(c.UI.Submit.name, schemeIndex);
            string readyButtonInputText = Helper.GetInputIcon(readyButtonIconIndex);

            _readyButton.UpdateSignifierText(readyButtonInputText);

            // up/down navigations
            int navigationUpInputIndex = GameManager.Instance.InputManager.GetSpriteIndexByScheme(c.UI.Navigate_Up.name, schemeIndex);
            int navigationDownInputIndex = GameManager.Instance.InputManager.GetSpriteIndexByScheme(c.UI.Navigate_Down.name, schemeIndex);
            string navigationUpText = Helper.GetInputIcon(navigationUpInputIndex);
            string navigationDownText = Helper.GetInputIcon(navigationDownInputIndex);
            _upNavigationSignifiersText.text = navigationUpText;
            _downNavigationSignifiersText.text = navigationDownText;

            _skillChangeSignifierText = $"{leftInputText}  {"/".Size(10)}  {rightInputText} {"Change Skill".Size(10)}";
            _skillNavigationSignifierText = $"{navigationUpText}  {"/".Size(10)}  {navigationDownText} {"Navigate".Size(10)}";
            _readyButtonSignifierText = $"{readyButtonInputText} {"Confirm".Size(10)}";
            _signifierText.text = _skillNavigationSignifierText;

            Invoke(nameof(FixChild), 0.1f);
        }

        private void FixChild()
        {
            RectTransform rect = _signifierText.transform.GetChild(0).GetComponent<RectTransform>();
            Vector2 offset = rect.offsetMax;
            offset.y += 6;
            rect.offsetMax = offset;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;

            _UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            _UIMap.FindAction(c.UI.Navigate_Right.name).performed -= NavigateRight;
            _UIMap.FindAction(c.UI.Navigate_Left.name).performed -= NavigateLeft;
            _UIMap.FindAction(c.UI.Navigate_Up.name).performed -= NavigateUp;
            _UIMap.FindAction(c.UI.Navigate_Down.name).performed -= NavigateDown;

            _UIMap.FindAction(c.UI.Cancel.name).performed -= PerformCancellation;
            _UIMap.FindAction(c.UI.Submit.name).performed -= PerformSelection;

            _UIMap = null;
        }
        #endregion

        #region Input Methods
        public override void NavigateLeft(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Left);
        }
        public override void NavigateRight(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Right);
        }
        public override void NavigateUp(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Up);
        }
        public override void NavigateDown(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Down);
        }

        private void PerformSelection(InputAction.CallbackContext context)
        {
            if (_activeSelectionSection is PlayerSkillsSelectionReadyButton button && !_isReady)
            {
                _isReady = true;
                OnPlayerReady?.Invoke();
                button.OnButtonClicked();
                GameManager.Instance.AudioManager.Play(_selectionSFX);
            }
            else if (!_isReady)
            {
                _activeSelectionSection.Unhighlight();
                _activeSelectionSection = _readyButton;
                _activeSelectionSection.Highlight();
                DisplayReadyButtonSignifiers();
                GameManager.Instance.AudioManager.Play(_selectionSFX);
            }
        }

        private void PerformCancellation(InputAction.CallbackContext context)
        {
            GameManager.Instance.AudioManager.Play(_errorSFX);

            if (_activeSelectionSection is PlayerSkillsSelectionReadyButton button && _isReady)
            {
                _isReady = false;
                OnPlayerNotReady?.Invoke();
                button.OnButtonUnclicked();
            }
            else if (!_isReady)
            {
                var playerInputManager = FindObjectOfType<PlayerInputManager>();

                if (playerInputManager != null)
                {
                    Destroy(GameManager.Instance.InputManager.GetHandler(PlayerIndex).gameObject);
                }
            }
        }
        #endregion

        public override void Navigate(Direction direction)
        {
            if (_isReady)
            {
                return;
            }

            GameManager.Instance.AudioManager.Play(_navigationSFX);

            if (direction is Direction.Left or Direction.Right)
            {
                NavigateHorizontally(direction);
            }
            else
            {
                NavigateVertically(direction);
            }
        }

        private void NavigateHorizontally(Direction direction)
        {
            if (_activeSelectionSection is SingleSkillSelectionSection selectionSection)
            {
                selectionSection.NavigateHorizontally(direction);
                var skillProfile = selectionSection.GetActiveSkill();
                _playerStartingSkillsPackage.SetProfile(skillProfile, PlayerIndex, _skillSelectorTypes[selectionSection]);

                UpdateDetails(skillProfile);
            }
        }

        private void NavigateVertically(Direction direction)
        {
            var nextCell = _activeSelectionSection.Navigate(direction);

            if (nextCell == null)
            {
                Debug.LogError($"No cell found at direction {direction}");
                return;
            }

            _activeSelectionSection.Unhighlight();
            _activeSelectionSection = nextCell;
            _activeSelectionSection.Highlight();

            if (_activeSelectionSection is SingleSkillSelectionSection selectionSection)
            {
                var skillProfile = selectionSection.GetActiveSkill();
                _playerStartingSkillsPackage.SetProfile(skillProfile, PlayerIndex, _skillSelectorTypes[selectionSection]);
                UpdateDetails(skillProfile);
                _skillCategoryDescriptionText.text = selectionSection.GetSkillCategoryDescription();

                DisplayChangeSkillSignifiers();
            }
            else
            {
                DisplayReadyButtonSignifiers();
            }
        }

        private void DisplayChangeSkillSignifiers()
        {
            _signifierText.text = $"{_skillNavigationSignifierText} ,  {_skillChangeSignifierText}";
        }

        private void DisplayReadyButtonSignifiers()
        {
            _signifierText.text = $"{_skillNavigationSignifierText} ,  {_readyButtonSignifierText}";
        }

        private void UpdateDetails(SkillProfile skillProfile)
        {
            if (skillProfile == null)
            {
                Debug.LogError("No skill profile found");
            }

            _videoPlayer.clip = skillProfile.PreviewVideo;
            _videoPlayer.Play();

            _skillDescriptionText.textInfo.Clear();
            _skillDescriptionText.text = skillProfile.SkillHolder.GetDescription();
            _skillDescriptionText.ForceMeshUpdate();
            int lines = _skillDescriptionText.textInfo.lineCount;

            if (lines < MAX_DESCRIPTION_LINES_COUNT)
            {
                for (int i = 0; i < MAX_DESCRIPTION_LINES_COUNT - lines; i++)
                {
                    _skillDescriptionText.text += "\n";
                }
            }

            _skillDescriptionText.text += skillProfile.SkillHolder.GetPropertiesText();

        }

        internal void SetDeviceIcon(Sprite deviceIcon)
        {
            _deviceIcon.sprite = deviceIcon;
        }

        private void SetInitialSkills()
        {
            _playerStartingSkillsPackage.SetStartingWeapon(_weaponSelectionSection.GetActiveSkill(), PlayerIndex);
            _playerStartingSkillsPackage.SetStartingChargeAttack(_chargeAttackSelectionSection.GetActiveSkill(), PlayerIndex);
            _playerStartingSkillsPackage.SetStartingSuperAbility(_superAbilitySelectionSection.GetActiveSkill(), PlayerIndex);
        }
    }
}
