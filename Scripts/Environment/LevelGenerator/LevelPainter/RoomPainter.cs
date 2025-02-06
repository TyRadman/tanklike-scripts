using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using Environment.MapMaker;

    public abstract class RoomPainter : MonoBehaviour, IManager
    {
        /// <summary>
        /// The type of filter to apply to the tiles.
        /// </summary>
        public enum FilterType
        {
            NeighbourOfTypeWithinDepth = 0,
            NeighbourOfTypeNotWithinDepth = 1,
            NeighbourOfTagWithinDepth = 2,
            NeighbourOfTagNotWithinDepth = 3,
            NeighbourOfTypeWithinDepthRange = 4,
            NeighbourOfTypeNotWithinDepthRange = 5,
            IsOfType = 6,
            NeighbourOfTagWithinDepthRange = 7,
            NeighbourOfTagNotWithinDepthRange = 8,
            IsOfTag = 9
        }

        [System.Serializable]
        public struct PaintingRule
        {
            public bool PerformOpposite;
            public FilterType FilterType;

            public int Depth;
            public int MinDepth;
            public int MaxDepth;

            public TileType TileType;
            public DestructableTag DestructableTag;
        }

        public bool IsActive { get; private set; }

        protected LevelData _levelData;

        [HideInInspector] public List<PaintingRule> PaintingRules;

        #region IManager
        public virtual void SetUp()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager is null");
                return;
            }

            _levelData = GameManager.Instance.LevelGenerator.LevelData;
        }

        public virtual void Dispose()
        {
            _levelData = null;
        }
        #endregion

        public abstract void PaintRoom(MapTiles_SO map, Room room);

        #region Utilities
        public List<Tile> GetTilesByRules(List<Tile> inputTiles, List<PaintingRule> rules)
        {
            List<Tile> tiles = new List<Tile>();

            for (int j = 0; j < inputTiles.Count; j++)
            {
                Tile t = inputTiles[j];

                if (t.BuiltTile == null)
                {
                    continue;
                }

                bool rulePassed = false;

                for (int i = 0; i < rules.Count; i++)
                {
                    PaintingRule paintingRule = rules[i];

                    switch (paintingRule.FilterType)
                    {
                        case FilterType.IsOfType:
                            rulePassed = t.GetTileType() == paintingRule.TileType && !paintingRule.PerformOpposite;
                            break;
                        case FilterType.NeighbourOfTypeWithinDepth:
                            rulePassed = HasNeighborWithinDepth(t, paintingRule.TileType, paintingRule.Depth, inputTiles);
                            break;
                        case FilterType.NeighbourOfTypeNotWithinDepth:
                            rulePassed = !HasNeighborWithinDepth(t, paintingRule.TileType, paintingRule.Depth, inputTiles);
                            break;
                        case FilterType.NeighbourOfTagWithinDepth:
                            rulePassed = HasNeighborWithinDepth(t, paintingRule.DestructableTag, paintingRule.Depth, inputTiles);
                            break;
                        case FilterType.NeighbourOfTagNotWithinDepth:
                            rulePassed = !HasNeighborWithinDepth(t, paintingRule.DestructableTag, paintingRule.Depth, inputTiles);
                            break;
                        case FilterType.NeighbourOfTypeWithinDepthRange:
                            rulePassed = HasNeighborWithinExactDepthRange(t, paintingRule.TileType, paintingRule.MinDepth, paintingRule.MaxDepth, inputTiles);
                            break;
                        case FilterType.NeighbourOfTypeNotWithinDepthRange:
                            rulePassed = !HasNeighborWithinExactDepthRange(t, paintingRule.TileType, paintingRule.MinDepth, paintingRule.MaxDepth, inputTiles);
                            break;
                        case FilterType.NeighbourOfTagWithinDepthRange:
                            rulePassed = HasNeighborWithinDepthRange(t, paintingRule.DestructableTag, paintingRule.MinDepth, paintingRule.MaxDepth, inputTiles);
                            break;
                        case FilterType.NeighbourOfTagNotWithinDepthRange:
                            rulePassed = !HasNeighborWithinDepthRange(t, paintingRule.DestructableTag, paintingRule.MinDepth, paintingRule.MaxDepth, inputTiles);
                            break;
                        case FilterType.IsOfTag:
                            rulePassed = t.CurrentTag == paintingRule.DestructableTag && !paintingRule.PerformOpposite;
                            break;
                        default:
                            rulePassed = false;
                            break;
                    }

                    if (!rulePassed)
                    {
                        break;
                    }
                }

                if (rulePassed)
                {
                    tiles.Add(t);
                }
            }

            return tiles.FindAll(t => t != null && t.BuiltTile != null);
        }

        /// <summary>
        /// Checks if a tile has a neighbor of a specific type within a certain depth.
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="targetType"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        protected bool HasNeighborWithinDepth(Tile tile, TileType targetType, int depth, List<Tile> tiles)
        {
            // a breadth-first search (BFS) approach where we initialize, process, explore, repeat, and return

            if (depth <= 0)
            {
                return false;
            }

            Queue<(Tile, int)> queueToCheck = new Queue<(Tile, int)>();
            HashSet<Tile> checkedTiles = new HashSet<Tile>();

            queueToCheck.Enqueue((tile, 0));
            checkedTiles.Add(tile);

            while (queueToCheck.Count > 0)
            {
                var (currentTile, currentDepth) = queueToCheck.Dequeue();

                if (currentDepth >= depth)
                {
                    continue;
                }

                for (int i = 0; i < currentTile.NeighbouringTiles.Count; i++)
                {
                    Tile neighbor = tiles[currentTile.NeighbouringTiles[i]];

                    if (neighbor.GetTileType() == targetType)
                    {
                        return true;
                    }

                    if (!checkedTiles.Contains(neighbor) && currentDepth + 1 < depth)
                    {
                        queueToCheck.Enqueue((neighbor, currentDepth + 1));
                        checkedTiles.Add(neighbor);
                    }
                }
            }

            return false;
        }

        protected bool HasNeighborWithinDepth(Tile tile, DestructableTag targetType, int depth, List<Tile> tiles)
        {
            // a breadth-first search (BFS) approach where we initialize, process, explore, repeat, and return
            if (depth <= 0)
            {
                return false;
            }

            Queue<(Tile, int)> queueToCheck = new Queue<(Tile, int)>();
            HashSet<Tile> checkedTiles = new HashSet<Tile>();

            queueToCheck.Enqueue((tile, 0));
            checkedTiles.Add(tile);

            while (queueToCheck.Count > 0)
            {
                var (currentTile, currentDepth) = queueToCheck.Dequeue();

                if (currentDepth >= depth)
                {
                    continue;
                }

                for (int i = 0; i < currentTile.NeighbouringTiles.Count; i++)
                {
                    Tile neighbor = tiles[currentTile.NeighbouringTiles[i]];

                    if (neighbor.CurrentTag == targetType)
                    {
                        return true;
                    }

                    if (!checkedTiles.Contains(neighbor) && currentDepth + 1 < depth)
                    {
                        queueToCheck.Enqueue((neighbor, currentDepth + 1));
                        checkedTiles.Add(neighbor);
                    }
                }
            }

            return false;
        }

        protected int GetNumberOfNeighbouringTilesOfType(Tile tile, TileType type, List<Tile> tiles)
        {
            int numberOfMatchingTiles = 0;

            for (int i = 0; i < tile.NeighbouringTiles.Count; i++)
            {
                if (tiles[tile.NeighbouringTiles[i]].GetTileType() == type)
                {
                    numberOfMatchingTiles++;
                }
            }

            return numberOfMatchingTiles;
        }

        protected int GetNumberOfNeighbouringTilesOfType(Tile tile, DestructableTag tag, List<Tile> tiles)
        {
            int numberOfMatchingTiles = 0;

            for (int i = 0; i < tile.NeighbouringTiles.Count; i++)
            {
                if (tiles[tile.NeighbouringTiles[i]].CurrentTag == tag)
                {
                    numberOfMatchingTiles++;
                }
            }

            return numberOfMatchingTiles;
        }

        protected bool HasNeighborWithinExactDepthRange(Tile tile, TileType targetType, int exactDepthMin, int exactDepthMax, List<Tile> tiles)
        {
            if (exactDepthMin <= 0)
            {
                return false;
            }

            Queue<(Tile, int)> queueToCheck = new Queue<(Tile, int)>();
            HashSet<Tile> checkedTiles = new HashSet<Tile>();

            queueToCheck.Enqueue((tile, 0));
            checkedTiles.Add(tile);

            while (queueToCheck.Count > 0)
            {
                var (currentTile, currentDepth) = queueToCheck.Dequeue();

                if (currentDepth >= exactDepthMin && currentDepth <= exactDepthMax)
                {
                    if (currentTile.GetTileType() == targetType)
                    {
                        return true;
                    }
                }

                if (currentDepth > exactDepthMax)
                {
                    continue;
                }

                for (int i = 0; i < currentTile.NeighbouringTiles.Count; i++)
                {
                    Tile neighbor = tiles[currentTile.NeighbouringTiles[i]];

                    if (!checkedTiles.Contains(neighbor))
                    {
                        queueToCheck.Enqueue((neighbor, currentDepth + 1));
                        checkedTiles.Add(neighbor);
                    }
                }
            }

            return false;
        }

        protected bool HasNeighborWithinDepthRange(Tile tile, DestructableTag targetType, int minDepth, int maxDepth, List<Tile> tiles)
        {
            if (minDepth <= 0)
            {
                return false;
            }

            Queue<(Tile, int)> queueToCheck = new Queue<(Tile, int)>();
            HashSet<Tile> checkedTiles = new HashSet<Tile>();

            queueToCheck.Enqueue((tile, 0));
            checkedTiles.Add(tile);

            while (queueToCheck.Count > 0)
            {
                var (currentTile, currentDepth) = queueToCheck.Dequeue();

                if (currentDepth >= minDepth && currentDepth <= maxDepth)
                {
                    if (currentTile.CurrentTag == targetType)
                    {
                        return true;
                    }
                }

                if (currentDepth > maxDepth)
                {
                    continue;
                }

                for (int i = 0; i < currentTile.NeighbouringTiles.Count; i++)
                {
                    int index = currentTile.NeighbouringTiles[i];

                    if (index >= tiles.Count)
                    {
                        continue;
                    }

                    Tile neighbor = tiles[index];

                    if (!checkedTiles.Contains(neighbor))
                    {
                        queueToCheck.Enqueue((neighbor, currentDepth + 1));
                        checkedTiles.Add(neighbor);
                    }
                }
            }

            return false;
        }
        #endregion
    }
}
