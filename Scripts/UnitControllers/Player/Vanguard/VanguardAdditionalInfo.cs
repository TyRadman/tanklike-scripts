using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class VanguardAdditionalInfo : TankAdditionalInfo
    {
        [field: SerializeField] public Transform[] BrockShootingPoints { get; private set; }
        /// <summary>
        /// The shooting points start from the bottom and go up.
        /// </summary>
        [field: SerializeField] public Transform[] BackShootingPoints { get; private set; }
    }
}
