using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public interface IPlayerController
    {
        int PlayerIndex { get; }
        UnitComponents ComponentsController { get; }
    }
}
