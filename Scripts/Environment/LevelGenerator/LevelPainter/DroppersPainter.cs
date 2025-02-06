using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using Environment.MapMaker;
    using TankLike.Combat.Destructible;
    using Utils;

    public class DroppersPainter : RoomPainter
    {
        private MapTileStyler _styler;

        public override void PaintRoom(MapTiles_SO map, Room room)
        {
            if (room is BossRoom)
            {
                return;
            }

            List<Tile> tiles = map.Tiles;

            _styler = _levelData.Styler;

            List<Tile> groundTiles = GetTilesByRules(tiles, PaintingRules);
            //List<Tile> groundTiles = tiles.FindAll(t => t.GetTileType() == TileType.Ground 
            //&& !HasNeighborWithinDepth(t, TileType.Gate, 7, tiles)
            //&& !HasNeighborWithinDepth(t, DestructableTag.GateGraphics, 3, tiles)
            //&& t != null && t.BuiltTile != null);

            int count = _levelData.DroppersRange.RandomValue();
            int cratesCount = Mathf.CeilToInt((float)count * _levelData.CratesToRocksChance);
            int rocksCount = count - cratesCount;


            List<Tile> crateSuitableTiles = groundTiles.FindAll(g => GetNumberOfNeighbouringTilesOfType(g, TileType.Wall, tiles).IsInRange(4, 8));

            SpawnDropppers(cratesCount, DestructableTag.Crate, crateSuitableTiles, tiles, room, 4);

            groundTiles.RemoveAll(t => t.CurrentTag == DestructableTag.Crate);
            List<Tile> rockSuitableTiles = groundTiles.FindAll(g => GetNumberOfNeighbouringTilesOfType(g, TileType.Wall, tiles).IsInRange(2, 8));

            SpawnDropppers(rocksCount, DestructableTag.Rock, rockSuitableTiles, tiles, room, 2);
        }

        private void SpawnDropppers(int count, DestructableTag dropperType, List<Tile> suitableTiles, List<Tile> tiles, Room room, int minWallsCount)
        {
            if (suitableTiles.Count == 0)
            {
                Debug.Log("No suitable tiles found for droppers");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                Tile selectedTile = suitableTiles.RandomItem();

                if (selectedTile == null)
                {
                    Debug.Log("No suitable tile");
                    break;
                }

                suitableTiles.Remove(selectedTile);

                Vector3 position = selectedTile.BuiltTile.transform.position;
                selectedTile.CurrentTag = dropperType;

                GameObject dropper = _styler.OverlayReferences.Find(o => o.Tag == dropperType).OverlayObject;

                if (dropper != null)
                {
                    Transform spawnedDropper = Instantiate(dropper, position, Quaternion.identity, room.Spawnables.SpawnablesParent).transform;
                    GameManager.Instance.DestructiblesManager.SetDestructibleValues(spawnedDropper.GetComponent<IDropper>());
                    spawnedDropper.GetComponent<IAimAssistTarget>().AssignAsTarget(room);
                }
            }
        }
    }
}
