using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class ComponentsBreak : MonoBehaviour
    {
        [System.Serializable]
        public class Values
        {
            public string Text;
            public Color BoxColor;
            public Color TextColor;
            [Range(20f, 100f)] public float Size;
            [Range(15, 50)] public int FontSize;
        }

        public Values Value;
    }
}
