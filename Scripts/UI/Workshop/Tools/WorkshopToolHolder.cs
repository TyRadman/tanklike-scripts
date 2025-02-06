using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using static TankLike.UnitControllers.TankTools;

namespace TankLike.UI.Workshop
{
    /// <summary>
    /// Holds the data of a tool and displays it on the workshop's tools menu. It's referred to in the comments as a 'Holder'
    /// </summary>
    public class WorkshopToolHolder : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _countText;
        public ToolPack CurrentTool { get; private set; }
        [field: SerializeField] public MenuSelectable MenuItem { get; private set; }

        public void SetUp(ToolPack tool)
        {
            gameObject.SetActive(true);
            CurrentTool = tool;
            _icon.sprite = CurrentTool.Info.IconImage;
            _nameText.text = CurrentTool.Info.Name;
            _countText.text = $"({CurrentTool.Tool.GetAmount()} / {CurrentTool.Tool.GetMaxAmount()})";
        }

        public void ResetHolder()
        {
            gameObject.SetActive(false);
        }

        public void Highlight(bool highlight)
        {
            MenuItem.Highlight(highlight);
        }

        public void UpdateCount()
        {
            _countText.text = $"({CurrentTool.Tool.GetAmount()} / {CurrentTool.Tool.GetMaxAmount()})";
        }
    }
}
