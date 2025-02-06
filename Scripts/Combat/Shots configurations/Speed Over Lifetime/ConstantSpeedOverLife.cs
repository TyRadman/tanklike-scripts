using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = "Constant Speed", menuName = "Shot Configurations/Speed Over Lifetime/Constant Speed")]
    public class ConstantSpeedOverLife : SpeedOverLife
    {
        public override float GetSpeed(float speed, float deltaTime)
        {
            return base.GetSpeed(speed, deltaTime);
        }
    }
}
