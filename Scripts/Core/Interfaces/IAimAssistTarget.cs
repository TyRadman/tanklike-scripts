using UnityEngine;

namespace TankLike
{
    using Environment;

    /// <summary>
    /// Represents an object (not an enemy) that can be targeted by the aim assist system.
    /// </summary>
    public interface IAimAssistTarget 
    {
        /// <summary>
        /// Assigns the object as a target in the specified room.
        /// </summary>
        /// <param name="room"></param>
        void AssignAsTarget(Room room);

        /// <summary>
        /// Event that is triggered when the target is destroyed.
        /// </summary>
        System.Action<Transform> OnTargetDestroyed { get; set; }
    }
}
