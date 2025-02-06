using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class SpeedOverDeflections : ScriptableObject
    {
        public virtual float GetSpeed()
        {
            return 0f;
        }
    }
}
