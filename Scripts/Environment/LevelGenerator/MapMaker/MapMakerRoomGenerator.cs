using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    using Combat.Destructible;
    using Environment.LevelGeneration;
    using LevelGeneration;

    public class MapMakerRoomGenerator : MonoBehaviour, IRoomGenerator
    {
        private MapMakerManager _manager;

        private void Awake()
        {
            _manager = GetComponent<MapMakerManager>();
        }

        [ContextMenu("Load Map")]
        public void BuildRoom(MapTiles_SO map, LevelData level, Room room, BuildConfigs configs = null)
        {
            // remove previous tile
            //for (int i = 0; i < _manager.AllTiles.GetLength(0); i++)
            //{
            //    for (int j = 0; j < _manager.AllTiles.GetLength(1); j++)
            //    {
            //        Destroy(_manager.AllTiles[i, j].TileObject);
            //    }
            //}

            //_manager.Overlays.ClearOverlays();
            
            _manager.Selector.UpdateLevelDimensionsWithoutBuildingRoom(map.Size);
            room = _manager.Room;


            //int xDimension = map.Size.x;
            //int yDimension = map.Size.y;
            //float startPositionX = -((float)MapMakerSelector.TILE_SIZE * (float)xDimension / 2f + (float)MapMakerSelector.TILE_SIZE / 2f);
            //float startPositionY = -((float)MapMakerSelector.TILE_SIZE * (float)yDimension / 2f + (float)MapMakerSelector.TILE_SIZE / 2f);
            Vector3 startingPosition = _manager.Selector.GetStartingPositionForTiles();//new Vector3(startPositionX, 0f, startPositionY);

            for (int i = 0; i < map.Tiles.Count; i++)
            {
                Tile tile = map.Tiles[i];
                GameObject tileToBuildPrefab = level.Styler.GetTile(tile.Tag);

                if (tileToBuildPrefab == null)
                {
                    Debug.LogError($"Tile of type {tile.Tag} doesn't exist in {level.Styler.name} styler");
                    Debug.Break();
                }

                GameObject tileToBuild = Instantiate(tileToBuildPrefab, room.transform);

                // rename it
                string tileName = $"{tile.Dimension.x},{tile.Dimension.y} ({tile.Tag})";
                tileToBuild.name = tileName;

                tileToBuild.transform.position = startingPosition + new Vector3(tile.Dimension.x, 0f, tile.Dimension.y) * MapMakerSelector.TILE_SIZE;

                TileData tileData = new TileData();
                tileData.SetTileObject(tileToBuild, tile.Tag);

                // set the position of the tile
                tileData.Dimension = tile.Dimension;
                // set the tile rotation
                tileToBuild.transform.eulerAngles += Vector3.up * tile.Rotation;
                // assign it to the 2D array
                _manager.AllTiles[tile.Dimension.x, tile.Dimension.y] = tileData;

                SetAssigner(tileData);

                if (tile.Overlays.Count > 0)
                {
                    //Selector.SetOverLayToBuild(tile.Overlay);
                    tile.Overlays.ForEach(o => _manager.Overlays.PlaceTile(ref _manager.AllTiles, tile.Dimension.x, tile.Dimension.y, o));
                }
            }
        }

        public void SetAssigner(TileData tile)
        {
            int tileTag = (int)tile.Tag;

            if (tileTag >= 0 && tileTag <= 3)
            {
                tile.SetArranger(_manager.GroundArranger);
            }
            else if (tileTag >= 1 && tileTag <= 6)
            {
                tile.SetArranger(_manager.GroundArranger);
            }
            else if (tileTag == (int)TileTag.Gate)
            {
                tile.SetArranger(_manager.GateArranger);
            }
        }
    }
}
