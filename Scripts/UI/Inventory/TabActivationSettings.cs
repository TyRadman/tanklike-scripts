using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Inventory
{
    [CreateAssetMenu(fileName = "Tab Activation Settings", menuName = "UI/ Tab Activation Settings")]
    public class TabActivationSettings : ScriptableObject
    {
        public float TabScale;
        public Color TabColor;
        public Color TabTextColor;
        public float TabFontSize;
    }
}
