using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace TankLike.UI.Inventory
{
    using Signifiers;
    using TankLike.Sound;
    using Utils;

    /// <summary>
    /// The inventory of the player that holds the map, skill tree, tools, quests etc.
    /// </summary>
    public class InventoryController : Navigatable
    {
        public enum InputState
        {
            TabHeads, TabContent
        }

        private TabReferenceUI _selectedTab;
        [SerializeField] private int _selectedTabIndex = 0;

        [Header("References")]
        [SerializeField] private List<TabReferenceUI> _tabs;
        [SerializeField] private List<TabReferenceUI> _oldTabss;
        [SerializeField] private GameObject _content;
        [SerializeField] private TextMeshProUGUI _switchLeftKeyText;
        [SerializeField] private TextMeshProUGUI _switchRightKeyText;
        [SerializeField] private UIActionSignifiersController _menuActionSignifiersController;

        [Header("Tab Heads Settings")]
        [SerializeField] private TabActivationSettings _enableTabSettings;
        [SerializeField] private TabActivationSettings _disableTabSettings;
        [SerializeField] private TabActivationSettings _selectedTabSettings;

        private const string RETURN_ACTION_TEXT = "Return";

        public void SetReferences()
        {
            _selectedTabIndex = 0;
            _selectedTab = _tabs[_selectedTabIndex];
            _content.SetActive(false);

            // deactivate all the tabs
            foreach (TabReferenceUI tab in _tabs)
            {
                tab.Tab.HighLight(_disableTabSettings);
                tab.Tab.SetName(tab.TabName);
                tab.Navigator.gameObject.SetActive(false);
            }
        }

        #region IManager
        public override void SetUp()
        {
            base.SetUp();

            IsActive = true;

            // close the tab (in case it was opened for editing)
            Close();
            _tabs.ForEach(t => t.Navigator.SetUp());
        }

        public override void Dispose()
        {
            base.Dispose();

            IsActive = false;

            _tabs.ForEach(t => t.Navigator.Dispose());
        }
        #endregion


        #region Open and Close
        public override void Open(int playerIndex = 0)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            SetPlayerIndex(playerIndex);

            // disable gameplay and enable UI input
            GameManager.Instance.InputManager.EnableUIInput();

            // load all the tabs' content
            _tabs.ForEach(t => t.Navigator.Load(playerIndex));

            // highlight the selected tab
            _selectedTab.Tab.HighLight(_enableTabSettings);

            // enable the content
            _content.SetActive(true);

            // display the content of the tab
            if (_selectedTab.Navigator != null)
            {
                _selectedTab.Navigator.gameObject.SetActive(true);
            }

            // set up the tab highlighted
            _selectedTab.Navigator.Open(PlayerIndex);
            _selectedTab.Navigator.SetActiveCellAsFirstCell();

            // change the tab's text size and color to indicate that the control went over to the content of the tab
            _selectedTab.Tab.HighLight(_selectedTabSettings);

            // display input signifiers
            SetUpSignifiers();

            GameManager.Instance.PauseMenuManager.PauseGameplay();

            base.Open(playerIndex);
        }

        public override void SetUpSignifiers()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            if (_menuActionSignifiersController == null)
            {
                Debug.LogError($"No signifiers controller reference at {gameObject.name}");
                return;
            }

            base.SetUpSignifiers();
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(PlayerIndex, ActionMap.UI);

            if (UIMap == null)
            {
                Debug.LogError($"Input map {ActionMap.UI} reference is null");
                return;
            }

            _menuActionSignifiersController.SetUp();

            int returnActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Cancel.name, PlayerIndex);
            //string inventoryReturnActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Inventory.name, PlayerIndex).ToString();

            _menuActionSignifiersController.DisplaySignifier(RETURN_ACTION_TEXT, Helper.GetInputIcon(returnActionIconIndex));
            _menuActionSignifiersController.SetLastSignifierAsParent();

            int leftKeyIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(InputManager.Controls.UI.Tab_Left.name, PlayerIndex);
            int rightKeyIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(InputManager.Controls.UI.Tab_Right.name, PlayerIndex);
            _switchLeftKeyText.text = Helper.GetInputIcon(leftKeyIconIndex);
            _switchRightKeyText.text = Helper.GetInputIcon(rightKeyIconIndex);
        }

        private void ResetSignifiers()
        {
            _menuActionSignifiersController.ClearAllSignifiers();
        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            if (!IsOpened)
            {
                return;
            }

            ResetSignifiers();
            GameManager.Instance.InputManager.EnablePlayerInput();

            GameManager.Instance.PauseMenuManager.UnpauseGameplay();
            _content.SetActive(false);
            ResetTabs();
            base.Close(playerIndex);
        }
        #endregion

        #region Navigation
        public void SwitchTabs(Direction direction)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            if (!IsOpened)
            {
                return;
            }

            // dehighlight the selected tab head
            _selectedTab.Tab.HighLight(_disableTabSettings);

            // deactivate the tab's content
            if (_selectedTab.Navigator != null)
            {
                _selectedTab.Navigator.gameObject.SetActive(false);
            }

            // dehighlight the previously selected tab
            _selectedTab.Navigator.Close(PlayerIndex);
            // select the next tab depending on the direction
            _selectedTabIndex = Helper.AddInRange(direction == Direction.Right ? 1 : -1, _selectedTabIndex, 0, _tabs.Count - 1);
            _selectedTab = _tabs[_selectedTabIndex];
            _selectedTab.Tab.HighLight(_enableTabSettings);

            // Open the highlighted tab
            _selectedTab.Navigator.Open(PlayerIndex);
            _selectedTab.Navigator.SetActiveCellAsFirstCell();

            // change the tab's text size and color to indicate that the control went over to the content of the tab
            _selectedTab.Tab.HighLight(_selectedTabSettings);
            _selectedTab.Navigator.IsOpened = true;

            // display the content of the tab
            if (_selectedTab.Navigator != null)
            {
                _selectedTab.Navigator.gameObject.SetActive(true);
            }

            // Play navigate menu audio
            GameManager.Instance.AudioManager.Play(GameManager.Instance.AudioManager.UIAudio.NavigateMenuAudio);
        }
        #endregion

        private void ResetTabs(int playerIndex = 0)
        {

            if (!IsOpened)
            {
                return;
            }

            // dehighlight the selected tab head
            _selectedTab.Tab.HighLight(_disableTabSettings);
            // deactivate the tab's content
            if (_selectedTab.Navigator != null)
            {
                _selectedTab.Navigator.gameObject.SetActive(false);
            }

            // dehighlight the previously selected tab
            _selectedTab.Navigator.Close(PlayerIndex);
            // select the next tab depending on the direction
            _selectedTabIndex = 0;
            _selectedTab = _tabs[_selectedTabIndex];
            _selectedTab.Tab.HighLight(_enableTabSettings);

            // change the tab's text size and color to indicate that the control went over to the content of the tab
            _selectedTab.Tab.HighLight(_selectedTabSettings);

            // display the content of the tab
            if (_selectedTab.Navigator != null)
            {
                _selectedTab.Navigator.gameObject.SetActive(true);
            }
        }
    }
}
