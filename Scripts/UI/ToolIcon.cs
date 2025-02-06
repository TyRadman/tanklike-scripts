using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TankLike.Combat;

namespace TankLike.UI
{
    public class ToolIcon : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private Image _amountPanel;
        [SerializeField] private Image _cooldownOverlay;
        [SerializeField] private TextMeshProUGUI _keyText;
        [SerializeField] private ToolAmountBar _barReference;
        [SerializeField] private HorizontalLayoutGroup _layoutGroup;
        private List<ToolAmountBar> _bars = new List<ToolAmountBar>();

        public void SetUp(Tool tool)
        {
            tool.SetToolIconUI(this);
            _iconImage.sprite = tool.GetIcon();
            _amountText.text = tool.GetAmount().ToString("0");
            _amountPanel.enabled = true;

            // create the amount bars
            for (int i = 0; i < tool.GetMaxAmount(); i++)
            {
                ToolAmountBar bar = Instantiate(_barReference, _layoutGroup.transform);
                _bars.Add(bar);
            }

            Invoke(nameof(DisableBarsLayoutGroup), 1f);
        }

        private void DisableBarsLayoutGroup()
        {
            _layoutGroup.enabled = false;
        }

        public void SetAmount(int amount)
        {
            //_amountText.text = amount;
            _bars[amount].DisableImage();
        }

        public void ResetIcon(Sprite emptyIcon)
        {
            _iconImage.sprite = emptyIcon;
            _amountText.text = string.Empty;
            _amountPanel.enabled = false;
        }

        public void SetCooldownOverlayFill(float amount)
        {
            _cooldownOverlay.fillAmount = amount;
        }

        public void SetKey(string key)
        {
            _keyText.text = key;
        }
    }
}
