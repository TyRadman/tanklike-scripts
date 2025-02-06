using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using Environment.MapMaker;
    using Utils;

    public class ExplosivesPainter : RoomPainter
    {
        [SerializeField] private GameObject _explosivePrefab;

        public override void PaintRoom(MapTiles_SO map, Room room)
        {
            List<Tile> tiles = map.Tiles;

            List<Tile> groundTiles = GetTilesByRules(tiles, PaintingRules);
            //List<Tile> groundTiles = tiles.FindAll(t => t.GetTileType() == TileType.Ground &&
            //!HasNeighborWithinDepth(t, TileType.Wall, 1, tiles) && 
            //!HasNeighborWithinDepth(t, TileType.Gate, 10, tiles) && 
            //HasNeighborWithinExactDepthRange(t, TileType.Wall, 2, 3, tiles));

            if (groundTiles.Count == 0)
            {
                Debug.Log("No suitable tiles found for explosives");
                return;
            }

            groundTiles.RemoveAll(t => t.Overlays.Count > 0);

            int explosivesCount = _levelData.ExplosivesRangePerRoom.RandomValue();


            for (int i = 0; i < explosivesCount; i++)
            {
                Tile tile = groundTiles.RandomItem();

                groundTiles.Remove(tile);

                if (tile == null)
                {
                    Debug.Log("No suitable tiles found for explosives");
                    break;
                }

                if(tile.BuiltTile == null)
                {
                    Debug.Log($"Tile ({tile.Dimension.x}, {tile.Dimension.y}) has no built tile");
                    continue;
                }

                Vector3 position = tile.BuiltTile.transform.position;
                tile.CurrentTag = DestructableTag.Explosive;

                IAimAssistTarget explosive = Instantiate(_explosivePrefab, position, Quaternion.identity, room.Spawnables.SpawnablesParent).GetComponent<IAimAssistTarget>();
                explosive.AssignAsTarget(room);
            }
        }
    }
}
