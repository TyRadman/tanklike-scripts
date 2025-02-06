using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;
using static TankLike.UnitControllers.TankTools;
using TankLike.Utils;
using TMPro;

namespace TankLike.UI
{
    public class ShopToolSelectableCellUI : SelectableVisualizedCellUI
    {
        public enum ToolOwner
        {
            Shop, Player
        }

        [field: Header("Shop Variables")]
        [field: SerializeField] public ToolOwner Owner { get; private set; }
        [field: SerializeField] public ToolInfo Info { get; private set; }
        [field: SerializeField] public bool DisplaysAbility { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
        [SerializeField] private TextMeshProUGUI _amountText;

        private void Awake()
        {
            _amountText.text = string.Empty;
        }

        #region Set Up Overloads
        public void SetUp(ToolPack tool, bool isTool = true)
        {
            Info = tool.Info;

            if (!isTool) return;

            if (tool.Tool != null)
            {
                Amount = tool.Tool.GetAmount();
                _amountText.text = Amount.ToString();
            }
            else
            {
                _amountText.text = string.Empty;
            }

            SetIcon(tool.Info.IconImage);

            DisplaysAbility = true;
        }

        // only for the locked cells
        public void SetUp(ToolInfo toolInfo)
        {
            Info = toolInfo;
        }

        public override void ResetCell()
        {
            base.ResetCell();
            Amount = 0;
            _amountText.text = string.Empty;
            DisplaysAbility = false;
            Info = null;
        }
        #endregion

        public override void MoveSelection(Direction direction, ref SelectableEntityUI cell, int playerIndex = 0)
        {
            SelectableEntityUI selectedSelectable = NextCellInputs.Find(s => s.CellDirection == direction &&
               (s.CellUser == (User)playerIndex || s.CellUser == User.Mutual))
                   .Cell;

            // if there is no cell in the required direction then return
            if (selectedSelectable == null)
            {
                return;
            }

            // if the next cell is a group, then perfrom movement through the group
            if (selectedSelectable.Type == SelectableType.Group)
            {
                HighLight(false);
                selectedSelectable.MoveSelection(direction, ref cell, playerIndex);
                return;
            }

            ///NORMAL CELL FUNCTIONALITY////
            base.MoveSelection(direction, ref cell, playerIndex);
        }

        public void SetOverlayActive(bool active)
        {
            _overLayImageObject.SetActive(active);
        }
    }
}
