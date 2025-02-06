using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI
{
    public interface ICell
    {
        public void MoveSelection(Direction direction, ref ICell cell, ref bool isCell, int playerIndex = 0);
        public void HighLight(bool highlight);
    }
}
