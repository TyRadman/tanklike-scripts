using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TankLike
{
    public interface IDisplayedInput
    {
        /// <summary>
        /// Called on start and every time the display input bindings need to be updated.
        /// </summary>
        void UpdateInputDisplay(int playerIndex);
    }
}
