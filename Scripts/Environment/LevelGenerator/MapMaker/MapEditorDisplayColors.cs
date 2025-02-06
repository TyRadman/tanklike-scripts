using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.EditorStuff
{
    [CreateAssetMenu(fileName = "Editor map colors", menuName = Directories.EDITOR + "Editor Map Icon Colors")]
    public class MapEditorDisplayColors : ScriptableObject
    {
        public static Color WALL_COLOR = Color.gray;
        public static Color GROUND_COLOR = Color.white;
        public static Color GATE_COLOR = Color.green;
        public static Color OVERLAY_CRATE_COLOR = Color.yellow;
        public static Color SPAWN_POINTS_COLOR = new Color(1f, 0.2311f, 0.2311f);
        public static Color BOSS_SPAWN_POINTS_COLOR = new Color(0.5542442f, 0.1176471f, 1f);
    }
}
