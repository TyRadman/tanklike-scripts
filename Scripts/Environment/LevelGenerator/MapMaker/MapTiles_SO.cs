using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    using Environment.LevelGeneration;
    using System;

    /// <summary>
    /// Holds a set of tiles (level theme)
    /// </summary>
    [CreateAssetMenu(fileName = "Map Tiles", menuName = "Level/ Map Tiles")]
    public class MapTiles_SO : ScriptableObject
    {
        public string Name;
        public List<Tile> Tiles;
        public int GateCount;
        public Vector2Int Size;


        public List<GateData> Surroundings = new List<GateData>(4)
        {
            new GateData(){Surrounding = Surrounding.Block, Direction = GateDirection.East},
            new GateData(){Surrounding = Surrounding.Block, Direction = GateDirection.North},
            new GateData(){Surrounding = Surrounding.Block, Direction = GateDirection.West},
            new GateData(){Surrounding = Surrounding.Block, Direction = GateDirection.South}
        };


        // Predefined directions for neighbors
        private readonly Vector2Int[] directions = 
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(-1, -1),
            new Vector2Int(1, -1), new Vector2Int(-1, 1)
        };

        [System.Serializable]
        public class GateData
        {
            public Surrounding Surrounding;
            public GateDirection Direction;
        }

        public void SetTiles(List<Tile> tiles)
        {
            Tiles = new List<Tile>();
            tiles.ForEach(t => Tiles.Add(t));
            GateCount = Tiles.FindAll(t => t.Tag == TileTag.Gate).Count;

            // cache the gates
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tile currentTile = Tiles[i];
                int maxXIndex = Tiles.OrderByDescending(t => t.Dimension.x).First().Dimension.x;
                int maxYIndex = Tiles.OrderByDescending(t => t.Dimension.y).First().Dimension.y;

                if (currentTile.Tag == TileTag.Gate)
                {
                    GateDirection direction = GameplayRoomGenerator.GetGateDirection(maxXIndex, maxYIndex, currentTile.Dimension);
                    int index = (int)direction / 90;
                    Surroundings[index].Surrounding = Surrounding.Gate;
                }
            }
        }

        public void CacheSurroundingTilesIndices()
        {
            Tile[,] tileGrid = new Tile[Size.x, Size.y]; ;

            foreach (Tile tile in Tiles)
            {
                tileGrid[tile.Dimension.x, tile.Dimension.y] = tile;
            }

            for (int i = 0; i < Tiles.Count; i++)
            {
                Tile tile = Tiles[i];
                tile.NeighbouringTiles = GetSurroundingTiles(tile, tileGrid);
            }
        }

        private List<int> GetSurroundingTiles(Tile currentTile, Tile[,] tileGrid)
        {
            List<int> neighbors = new List<int>();

            foreach (Vector2Int direction in directions)
            {
                int neighborX = currentTile.Dimension.x + direction.x;
                int neighborY = currentTile.Dimension.y + direction.y;

                // Bounds check for the grid
                if (neighborX >= 0 && neighborX < tileGrid.GetLength(0) &&
                    neighborY >= 0 && neighborY < tileGrid.GetLength(1))
                {
                    Tile neighborTile = tileGrid[neighborX, neighborY];

                    if (neighborTile != null)
                    {
                        neighbors.Add(Tiles.IndexOf(neighborTile));
                    }
                }
            }

            return neighbors;
        }


    }
}
