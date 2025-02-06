using System.Collections;
using System.Collections.Generic;
using TankLike.Sound;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "BD_NAME", menuName = Directories.BOSSES + "Boss Data")]
    public class BossData : UnitData
    {
        [field: SerializeField] public BossType BossType { get; private set; }
        [field: SerializeField] public GameObject BossPrefab { get; private set; }
        [field: SerializeField] public Audio BossBGMusic { get; private set; }
        [field: SerializeField] public float ZoomValue { get; private set; }
        [field: SerializeField] public float CameraMovementToBossDuration { get; private set; } = 2f;
        [field: SerializeField] public float BossAnimationDuration { get; private set; } = 1f;
        [field: SerializeField] public Vector3 RoomSize { get; private set; }
        [field: SerializeField] public UnitParts PartsPrefab { get; private set; }

    }
}
