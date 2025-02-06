using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class Break : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            public Color FontColor = Color.white;
            public Color BoxColor = Color.black;
            public int FontSize = 30;
            public float BoxSize = 10f;
        }

        public Data Info;
        [HideInInspector] public string _message;
    }
}
