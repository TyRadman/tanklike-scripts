using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = "GameplaySettings_", menuName = "TankLike/Gameplay Settings")]
    public class GameplaySettings : ScriptableObject
    {
        public float AimSensitivity;
    }
}
