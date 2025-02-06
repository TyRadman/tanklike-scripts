using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ScreenFreeze
{
    [CreateAssetMenu(fileName = "SF_", menuName = "TankLike/Others/Screen Freeze")]
    public class ScreenFreezeData : ScriptableObject
    {
        public float Duration;
        public AnimationCurve Curve;
    }
}
