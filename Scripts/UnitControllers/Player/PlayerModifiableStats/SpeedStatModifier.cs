using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [System.Serializable]
    public class SpeedStatModifier : IStatModifier
    {
        public float SpeedMultiplier { get; private set; } = 1f;

        public SpeedStatModifier(float speedMultiplier)
        {
            SpeedMultiplier = speedMultiplier;
        }

        public void AddStatModifier(IStatModifiersController controller)
        {
            controller.AddStat(this);
        }

        public void RemoveStatModifier(IStatModifiersController controller)
        {
            controller.RemoveStat(this);
        }
    }
}
