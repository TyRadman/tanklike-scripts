using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "Wiggle_", menuName = "TankLike/Others/Wiggle")]
    public class Wiggle : ScriptableObject
    {
        [Tooltip("A positive value moves the tank forward")]
        public AnimationCurve Curve;
        public float Duration = 0.5f;
        public float MaxAngle = 15f;
        public bool Backward;
    }
}
