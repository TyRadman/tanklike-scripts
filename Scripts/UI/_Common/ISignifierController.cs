using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Signifiers
{
    /// <summary>
    /// An interface for objects that act as signifiers for any in-game actions.
    /// </summary>
    public interface ISignifierController
    {
        public void DisplaySignifier(string action, string key);
    }
}
