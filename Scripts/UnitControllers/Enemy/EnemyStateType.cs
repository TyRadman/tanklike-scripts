using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public enum EnemyStateType
    {
        IDLE,
        MOVE,
        AIM,
        CHASE,
        ATTACK,
        RETREAT,
        GET_HIT,
        DEATH,
    }
}
