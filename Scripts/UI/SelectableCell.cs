using System.Collections;
using System.Collections.Generic;
using TankLike.UI.SkillTree;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI
{
    /// <summary>
    /// A base class for all cells that can be set in a grid with the ability to be navigated through by the player.
    /// </summary>
    public abstract class SelectableCell : SelectableEntityUI
    {
        public enum User
        {
            PlayerOne = 0, PlayerTwo = 1, Mutual = 2
        }

        #region Structs
        [System.Serializable]
        public class NextCell
        {
            public SelectableEntityUI Cell;
            public Direction CellDirection;
            public User CellUser = User.Mutual;
        }
        #endregion

        public Vector2Int Location;
        public List<NextCell> NextCellInputs;

        public virtual void ResetCell()
        {

        }

        public override void HighLight(bool highlight)
        {

        }

        public void CreateNextCellsHolders()
        {
            for (int i = 0; i < 4; i++)
            {
                NextCellInputs.Add(new NextCell() { CellDirection = (Direction)i });
            }
        }

        public override void MoveSelection(Direction direction, ref SelectableEntityUI cell, int playerIndex = 0)
        {
            // we choose the cell at the given direction and with the given user index
            SelectableEntityUI selectedSelectable = NextCellInputs.Find(s => s.CellDirection == direction &&
            (s.CellUser == (User)playerIndex || s.CellUser == User.Mutual))
                .Cell;

            // if there is a cell to move to, then highlight it and dehighlight this one
            if (selectedSelectable != null)
            {
                selectedSelectable.HighLight(true);
                HighLight(false);
                cell = selectedSelectable;
            }
        }
    }
}
