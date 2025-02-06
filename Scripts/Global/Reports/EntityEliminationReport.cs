using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using UnitControllers;

    public class EntityEliminationReport
    {
        public IController Target { get; set; }
        public int PlayerIndex { get; set; }
        public Vector3 Position { get; set; }
    }
}
