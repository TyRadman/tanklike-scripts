using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TankLike.Environment.LevelGeneration
{
    using Environment.MapMaker;
    using Combat.Destructible;
    using Utils;

    public class DestructibleWallsPainter : RoomPainter
    {
        private const int GROUND_TILES_COUNT_PER_WALL = 15;
        private const int WALLS_COUNT_NOISE = 2;

        private const float WALL_PROBABILITY = 0.1f;
        private const float DESTRUCTIBLE_WALL_PROBABILITY = 0.4f;
        private const float RANDOM_POSITION_PROBABILITY = 0.2f;

        private int[] _angles = { 0, 90, 180, 270 };

        private Vector3Int probabilities;
        private Vector2Int attempts;

        public override void PaintRoom(MapTiles_SO map, Room room)
        {
            MapTileStyler styler = _levelData.Styler;

            List<Tile> tiles = map.Tiles;

            List<Tile> destructibleWallTiles = new List<Tile>();

            //List<Tile> groundTiles = tiles.FindAll(t => t.GetTileType() == TileType.Ground
            //&& !HasNeighborWithinDepth(t, TileType.Gate, 10, tiles)
            //&& !HasNeighborWithinDepth(t, DestructableTag.GateGraphics, 4, tiles)
            //&& t.CurrentTag == DestructableTag.None
            //&& t != null && t.BuiltTile != null);
            List<Tile> groundTiles = GetTilesByRules(tiles, PaintingRules);

            int groundTilesCount = tiles.FindAll(tiles => tiles.GetTileType() == TileType.Ground).Count;
            int numberOfWalls = Mathf.Max(
                Mathf.CeilToInt(
                    (float)groundTilesCount / (float)GROUND_TILES_COUNT_PER_WALL)
                + (Random.Range(-WALLS_COUNT_NOISE, WALLS_COUNT_NOISE)), WALLS_COUNT_NOISE);

            //int numberOfWalls = _levelData.DestructibleWallsRangePerRoom.RandomValue();

            // generate probabilities
            for (int i = 0; i < numberOfWalls; i++)
            {
                Tile selectedTile = GetTileBasedOnProbability(groundTiles, tiles, destructibleWallTiles);

                if (selectedTile == null)
                {
                    Debug.Log("No suitable tiles found for grass");
                    break;
                }

                if (selectedTile.BuiltTile == null)
                {
                    Debug.Log($"Tile ({selectedTile.Dimension.x}, {selectedTile.Dimension.y}) has no built tile");
                    continue;
                }

                groundTiles.Remove(selectedTile);
                destructibleWallTiles.Add(selectedTile);

                Vector3 position = selectedTile.BuiltTile.transform.position;
                selectedTile.CurrentTag = DestructableTag.DestructibleWalls;

                // Apply random rotation and scale here
                float randomAngle = _angles.RandomItem();
                float randomRotation = 0f;
                float randomScale = 0f;

                if (styler.DestructibleWallRandomRotationChance.IsChanceSuccessful())
                {
                    randomRotation = Random.Range(styler.DestructibleWallRandomRotationRange.x, styler.DestructibleWallRandomRotationRange.y);
                }

                if (styler.DestructibleWallRandomScaleChance.IsChanceSuccessful())
                {
                    randomScale = Random.Range(styler.DestructibleWallRandomScaleRange.x, styler.DestructibleWallRandomScaleRange.y);
                }
                else
                {
                    randomScale = 1f;
                }

                Quaternion rotation = Quaternion.Euler(0f, randomAngle + randomRotation, 0);
                Vector3 scale = Vector3.one * randomScale;

                DestructibleWall wall = Instantiate(styler.GetDestructibleWall(), position, rotation, room.Spawnables.SpawnablesParent);
                wall.transform.localScale = scale;

                wall.SetUp();
            }

            probabilities = Vector3Int.zero;
            attempts = Vector2Int.zero;
        }

        private Tile GetTileBasedOnProbability(List<Tile> groundTiles, List<Tile> tiles, List<Tile> destructibleWallTiles)
        {
            Dictionary<System.Func<Tile>, float> chances;
            attempts.x++;

            // if there are destructible walls, then include the probability of spawning a destructible wall next to a destructible wall
            if (destructibleWallTiles.Count > 0)
            {
                chances = new Dictionary<System.Func<Tile>, float>()
                {
                    {() => GetTileNextToWalls(groundTiles, tiles), WALL_PROBABILITY},
                    {() => GetTileNextToDestructibleWalls(groundTiles, tiles), DESTRUCTIBLE_WALL_PROBABILITY},
                    {() => GetRandomTile(groundTiles), RANDOM_POSITION_PROBABILITY}
                };
            }
            else
            {
                attempts.y++;
                return groundTiles.RandomItem();
            }

            attempts.y++;
            return ChanceSelector.SelectByChance(chances, () => GetRandomTile(groundTiles));
        }

        private Tile GetRandomTile(List<Tile> groundTiles)
        {
            //Debug.Log("Random position".Color(Colors.Green));
            probabilities.x = probabilities.x + 1;
            return groundTiles.RandomItem();
        }

        private Tile GetTileNextToWalls(List<Tile> groundTiles, List<Tile> tiles)
        {
            //Debug.Log("Walls".Color(Colors.Green));
            Tile selectedTile = null;

            selectedTile = groundTiles.FindAll(t => HasNeighborWithinDepth(t, TileType.Wall, 1, tiles)).RandomItem();

            if (selectedTile == null)
            {
                Debug.Log("No tile next to walls".Color(Colors.Red));
                selectedTile = tiles.RandomItem();
            }

            probabilities.y = probabilities.y + 1;

            return selectedTile;
        }

        private Tile GetTileNextToDestructibleWalls(List<Tile> groundTiles, List<Tile> tiles)
        {
            Tile selectedTile = null;

            probabilities.z = probabilities.z + 1;

            selectedTile = groundTiles.FindAll(t => HasNeighborWithinDepth(t, DestructableTag.DestructibleWalls, 1, tiles))
                .OrderBy(t => GetNumberOfNeighbouringTilesOfType(t, TileType.Wall, tiles))
                .ToList().RandomItem();

            if (selectedTile == null)
            {
                Debug.Log("No tile next to destructible walls. Went with a random tile".Color(Colors.Red));
                selectedTile = groundTiles.RandomItem();
            }

            return selectedTile;
        }
    }
}
