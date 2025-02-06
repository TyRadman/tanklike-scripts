using System.Collections;
using System.Collections.Generic;
using TankLike.Sound;
using UnityEngine;

namespace TankLike.UI.PauseMenu
{
    public class PauseMenuNavigatable : Navigatable
    {
        public override void Open(int playerIndex = 0)
        {
            base.Open(playerIndex);
        }

        public override void Close(int playerIndex = 0)
        {
            base.Close(playerIndex);
        }

        public override void Navigate(Direction direction)
        {
            base.Navigate(direction);
        }
    }
}
