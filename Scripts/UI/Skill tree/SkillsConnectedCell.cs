

namespace TankLike.UI.SkillTree
{
    [System.Serializable]
    public class SkillsConnectedCell
    {
        public UICell Cell;
        public Direction CellDirection;

        [System.Serializable]
        public class TempConnection
        {
            public UICell Cell;
            public Direction CellDirection;
        }
    }
}
