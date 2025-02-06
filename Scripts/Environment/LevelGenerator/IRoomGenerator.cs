using TankLike.Environment;
using TankLike.Environment.LevelGeneration;
using TankLike.Environment.MapMaker;

namespace TankLike
{
    public interface IRoomGenerator
    {
        void BuildRoom(MapTiles_SO map, LevelData styler, Room room, BuildConfigs configs);
    }
}
