using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    public class GressArranger : TilesArranger
    {
        public override void PlaceTile(ref TileData[,] tiles, int xIndex, int yIndex)
        {
            #region Check Overlays
            // check if the tile we're deleting has an overlay, in which case it would most likely still be a ground tile
            if (!_manager.Overlays.IsEmptyOverlay(xIndex, yIndex))
            {
                _manager.Overlays.RemoveTile(xIndex, yIndex);
                return;
            }
            #endregion

            TileData[,] tilesToCheck = MapMakerWallArranger.GetSurroundingTiles(tiles, xIndex, yIndex, MapMakerWallArranger.EmptyTile, true);
            
            // if the tile we're checking is not a ground tile, then turn it into a ground tile by name so that the ground tile alignment doesn't get influenced by a non-ground tile that will be deleted
            if (!MapMakerManager.TagEquals(tiles[xIndex, yIndex].Tag, TileType.Ground)) tiles[xIndex, yIndex].Tag = TileTag.Ground;

            for (int y = 0; y < tilesToCheck.GetLength(1); y++)
            {
                for (int x = 0; x < tilesToCheck.GetLength(0); x++)
                {
                    // if there is a tile in the surrounding tiles (and the index of the next cell in the world cells is greater than one i.e. isn't empty) then check for the surrounding cells and adjust them in accordance
                    if (tilesToCheck[x, y] != null && (MapMakerManager.TagEquals(tilesToCheck[x, y].Tag, TileType.Ground) || (x == 1 && y == 1)))
                    {
                        // find the index of the current surrounding tile in relation to all the tiles
                        Vector2Int indicesInTiles = MapMakerWallArranger.IndexOf(tiles, tilesToCheck[x, y]);
                        SetGrassTile(ref tiles, indicesInTiles.x, indicesInTiles.y);
                    }
                }
            }

            _manager.WallArranger.CheckTiles(ref tiles, xIndex, yIndex, false, false);
        }

        public override void RemoveTile(ref TileData[,] tiles, int x, int y)
        {
            Destroy(tiles[x, y].TileObject);
        }

        public void SetGrassTile(ref TileData[,] tiles, int x, int y)
        {
            TileTag tileToBuild = TileTag.Ground;
            float rotation = 0f;
            bool buildTile = false;
            TileData[,] surroundingTile = MapMakerWallArranger.GetSurroundingTiles(tiles, x, y, MapMakerWallArranger.EmptyTile);

            if (IsInnerCornerTile(ref surroundingTile, ref rotation))
            {
                buildTile = true;
                tileToBuild = TileTag.Ground_InCorner;
            }
            else if (IsOuterCornerTile(ref surroundingTile, ref rotation))
            {
                buildTile = true;
                tileToBuild = TileTag.Ground_OutCorner;
            }
            else if(IsOneSidedTile(ref surroundingTile, ref rotation))
            {
                buildTile = true;
                tileToBuild = TileTag.Ground_OneSide;
            }
            else
            {
                buildTile = true;
                tileToBuild = TileTag.Ground;
            }

            if (!buildTile) return;

            GameObject tileObject = _manager.Styler.Tiles.Find(t => t.Tag == tileToBuild).Tiles[0];
            CreateTile(ref tiles, x, y, rotation, tileObject, tileToBuild);
        }

        private bool IsInnerCornerTile(ref TileData[,] tiles, ref float rotation)
        {
            TileType type = TileType.Wall;
            bool isLowerLeftCorner = MapMakerManager.TagEquals(tiles[0, 1].Tag, type) && MapMakerManager.TagEquals(tiles[1, 0].Tag, type);
            bool isLowerRighCorner = MapMakerManager.TagEquals(tiles[2, 1].Tag, type) && MapMakerManager.TagEquals(tiles[1, 0].Tag, type);
            bool isUpperRighCorner = MapMakerManager.TagEquals(tiles[2, 1].Tag, type) && MapMakerManager.TagEquals(tiles[1, 2].Tag, type);
            bool isUpperLeftCorner = MapMakerManager.TagEquals(tiles[0, 1].Tag, type) && MapMakerManager.TagEquals(tiles[1, 2].Tag, type);
            bool hasTwoStraightTilesOnly = GetSurroundingTilesStraightTilesCount(tiles, type) == 2;

            if (isLowerLeftCorner) rotation = 0f;
            else if (isLowerRighCorner) rotation = 270f;
            else if (isUpperRighCorner) rotation = 180f;
            else if (isUpperLeftCorner) rotation = 90f;

            return hasTwoStraightTilesOnly && (isLowerLeftCorner || isUpperLeftCorner || isUpperRighCorner || isLowerRighCorner);
        }

        private bool IsOuterCornerTile(ref TileData[,] tiles, ref float rotation)
        {
            TileType type = TileType.Wall;
            bool isLowerLeftCorner = MapMakerManager.TagEquals(tiles[0, 0].Tag, type);
            bool isLowerRighCorner = MapMakerManager.TagEquals(tiles[2, 0].Tag, type);
            bool isUpperRighCorner = MapMakerManager.TagEquals(tiles[2, 2].Tag, type);
            bool isUpperLeftCorner = MapMakerManager.TagEquals(tiles[0, 2].Tag, type);
            bool hasTwoStraightTilesOnly = GetSurroundingTilesStraightTilesCount(tiles, type) == 0;

            if (isLowerLeftCorner) rotation = 0f;
            else if (isLowerRighCorner) rotation = 270f;
            else if (isUpperRighCorner) rotation = 180f;
            else if (isUpperLeftCorner) rotation = 90f;

            return hasTwoStraightTilesOnly && (isLowerLeftCorner || isUpperLeftCorner || isUpperRighCorner || isLowerRighCorner);
        }

        private bool IsOneSidedTile(ref TileData[,] tiles, ref float rotation)
        {
            if (GetSurroundingTilesStraightTilesCount(tiles, TileType.Wall) != 1) return false;

            List<TileData> walls = MapMakerWallArranger.ExistsOnSide(tiles, TileType.Wall);
            rotation = walls.IndexOf(walls.Find(t => t != null)) * -90f;

            return true;
        }

        /// <summary>
        /// Calculates the number of tiles horizontally and vertically around a tile (not the diagonal ones.) The 2D array has to be 3*3
        /// </summary>
        /// <param name="tiles">The 2D array of tiles</param>
        /// <param name="type">The tile type to compare to</param>
        /// <returns></returns>
        private int GetSurroundingTilesStraightTilesCount(TileData[,] tiles, TileType type)
        {
            int existingNumber = 0;

            if (MapMakerManager.TagEquals(tiles[1, 0].Tag, type)) existingNumber++;
            if (MapMakerManager.TagEquals(tiles[2, 1].Tag, type)) existingNumber++;
            if (MapMakerManager.TagEquals(tiles[1, 2].Tag, type)) existingNumber++;
            if (MapMakerManager.TagEquals(tiles[0, 1].Tag, type)) existingNumber++;

            return existingNumber;
        }

        private void CreateTile(ref TileData[,] tiles, int x, int y, float angle, GameObject objectToBuild, TileTag tag)
        {
            //base.PlaceTile(ref tiles, x, y);
            GameObject tileObject = Instantiate(objectToBuild, _manager.Room.transform);
            TileData tile = new TileData();
            tile.SetTileObject(tileObject, tag);

            tile.SetName(x, y);
            tile.TileObject.transform.position = tiles[x, y].TileObject.transform.position;
            tile.TileObject.transform.eulerAngles += Vector3.up * angle;
            tile.SetArranger(this);
            Destroy(tiles[x, y].TileObject);
            tiles[x, y] = tile;
            tile.EnableBoxCollider(true);
        }
    }
}
