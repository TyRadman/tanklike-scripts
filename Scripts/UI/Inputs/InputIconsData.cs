using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UI
{
    [System.Serializable]
    public class InputIconsData 
    {
        public string Action;
        public List<string> Actions;
        public int KeyboardSpriteIndex;
        public int ControllerSpriteIndex;

        public bool HasAction(string action)
        {
            return Actions.Contains(action);
        }
    }
}
