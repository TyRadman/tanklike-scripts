using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    using Combat.Destructible;
    using Environment.LevelGeneration;
    using System.Collections.Generic;

    public class MapMakerManager : MonoBehaviour
    {
        public MapMakerWallArranger WallArranger;
        public MapMakerGateArranger GateArranger;
        public MapMakerCameraControl Controller;
        public GressArranger GroundArranger;
        public OverlaysArranger Overlays;
        public MapMakerUI UI;
        public TileData[,] AllTiles;
        [HideInInspector] public Room Room { get; set; }
        public MapTileStyler Styler;
        public MapMakerRoomGenerator Generator;
        [Header("Map Loading")]
        [SerializeField] private MapTiles_SO _mapToBuild;
        [SerializeField] private LevelData _styler;
        [SerializeField] private int _tilesCount = 0;

        [field: SerializeField] public MapMakerMirroringController MirroringController { get; private set; }
        [field: SerializeField] public MapMakerSelector Selector { get; private set; }

        private static Dictionary<TileTag, TileType> _tileTagToType = new Dictionary<TileTag, TileType>
        {
            { TileTag.Ground, TileType.Ground },
            { TileTag.Ground_OneSide, TileType.Ground },
            { TileTag.Ground_OutCorner, TileType.Ground },
            { TileTag.Ground_InCorner, TileType.Ground },
            { TileTag.Wall_NoSides, TileType.Wall },
            { TileTag.Wall_OneSide, TileType.Wall },
            { TileTag.Wall_TwoSides, TileType.Wall },
            { TileTag.Wall_Corner, TileType.Wall },
            { TileTag.Wall_ThreeSides, TileType.Wall },
            { TileTag.Wall_FourSides, TileType.Wall },
            { TileTag.Gate, TileType.Gate }
        };

        private void Start()
        {
            CorrectWalls();
            SetUp();
        }

        private void SetUp()
        {
            MirroringController.SetUp();
            Selector.SetUp(this);
        }

        private void Update()
        {
            _tilesCount = AllTiles.GetLength(0) * AllTiles.GetLength(1);
        }

        public void CorrectWalls()
        {
            WallArranger.CheckTiles(ref AllTiles, 0, 0, false);
        }

        /// <summary>
        /// Use this method to compare whether a ground tile is ground, or if a specific wall is a general wall
        /// </summary>
        /// <param name="tileToCompare"></param>
        /// <param name="tileToCompareTo"></param>
        /// <returns></returns>
        public static bool TagEquals(TileTag tileToCompare, TileType tileToCompareTo)
        {
            if (_tileTagToType.ContainsKey(tileToCompare))
            {
                return _tileTagToType[tileToCompare] == tileToCompareTo;
            }

            return false;
        }

        public static TileType ToTileType(TileTag tag)
        {
            return _tileTagToType[tag];
        }

        public void ClearLevel()
        {
            Selector.BuildNewRoom();
            Selector.SetUpGround();
            Overlays.ClearOverlays();
        }

        [ContextMenu("Load Map")]
        public void LoadMap()
        {
            Generator.BuildRoom(_mapToBuild, _styler, Room);
        }
    }

    public enum TileType
    {
        Ground = 0, Wall = 1, Gate = 2, Overlay = 3, SpawnPoint = 4
    }

    public enum TileTag
    {
        Ground = 0, Ground_OneSide = 1, Ground_OutCorner = 2, Ground_InCorner = 3, 
        Wall_NoSides = 4, Wall_OneSide = 5, Wall_TwoSides = 6, Wall_Corner = 7, Wall_ThreeSides = 8, Wall_FourSides = 9,
        Gate = 10
    }
}
