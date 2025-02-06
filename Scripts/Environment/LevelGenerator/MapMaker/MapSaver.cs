using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.Environment.MapMaker
{
    using System;
#if UNITY_EDITOR
    using TankLike.Combat.Destructible;

    public class MapSaver : MonoBehaviour
    {
        [HideInInspector] public List<Tile> Tiles;

        private MapMakerManager _manager;

        public static Color GROUND_COLOR = Color.white;
        public static Color WALL_COLOR = new Color(1, 0.2358491f, 0.2358491f);

        private readonly List<(int size, string name)> _sizesThreshold = new List<(int size, string name)>() 
        {
            ( 0, "S" ),
            ( 42, "M" ),
            ( 70, "L" )
        };

        private const string DIRECTORY = "Assets/Environment/MapMaker/Rooms";
        private const string PREFIX = "Map_G";

        private void Awake()
        {
            _manager = GetComponent<MapMakerManager>();
        }

        public void SaveMap()
        {
            TileData[,] tiles = _manager.AllTiles;
            Tiles = new List<Tile>();

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    Tile tile = GetTileFromTileData(tiles[x, y]);
                    Tiles.Add(tile);
                }
            }

            string name = _manager.UI.GetMapName();
            int gatesCount = Tiles.FindAll(t => t.Tag == TileTag.Gate).Count;
            string mapSize = GetMapSize(_manager.Selector.LevelDimensions);
            string path = $"{DIRECTORY}/{PREFIX}{gatesCount}_{mapSize}_{name}.asset";

            if (System.IO.File.Exists(path))
            {
                _manager.UI.SetSaveMenuWarning($"Map with name Map already exists in {DIRECTORY}");
                return;
            }

            MapTiles_SO map = ScriptableObject.CreateInstance<MapTiles_SO>();
            map.Size = new Vector2Int(_manager.Selector.LevelDimensions.x, _manager.Selector.LevelDimensions.y);
            map.SetTiles(Tiles);
            map.Name = name;
            map.CacheSurroundingTilesIndices();

            AssetDatabase.CreateAsset(map, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            CancelButton();
        }

        private string GetMapSize(Vector2Int levelDimensions)
        {
            int sum = levelDimensions.x + levelDimensions.y;

            for (int i = _sizesThreshold.Count - 1; i >= 0; i--)
            {
                if (sum > _sizesThreshold[i].size)
                {
                    return _sizesThreshold[i].name;
                }
            }

            return "UnknownSize";
        }

        public Tile GetTileFromTileData(TileData tileData)
        {
            Tile tile = new Tile();
            tile.Dimension = tileData.Dimension;
            tile.Rotation = tileData.TileObject.transform.eulerAngles.y;
            tile.Tag = tileData.Tag;
            tile.Overlays = _manager.Overlays.GetOverlayTypeAtIndex(tile.Dimension.x, tile.Dimension.y);
            return tile;
        }

        #region Buttons
        public void SaveButton()
        {
            _manager.UI.ShowSaveMenu(true);
            _manager.Selector.IsActive = false;
            _manager.Controller.IsActive = false;
        }

        public void CancelButton()
        {
            _manager.UI.ShowSaveMenu(false);
            _manager.Selector.IsActive = true;
            _manager.Controller.IsActive = true;
        }
        #endregion
    }
#endif

    [System.Serializable]
    public class Tile
    {
        public TileTag Tag;
        public Vector2Int Dimension;
        public float Rotation;
        public List<DestructableTag> Overlays;
        [HideInInspector] public DestructableTag CurrentTag = DestructableTag.None;
        public bool IsSpawnPoint = false;
        [HideInInspector] public TileNeighbouringStatus NeighbouringStatus;
        public List<int> NeighbouringTiles = new List<int>();
        public EnemyType EnemyType;
        public GameObject BuiltTile;

        public bool HasEnemyOfType(EnemyType enemyType)
        {
            return EnemyType == enemyType && CurrentTag == DestructableTag.Enemy;
        }

        public TileType GetTileType()
        {
            return MapMakerManager.ToTileType(Tag);
        }

        internal Tile Clone()
        {
            Tile tile = new Tile();
            tile.Tag = Tag;
            tile.Dimension = Dimension;
            tile.Rotation = Rotation;
            tile.Overlays = new List<DestructableTag>(Overlays);
            tile.IsSpawnPoint = IsSpawnPoint;
            tile.NeighbouringStatus = NeighbouringStatus;
            tile.NeighbouringTiles = new List<int>(NeighbouringTiles);
            return tile;
        }

        public bool HasNeighbourOfType(TileType type, List<Tile> tiles)
        {
            return NeighbouringTiles.Exists(t => tiles[t].GetTileType() == type);
        }
    }

    public enum DestructableTag
    {
        None = 1000, Crate = 0, Rock = 1, Wall = 2, SpawnPoint = 3, BossSpawnPoint = 4,
        Explosive = 1001, DestructibleWalls = 5, Enemy = 6,
        GateGraphics = 1002
    }

    public enum TileNeighbouringStatus
    {
        MiddleGround = 0, WallNeighouringGround = 1, GroundNeighbouringWall = 2, MiddleWall = 3
    }
}
