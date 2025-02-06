using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cam
{
    [CreateAssetMenu(fileName = "CameraShake_NAME", menuName = Directories.CAMERA + "Camera Shake")]
    public class CameraShakeSettings : ScriptableObject
    {
        [field: SerializeField] public CameraShakeType Type { get; private set; }
        [field: SerializeField] public float Intensity {get; private set;}
        [field: SerializeField] public float Frequency {get; private set;}
        [field: SerializeField] public float Time {get; private set;}
        [field: SerializeField] public bool Smooth {get; private set;}
    }

    public enum CameraShakeType
    {
        DEFAULT = 0,
        SHOOT = 1,
        EXPLOSION = 2,
        HIT = 3,
        GATE = 4,
        GROUND_POUND = 5,
    }
}
