using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UI
{
    using Signifiers;
    using Combat;
    using Sound;
    using UnitControllers;
    using Utils;
    using static UnitControllers.TankTools;

    public class ToolsNavigator : Navigatable, IInput, IManager
    { 
        // the state of the select button
        public enum SelectState
        {
            Select, Buy, Sell
        }

        [Header("Modifiers")]
        [SerializeField] private int _toolsToDisplayCount = 5;
        [SerializeField] private bool _showShopItems = true;

        [Header("References")]
        [SerializeField] private ShopToolSelectableCellUI _firstCell;
        [SerializeField] private PlayerShopInfoUI _currentPlayerInfo;
        [SerializeField] private GameObject _shopParent;
        [SerializeField] private UIActionSignifiersController _menuActionSignifiersController;
        [SerializeField] private List<ShopToolSelectableCellUI> _allShopCells;

        [Header("Audio")]
        [SerializeField] private Audio _onSwitchAudio;
        [SerializeField] private Audio _onSelectAudio;
        [SerializeField] private Audio _onErrorAudio;

        // The cell that will be selected by default when the shop is opened
        private SelectState _shopState = SelectState.Select;
        private ShopToolSelectableCellUI _activeCell;
        private List<ToolInfo> _tools;
        // these are the tools that the shop will display on this level
        private List<ToolPack> _toolsToDisplay = new List<ToolPack>();
        private PlayerComponents _currentPlayer;


        #region Constants
        private const string INSUFFICIENT_COINS_MESSAGE = "NOT ENOUGH COINS";
        private const string RETURN_ACTION_TEXT = "Return";
        private const string SELECT_ACTION_TEXT = "Select";
        #endregion

        public void SetReferences()
        {
            _shopParent.SetActive(false);
            _activeCell = _firstCell;
        }

        #region IManager
        public override void SetUp()
        {
            IsActive = true;

            base.SetUp();
            SetToolsInShop();
        }

        public override void Dispose()
        {
            IsActive = false;

            // TODO: Add dispose logic
        }
        #endregion

        #region Open and close
        public override void Open(int playerIndex)
        {
            base.Open(PlayerIndex);
            SetPlayerIndex(playerIndex);
            _currentPlayer = GameManager.Instance.PlayersManager.GetPlayer(PlayerIndex);
            // fill the player's side of the shop with the selected player's info (the tools they have)
            _currentPlayerInfo.SetPlayer(_currentPlayer);
            SetUpInput(PlayerIndex);
            HighlightFirstCell();
            _shopParent.SetActive(true);
            LoadPlayerItemsIntoShop();
            // update the UI
            _currentPlayerInfo.UpdateUIOfSelectedCell(_firstCell);
            IsOpened = true;
            //_currentPlayerInfo.gameObject.SetActive(true);
        }

        public override void Close(int playerIndex)
        {
            SetPlayerIndex(-1);
            base.Close(playerIndex);
            DisposeInput(playerIndex);
            _shopParent.SetActive(false);
            HighlightActiveSkill(false);
        }
        #endregion

        #region Input
        public void SetUpInput(int playerIndex)
        {
            GameManager.Instance.InputManager.DisableInputs((playerIndex + 1) % 2);
            GameManager.Instance.InputManager.EnableUIInput(playerIndex);

            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Up.name).performed += NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed += NavigateDown;
            UIMap.FindAction(c.UI.Submit.name).performed += SelectItem;
            UIMap.FindAction(c.UI.Cancel.name).performed += CloseShop;
            UIMap.FindAction(c.UI.Exit.name).performed += CloseShop;

            SetUpSignifiers();
        }

        public override void SetUpSignifiers()
        {
            base.SetUpSignifiers();

            PlayerInputActions c = InputManager.Controls;

            int returnActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Cancel.name, PlayerIndex);
            _menuActionSignifiersController.DisplaySignifier(RETURN_ACTION_TEXT, Helper.GetInputIcon(returnActionIconIndex));
            _menuActionSignifiersController.SetLastSignifierAsParent();

            int selectActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Submit.name, PlayerIndex);
            _menuActionSignifiersController.DisplaySignifier(SELECT_ACTION_TEXT, Helper.GetInputIcon(selectActionIconIndex));
            _menuActionSignifiersController.SetLastSignifierAsParent();
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Left.name).performed -= NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed -= NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Up.name).performed -= NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed -= NavigateDown;
            UIMap.FindAction(c.UI.Submit.name).performed -= SelectItem;
            UIMap.FindAction(c.UI.Cancel.name).performed -= CloseShop;
            UIMap.FindAction(c.UI.Exit.name).performed -= CloseShop;
  
            GameManager.Instance.InputManager.EnablePlayerInput();

            _menuActionSignifiersController.ClearAllSignifiers();
        }

        public override void NavigateLeft(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Left);
        }

        public override void NavigateRight(InputAction.CallbackContext _)
        {
            base.NavigateRight(_);
            Navigate(Direction.Right);
        }

        public override void NavigateUp(InputAction.CallbackContext _)
        {
            base.NavigateUp(_);
            Navigate(Direction.Up);
        }

        public override void NavigateDown(InputAction.CallbackContext _)
        {
            base.NavigateDown(_);
            Navigate(Direction.Down);
        }

        public void CloseShop(InputAction.CallbackContext _)
        {
            Return();
        }

        public void SelectItem(InputAction.CallbackContext _)
        {
            Select();
        }
        #endregion

        #region Input methods
        #region Navigation
        public override void Navigate(Direction direction)
        {
            base.Navigate(direction);

            if (!IsOpened)
            {
                return;
            }

            switch (_shopState)
            {
                case SelectState.Select:
                    {
                        MoveSelection(direction);
                        break;
                    }
                case SelectState.Buy:
                    {
                        ChangeBuyAmount(direction);
                        break;
                    }
                case SelectState.Sell:
                    {
                        ChangeSellAmount(direction);
                        break;
                    }
            }
        }

        private void MoveSelection(Direction direction)
        {
            SelectableEntityUI newSelectable = null;
            _activeCell.MoveSelection(direction, ref newSelectable, PlayerIndex);

            if (newSelectable == null)
            {
                return;
            }

            // if no movement occured, then don't display anything and return. Otherwise...
            _activeCell = (ShopToolSelectableCellUI)newSelectable;

            // fill in the tool's info and whether it's a shop tool or a tool that the player owns
            _currentPlayerInfo.UpdateUIOfSelectedCell(_activeCell);
            GameManager.Instance.AudioManager.Play(_onSwitchAudio);
        }

        private void ChangeBuyAmount(Direction direction)
        {
            if (direction == Direction.Down || direction == Direction.Up)
            {
                return;
            }

            // change the amount 
            _currentPlayerInfo.AddSelectedToolAmountBuy(direction == Direction.Right, _activeCell.Info);
            GameManager.Instance.AudioManager.Play(_onSwitchAudio); //for now
        }

        private void ChangeSellAmount(Direction direction)
        {
            if (direction == Direction.Down || direction == Direction.Up)
            {
                return;
            }

            // change the amount 
            _currentPlayerInfo.AddSelectedToolAmountSell(direction == Direction.Right, _activeCell.Info, _activeCell.Amount);
            GameManager.Instance.AudioManager.Play(_onSwitchAudio); //for now
        }
        #endregion

        #region Select
        public override void Select()
        {
            if (!IsOpened) return;

            switch (_shopState)
            {
                case SelectState.Select:
                    {
                        SelectTool();
                        break;
                    }
                case SelectState.Buy:
                    {
                        BuyTool();
                        break;
                    }
                case SelectState.Sell:
                    {
                        SellTool();
                        break;
                    }
            }
        }

        public void SelectTool()
        {
            if (!_activeCell.DisplaysAbility)
            {
                return;
            }

            GameManager.Instance.AudioManager.Play(_onSelectAudio);

            // dehighlight the currently selected tool
            _activeCell.HighLight(false);
            // highlight the buttons to draw attention and make sense
            _currentPlayerInfo.Highlight(true);
            // change the shop state
            ChangeShopState();
        }

        private void ChangeShopState()
        {
            // switch the input state. If it's an inventory tool, then switch to buy, otherwise, go to sell
            if (_activeCell.Owner == ShopToolSelectableCellUI.ToolOwner.Player)
            {
                _shopState = SelectState.Sell;
                // set the default amount of the tool to the max amount the tool has to encourage the player to sell the item completely i.e. spend more money :)
                _currentPlayerInfo.SetSelectedToolAmountSell(_activeCell.Amount, _activeCell.Info);
            }
            else
            {
                _shopState = SelectState.Buy;
                // set the default amount of the tool to 1
                _currentPlayerInfo.SetSelectedToolAmountBuy(1, _activeCell.Info);
            }
        }

        public void BuyTool()
        {
            PlayerShopInfoUI shopInfo = _currentPlayerInfo;

            // buying process
            // validating numbers (coins and nodes)
            if (GameManager.Instance.PlayersManager.Coins.CoinsAmount < _activeCell.Info.Cost * shopInfo.GetCurrentToolAmount())
            {
                GameManager.Instance.AudioManager.Play(_onErrorAudio);
                shopInfo.DisplayNotification(INSUFFICIENT_COINS_MESSAGE);
                return;
            }

            GameManager.Instance.AudioManager.Play(_onSelectAudio);

            // deduct coins. We deduct the nodes from the PlayerTools directly as not all the tools will be bought through the shop
            GameManager.Instance.PlayersManager.Coins.AddCoins(-_activeCell.Info.Cost * shopInfo.GetCurrentToolAmount());
            // add item to the inventory
            _currentPlayer.Tools.AddTool(_activeCell.Info, shopInfo.GetCurrentToolAmount());
            // if all goes well, deactivate the currently active tool
            DeselectTool();
            LoadPlayerItemsIntoShop();
        }

        public void SellTool()
        {
            PlayerShopInfoUI shopInfo = _currentPlayerInfo;

            // deduct the coins based on the amount of tools sold with an interest
            GameManager.Instance.PlayersManager.Coins.AddCoins(Mathf.CeilToInt(_activeCell.Info.Cost  * Constants.SHOP_SELL_ITEMS_PRICE_MULTIPLIER) * shopInfo.GetCurrentToolAmount());

            // remove the tools
            _currentPlayer.Tools.RemoveToolByTag(_activeCell.Info.ToolReference.GetTag(), shopInfo.GetCurrentToolAmount());

            LoadPlayerItemsIntoShop();
            DeselectTool();
        }
        #endregion

        #region Return
        public override void Return()
        {
            if (!IsOpened)
            {
                return;
            }

            switch (_shopState)
            {
                case SelectState.Select:
                    {
                        Close(PlayerIndex);
                        break;
                    }
                case SelectState.Buy:
                    {
                        DeselectTool();
                        break;
                    }
                case SelectState.Sell:
                    {
                        DeselectTool();
                        break;
                    }
            }
        }
        #endregion
        #endregion

        #region Extras
        public void HighlightFirstCell()
        {
            _activeCell = _firstCell;
            _firstCell.HighLight(true);
        }

        public void HighlightActiveSkill(bool highlight)
        {
            _activeCell.HighLight(highlight);
        }

        public void LoadPlayerItemsIntoShop()
        {
            _currentPlayerInfo.FillPlayerToolCells(_currentPlayer.Tools.GetTools());
        }

        public void DeselectTool()
        {
            // highlight the currently selected tool
            _activeCell.HighLight(true);
            // switch the input state
            _shopState = SelectState.Select;
            // dehighlight the buttons to draw attention and make sense
            _currentPlayerInfo.Highlight(false);
            // reset the amount
            _currentPlayerInfo.SetSelectedToolAmountBuy(1);
            // change the text of the button to what it was
            _currentPlayerInfo.UpdateUIOfSelectedCell(_activeCell);
        }

        public override void DehighlightCells()
        {
            base.DehighlightCells();
            IsOpened = false;
            _activeCell.HighLight(false);
        }

        public override void MoveFromActiveCell()
        {

        }

        public override void SetActiveCellAsFirstCell()
        {
            _activeCell = _firstCell;
        }

        private void SetToolsInShop()
        {
            if (!_showShopItems)
            {
                return;
            }

            _tools = GameManager.Instance.ToolsDatabase.GetAllTools();

            // make a copy of the tools and shuffle them to add variety
            List<ToolInfo> tools = _tools.Duplicate();

            for (int i = 0; i < _toolsToDisplayCount; i++)
            {
                ToolPack toolToAdd = new ToolPack();

                // if there are any tools to choose from
                if (tools.Count > 0)
                {
                    toolToAdd.Info = tools.RandomItem(true);
                }
                else
                {
                    toolToAdd.Info = _tools.RandomItem();
                }

                _toolsToDisplay.Add(toolToAdd);
                _allShopCells[i].SetUp(toolToAdd);
            }

            //// THE REST OF THE CELLS ////
            // cover the other cells that are not occupied
            _allShopCells.FindAll(c => !c.DisplaysAbility).ForEach(c => c.SetOverlayActive(true));
        }
        #endregion
    }
}
