using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    /// <summary>
    /// Holds data related to how the indicator behaves.
    /// </summary>
    [System.Serializable]
    public class IndicatorSettings
    {

        /// <summary>
        /// What is the state of the line of the indicator. The line extends from the player to the end of where the ability can be extended.
        /// <para><b>GroundLine</b>: the line is displayed on the ground.</para>
        /// <para><b>HighLine</b>: the line is displayed in the air.</para>
        /// <para><b>None</b>: There will be no line displayed.</para>
        /// </summary>
        [field: SerializeField] public AirIndicator.IndicatorLineState LineState { get; set; }
        /// <summary>
        /// The range within which the crosshair moves.
        /// </summary>
        [field: SerializeField] public Vector2 AimRange { get; set; }
        /// <summary>
        /// The value by which the crosshair movement speed is multiplied.
        /// </summary>
        [field: SerializeField] public float AimSpeedMultiplier { get; set; }

        /// <summary>
        /// Whether the crosshair is the parent of the indicator's end when the aiming with the indicator starts.
        /// </summary>
        [field: SerializeField, Header("End Point")] public bool IsCrosshairTheParent { get; set; }
        [field: SerializeField] public bool ShowCrosshairOnEnd { get; set; } = false;
        /// <summary>
        /// The size of the circle at the end of the indicator.
        /// </summary>
        [field: SerializeField] public float EndSize { get; set; }


        [field : SerializeField, Header("Air Trajectory")] public float AirLineWidth { get; set; } = 0.3f;
        [field: SerializeField] public float LineHeight { get; set; }

        /// <summary>
        /// Whether the player can move the end of the indicator into walls.
        /// </summary>
        [field: SerializeField, Header("Ground Trajectory")] public bool AvoidWalls { get; set; }
    }

    [System.Serializable]
    public class AirTrajectorySettings
    {
        public float AirLineWidth { get; set; } = 0.3f;
        public float LineHeight { get; set; }
    }

    [System.Serializable]
    public class  GroundTrajectory
    {
        /// <summary>
        /// Whether the player can move the end of the indicator into walls.
        /// </summary>
        public bool AvoidWalls { get; set; }
    }
}
