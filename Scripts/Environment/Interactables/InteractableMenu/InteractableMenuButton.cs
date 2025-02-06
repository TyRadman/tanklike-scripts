using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI
{
    /// <summary>
    /// Holds a menu selectable and additional information like whether its OnClick has been populated.
    /// </summary>
    public class InteractableMenuButton : MonoBehaviour
    {
        public MenuSelectable Button;
        public bool IsActive = false;
    }
}
