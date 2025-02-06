using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankLike.Combat;
using TankLike.UnitControllers;
using TMPro;
using static TankLike.UnitControllers.TankTools;

namespace TankLike.UI
{
    /// <summary>
    /// Displays the player's tools in the shop
    /// </summary>
    [RequireComponent(typeof(GridAssigner))]
    public class PlayerShopInfoUI : MonoBehaviour
    {
        [Header("Text References")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private TextMeshProUGUI _nodesText;
        [SerializeField] private TextMeshProUGUI _notificationText;
        [Header("Button References")]
        [SerializeField] private Button _mainButton;
        [SerializeField] private TextMeshProUGUI _mainButtonText;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [Header("Other References")]
        [SerializeField] private List<ShopToolSelectableCellUI> _cells;
        [SerializeField] private Image _highlightImage;
        private int _currentSelectedToolAmount = 1;
        [SerializeField] private PlayerComponents _player;
        [Header("Values")]
        [SerializeField] private Color _enoughCoinsTextColor;
        [SerializeField] private Color _notEnoughCoinsTextColor;
        [SerializeField] private ToolInfo _emptyCellsInfo;

        private const float NOTIFICATION_DISPLAY_DURATION = 3f;
        private const string BUY_TEXT = "Buy";
        private const string SELL_TEXT = "Sell";
        private const int MAX_BOUHGT_TOOLS_AMOUNT = 77;

        private void Awake()
        {
            _highlightImage.enabled = false;
            _notificationText.enabled = false;
            _currentSelectedToolAmount = 1;
        }

        public void SetPlayer(PlayerComponents player)
        {
            _player = player;
            _costText.text = $"0 / {GameManager.Instance.PlayersManager.Coins.CoinsAmount}";
        }

        /// <summary>
        /// Fills the price, and amount of the selected cell
        /// </summary>
        /// <param name="cell"></param>
        public void UpdateUIOfSelectedCell(ShopToolSelectableCellUI cell)
        {
            ToolInfo info = cell.Info == null ? _emptyCellsInfo : cell.Info;

            _nameText.text = info.Name;
            _descriptionText.text = info.Description;
            _costText.text = $"{info.Cost} / {GameManager.Instance.PlayersManager.Coins.CoinsAmount}";

            // set the buy/sell button's text
            _mainButtonText.text = cell.Owner == ShopToolSelectableCellUI.ToolOwner.Shop ? BUY_TEXT : SELL_TEXT;
            // set currencies colors
            SetCurrenciesColor(info);
        }

        private void SetCurrenciesColor(ToolInfo info)
        {
            _costText.color = _currentSelectedToolAmount * info.Cost <= GameManager.Instance.PlayersManager.Coins.CoinsAmount ? _enoughCoinsTextColor : _notEnoughCoinsTextColor;
        }

        public void FillPlayerToolCells(List<ToolPack> tools)
        {
            _cells.ForEach(c => c.ResetCell());

            for (int i = 0; i < tools.Count; i++)
            {
                _cells[i].SetUp(tools[i]);
            }
        }

        public void Highlight(bool highlight)
        {
            _highlightImage.enabled = highlight;
        }

        #region Buy Methods
        public void SetSelectedToolAmountBuy(int amount, ToolInfo info)
        {
            // set the amount
            _currentSelectedToolAmount = amount;

            // update the number on the button's text
            _mainButtonText.text = $"Amount: {_currentSelectedToolAmount}";

            // multiply the price of the tool with its amount
            _costText.text = $"{_currentSelectedToolAmount * info.Cost} / {GameManager.Instance.PlayersManager.Coins.CoinsAmount}";

            // change the color
            SetCurrenciesColor(info);
        }

        public void SetSelectedToolAmountBuy(int amount)
        {
            _currentSelectedToolAmount = amount;
        }

        public void AddSelectedToolAmountBuy(bool increment, ToolInfo info)
        {
            // change the amount
            _currentSelectedToolAmount = Mathf.Clamp(_currentSelectedToolAmount + (increment ? 1 : -1), 1, MAX_BOUHGT_TOOLS_AMOUNT);
            // update the number on the button's text
            _mainButtonText.text = $"Amount: {_currentSelectedToolAmount}";

            // multiply the price of the tool with its amount
            _costText.text = $"{_currentSelectedToolAmount * info.Cost} / {GameManager.Instance.PlayersManager.Coins.CoinsAmount}";

            // change the color
            SetCurrenciesColor(info);
        }
        #endregion

        #region Sell Methods
        public void SetSelectedToolAmountSell(int amount, ToolInfo info)
        {
            // reset the colors because there is no space to make an validation error when selling
            _costText.color = _enoughCoinsTextColor;
            _nodesText.color = _enoughCoinsTextColor;
            // change the amount
            _currentSelectedToolAmount = amount;
            // update the number on the button's text
            _mainButtonText.text = $"Amount: {_currentSelectedToolAmount}";

            // multiply the price of the tool with its amount
            _costText.text = $"{Mathf.CeilToInt(info.Cost * Constants.SHOP_SELL_ITEMS_PRICE_MULTIPLIER) * _currentSelectedToolAmount} + {GameManager.Instance.PlayersManager.Coins.CoinsAmount}";
        }

        public void AddSelectedToolAmountSell(bool increment, ToolInfo info, int maxAmount = int.MaxValue)
        {
            // reset the colors because there is no space to make an validation error when selling
            _costText.color = _enoughCoinsTextColor;
            _nodesText.color = _enoughCoinsTextColor;
            // change the amount
            _currentSelectedToolAmount = Mathf.Clamp(_currentSelectedToolAmount + (increment ? 1 : -1), 1, maxAmount);
            // update the number on the button's text
            _mainButtonText.text = $"Amount: {_currentSelectedToolAmount}";

            // multiply the price of the tool with its amount
            _costText.text = $"{Mathf.CeilToInt(info.Cost * Constants.SHOP_SELL_ITEMS_PRICE_MULTIPLIER) * _currentSelectedToolAmount} + {GameManager.Instance.PlayersManager.Coins.CoinsAmount}";
        }
        #endregion

        public int GetCurrentToolAmount()
        {
            return _currentSelectedToolAmount;
        }

        public void DisplayNotification(string text)
        {
            _notificationText.enabled = true;
            _notificationText.text = text;
            Invoke(nameof(HideNotificationMessage), NOTIFICATION_DISPLAY_DURATION);
        }

        private void HideNotificationMessage()
        {
            _notificationText.enabled = false;
        }
    }
}