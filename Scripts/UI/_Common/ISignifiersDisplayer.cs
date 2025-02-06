using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Signifiers
{
    /// <summary>
    /// An interface for classes that display any action-related signifiers.
    /// </summary>
    public interface ISignifiersDisplayer 
    {
        /// <summary>
        /// Sets a reference to the signifier controller and/or sets it up.
        /// </summary>
        /// <param name="signifierController">Reference to the signifier controller.</param>
        public void SetUpActionSignifiers(ISignifierController signifierController);
        public ISignifierController SignifierController { get; set; }
    }
}
