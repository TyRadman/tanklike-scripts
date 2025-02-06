using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public interface IStatModifiersController
    {
        void AddStat(SpeedStatModifier modifier);
        void RemoveStat(SpeedStatModifier modifier);
    }
}
