namespace TankLike.UnitControllers
{
    /// <summary>
    /// Interface for classes that can receive data from UnitData.
    /// </summary>
    public interface IUnitDataReciever
    {
        /// <summary>
        /// Applies data from UnitData to the class properties that need it.
        /// </summary>
        /// <param name="data"></param>
        void ApplyData(UnitData data);
    }
}
