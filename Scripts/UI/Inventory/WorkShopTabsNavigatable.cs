
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using TMPro;

namespace TankLike.UI.Workshop
{
    using Signifiers;
    using Inventory;
    using Utils;

    public class WorkShopTabsNavigatable : Navigatable, IInput
    {
        public Action OnWorkShopClosed { get; set; }

        [Header("References")]
        [SerializeField] private List<TabReferenceUI> _tabs;
        [SerializeField] private GameObject _content;
        [SerializeField] private UIActionSignifiersController _menuActionSignifiersController;
        [SerializeField] private TextMeshProUGUI _switchTabLeftText;
        [SerializeField] private TextMeshProUGUI _switchTabRightText;

        private TabReferenceUI _selectedTab;
        private int _selectedTabIndex = 0;

        private const string RETURN_ACTION_TEXT = "Return";

        #region IManager
        public override void SetUp()
        {
            _selectedTabIndex = 0;
            _selectedTab = _tabs[_selectedTabIndex];

            // deactivate all the tabs
            foreach (TabReferenceUI tab in _tabs)
            {
                tab.SetUp();
            }

            _content.SetActive(false);
            IsActive = true;

            base.SetUp();

            _tabs.ForEach(t => t.Navigator.SetUp());
            _tabs.ForEach(t => t.Navigator.SetUpActionSignifiers(_menuActionSignifiersController));
        }

        public override void Dispose()
        {
            IsActive = false;
            _tabs.ForEach(t => t.Navigator.Dispose());
        }
        #endregion

        #region Open and Close
        public override void Open(int playerIndex = 0)
        {
            base.Open(playerIndex);

            SetPlayerIndex(playerIndex);

            SetUpInput(PlayerIndex);

            // load all the tabs' content
            _tabs.ForEach(t => t.Navigator.Load(playerIndex));

            // highlight the selected tab
            _selectedTab.Tab.HighLight();

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
        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsOpened)
            {
                return;
            }

            OnWorkShopClosed?.Invoke();
            _selectedTab.Navigator.Close(PlayerIndex);

            DisposeInput(PlayerIndex);

            _content.SetActive(false);

            base.Close(playerIndex);
        }
        #endregion

        #region Input Set up
        public void SetUpInput(int playerIndex)
        {
            GameManager.Instance.InputManager.DisableInputs();
            GameManager.Instance.InputManager.EnableUIInput(playerIndex);

            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Cancel.name).performed += ReturnAction;
            UIMap.FindAction(c.UI.Tab_Left.name).performed += SwitchTabsLeft;
            UIMap.FindAction(c.UI.Tab_Right.name).performed += SwitchTabsRight;
            UIMap.FindAction(c.UI.Exit.name).performed += CloseWorkShop;
            
            SetUpSignifiers();
        }

        public override void SetUpSignifiers()
        {
            _menuActionSignifiersController.ClearAllSignifiers();

            PlayerInputActions c = InputManager.Controls;
            int schemeIndex = GameManager.Instance.InputManager.GetInputSchemeIndex(PlayerIndex);

            AddReturnSignifier();

            int switchTabLeftIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Tab_Left.name, schemeIndex);
            _switchTabLeftText.text = Helper.GetInputIcon(switchTabLeftIconIndex);

            int switchTabRightIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Tab_Right.name, schemeIndex);
            _switchTabRightText.text = Helper.GetInputIcon(switchTabRightIconIndex);
        }

        private void AddReturnSignifier()
        {
            PlayerInputActions c = InputManager.Controls;
            int schemeIndex = GameManager.Instance.InputManager.GetInputSchemeIndex(PlayerIndex);

            // add the actions that are mutual between windows to the signifiers controller
            int returnActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Cancel.name, schemeIndex);
            _menuActionSignifiersController.DisplaySignifier(RETURN_ACTION_TEXT, Helper.GetInputIcon(returnActionIconIndex));
            _menuActionSignifiersController.SetLastSignifierAsParent();
        }

        public void DisposeInput(int playerIndex)
        {
            GameManager.Instance.InputManager.EnableInput(ActionMap.Player);
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Cancel.name).performed -= ReturnAction;
            UIMap.FindAction(c.UI.Tab_Left.name).performed -= SwitchTabsLeft;
            UIMap.FindAction(c.UI.Tab_Right.name).performed -= SwitchTabsRight;
            UIMap.FindAction(c.UI.Exit.name).performed -= CloseWorkShop;

            _menuActionSignifiersController.ClearAllSignifiers();
        }

        private void ReturnAction(InputAction.CallbackContext _)
        {
            Return();
        }

        private void SwitchTabsLeft(InputAction.CallbackContext _)
        {
            SwitchTabs(Direction.Left);
        }

        private void SwitchTabsRight(InputAction.CallbackContext _)
        {
            SwitchTabs(Direction.Right);
        }

        private void CloseWorkShop(InputAction.CallbackContext _)
        {
            Close(PlayerIndex);
        }
        #endregion

        #region Input Methods

        #region Navigation
        public void SwitchTabs(Direction direction)
        {
            if (!IsOpened)
            {
                return;
            }

            _menuActionSignifiersController.ClearAllSignifiers();
            AddReturnSignifier();

            // dehighlight the selected tab head
            _selectedTab.Tab.Dehighlight();

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
            _selectedTab.Tab.HighLight();

            // set up the tab highlighted
            _selectedTab.Navigator.Open(PlayerIndex);
            _selectedTab.Navigator.SetActiveCellAsFirstCell();

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

        #region Return
        public override void Return()
        {
            base.Return();

            if (!IsOpened)
            {
                return;
            }

            Close();
        }
        #endregion

        #endregion
    }
}
