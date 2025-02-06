using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using Environment.MapMaker;
    using Utils;

    public class EnemiesPainter : RoomPainter
    {
        private List<Tile> _enemyTiles = new List<Tile>();

        public override void PaintRoom(MapTiles_SO map, Room room)
        {

        }

        public void PositionEnemies(EnemyWave wave, Room room)
        {
            List<EnemySpawnProfile> enemies = wave.Enemies;

            List<Tile> pureTiles = room.MapTiles.Tiles;
            List<Tile> tiles = pureTiles.FindAll(t => t.GetTileType() == TileType.Ground
            && !HasNeighborWithinDepth(t, TileType.Wall, 1, pureTiles)
            && !HasNeighborWithinDepth(t, TileType.Gate, 9, pureTiles)
            && t.BuiltTile != null);

            //tiles.ForEach(t => t.BuiltTile.transform.position += Vector3.up * 0.5f);

            _enemyTiles.ForEach(t => t.CurrentTag = DestructableTag.None);
            _enemyTiles.Clear();

            if (tiles == null || tiles.Count == 0)
            {
                Debug.Log("No suitable tiles found for enemies!");
                return;
            }

            Tile randomEnemyTile = tiles.FindAll(t => t.CurrentTag == DestructableTag.None
                                                && HasNeighborWithinDepthRange(t, DestructableTag.None, 3, 5, pureTiles)).RandomItem();
            randomEnemyTile.CurrentTag = DestructableTag.Enemy;

            if (randomEnemyTile == null)
            {
                Debug.LogError($"No initial tile 1");
                return;
            }

            Tile selectedTile = tiles.FindAll(t => HasNeighborWithinDepthRange(t, DestructableTag.Enemy, 3, 5, pureTiles)
                                                    && t.CurrentTag == DestructableTag.None).RandomItem();

            if (selectedTile == null)
            {
                Debug.LogError($"No initial tile 2");
                return;
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];

                selectedTile.CurrentTag = DestructableTag.Enemy;
                _enemyTiles.Add(selectedTile);

                Vector3 position = selectedTile.BuiltTile.transform.position;

                enemy.SetSpawnPoint(position);

                // select next point
                List<Tile> availableTiles = tiles.FindAll(t => HasNeighborWithinDepthRange(t, DestructableTag.Enemy, 3, 5, pureTiles)
                                                            && t.CurrentTag == DestructableTag.None
                                                            && !_enemyTiles.Contains(t));

                if (availableTiles.Count == 0)
                {
                    Debug.Log("CRITICAL: No suitable tiles found for enemies!");
                    return;
                }

                selectedTile = availableTiles.RandomItem();
            }
        }
    }
}
