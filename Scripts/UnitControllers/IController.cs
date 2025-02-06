namespace TankLike.UnitControllers
{
    /// <summary>
    /// Interface for all unit controllers.
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// Checks if the unit is currently active.
        /// </summary>
        public bool IsActive { get; }
        /// <summary>
        /// Setting up the unit upon spawning. Called only once when the unit is first instantiated.
        /// </summary>
        public void SetUp(IController controller);
        /// <summary>
        /// Make the unit playable after being deactivated or when the unit spawns for the first time in the game.
        /// </summary>
        public void Activate();
        /// <summary>
        /// Temporarily stop the unit from being playable.
        /// </summary>
        public void Deactivate();
        /// <summary>
        /// Resets all its components' values.
        /// </summary>
        public void Restart();
        /// <summary>
        /// For when the unit is to be removed from the game entirely. More like a cleaning script.
        /// </summary>
        public void Dispose();
    }
}
