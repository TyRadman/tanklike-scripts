namespace TankLike
{
    /// <summary>
    /// Interface for a builder pattern to create objects 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBuilder<T>
    {
        T Build();
    }
}
