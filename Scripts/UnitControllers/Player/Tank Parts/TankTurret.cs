using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankTurret : TankBodyPart
    {
        [field: SerializeField] public Transform[] ShootingPoints { private set; get; }
        [field: SerializeField] public Animator Animator { private set; get; }
    }
}
