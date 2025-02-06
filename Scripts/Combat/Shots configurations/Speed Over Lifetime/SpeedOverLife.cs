using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public abstract class SpeedOverLife : ScriptableObject
    {
        public virtual float GetSpeed(float speed, float deltaTime)
        {
            return speed;
        }

        public virtual void Reset()
        {

        }
    }
}
