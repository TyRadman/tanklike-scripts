using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    /// <summary>
    /// For walls only
    /// </summary>
    public class MapMakerWallArranger : TilesArranger
    {
        public static TileData EmptyTile;

        protected override void Awake()
        {
            base.Awake();
            GameObject tileObject = Instantiate(_manager.Styler.GetTile(TileTag.Wall_NoSides));
            EmptyTile = new TileData();
            EmptyTile.SetTileObject(tileObject, TileTag.Wall_NoSides);

            EmptyTile.TileObject.SetActive(false);
            EmptyTile.TileObject.name = "Empty";
        }

        public override void PlaceTile(ref TileData[,] tiles, int x, int y)
        {
            if (tiles[x, y].GatePart == TileData.GatePartType.PartOfGate)
            {
                _manager.UI.DisplayMessage("Can put tiles on a gate tile");
                return;
            }

            Vector3 position = tiles[x, y].TileObject.transform.position;

            // get the prefab of the tile to build from the styler
            GameObject tileToBuild = _manager.Styler.GetTile(TileTag.Wall_NoSides);
            GameObject tileObject = Instantiate(tileToBuild, position, tileToBuild.transform.rotation, _manager.Room.transform);
            TileData tile = new TileData();

            tile.SetTileObject(tileObject, TileTag.Wall_NoSides);
            tile.SetArranger(this);
            tile.SetName(x, y);
            tile.EnableBoxCollider(true);
            tile.SetGatePart(tiles[x, y].GatePart);

            base.PlaceTile(ref tiles, x, y);
            tiles[x, y] = tile;
            // arrange the surrounding tiles in accordance
            CheckTiles(ref tiles, x, y);
        }

        public override void RemoveTile(ref TileData[,] tiles, int x, int y)
        {
            base.RemoveTile(ref tiles, x, y);

            if (tiles[x, y] == null)
            {
                return;
            }

            Destroy(tiles[x, y].TileObject);
        }

        public void CheckTiles(ref TileData[,] tiles, int xIndex, int yIndex, bool checkSurroundingTilesOnly = true, bool checkForGrass = true)
        {
            TileData[,] tilesToCheck;

            if (checkSurroundingTilesOnly)
            {
                // get the surrounding tiles of the tile that was just painted
                TileData[,] surroundingTiles = GetSurroundingTiles(tiles, xIndex, yIndex, EmptyTile);
                // include the tile that was just added by the brush
                surroundingTiles[1, 1] = tiles[xIndex, yIndex];
                tilesToCheck = surroundingTiles;
            }
            else
            {
                tilesToCheck = tiles;
            }

            for (int y = 0; y < tilesToCheck.GetLength(1); y++)
            {
                for (int x = 0; x < tilesToCheck.GetLength(0); x++)
                {
                    // find the index of the current surrounding tile in relation to all the tiles
                    Vector2Int indicesInTiles = IndexOf(tiles, tilesToCheck[x, y]);

                    // if there is a tile in the surrounding tiles (and the index of the next cell in the world cells is greater than one i.e. isn't empty) then check for the surrounding cells and adjust them in accordance
                    if (tilesToCheck[x, y] != null && tiles.GetLength(0) > indicesInTiles.x && tiles.GetLength(1) > indicesInTiles.y && indicesInTiles.x >= 0 && indicesInTiles.y >= 0)
                    {
                        SetTileAccordingToSurroundings(tiles, indicesInTiles.x, indicesInTiles.y);

                        // true && !(true || false)
                        if (MapMakerManager.TagEquals(tilesToCheck[x, y].Tag, TileType.Ground) && checkForGrass)
                        {
                            _manager.GroundArranger.SetGrassTile(ref tiles, indicesInTiles.x, indicesInTiles.y);
                        }
                    }
                }
            }
        }

        public void SetTileAccordingToSurroundings(TileData[,] tiles, int x, int y)
        {
            // if it's not a wall tile, then there's nothing to do about it here
            if (!MapMakerManager.TagEquals(tiles[x, y].Tag, TileType.Wall))
            {
                return;
            }

            TileData[,] surroundingTiles = GetSurroundingTiles(tiles, x, y, EmptyTile);
            List<TileData> surroundingGrounds = ExistsOnSide(surroundingTiles, TileType.Ground);

            // if the surrounding tiles have a ground tile in them, then check for conditions otherwise, just put a no-sided tile
            if (TileExistsIn2DArray(surroundingTiles, TileType.Ground))
            {
                // for one sided walls
                if (surroundingGrounds.FindAll(t => t != null).Count == 1)
                {
                    ReplaceTileWithTileType(tiles, x, y, TileTag.Wall_OneSide);
                    Transform tile = tiles[x, y].TileObject.transform;
                    int rotation = surroundingGrounds.IndexOf(surroundingGrounds.Find(t => t != null));
                    tiles[x, y].TileObject.transform.eulerAngles = new Vector3(tile.eulerAngles.x, tile.eulerAngles.y - 90f * rotation, tile.eulerAngles.x);
                }

                // three sided walls
                if (surroundingGrounds.FindAll(t => t != null).Count == 3)
                {
                    ReplaceTileWithTileType(tiles, x, y, TileTag.Wall_ThreeSides);
                    Transform tile = tiles[x, y].TileObject.transform;
                    // we look for the tile that is not a ground
                    int rotation = surroundingGrounds.IndexOf(surroundingGrounds.Find(t => t == null));
                    tiles[x, y].TileObject.transform.eulerAngles = new Vector3(tile.eulerAngles.x, tile.eulerAngles.y - 90f * rotation, tile.eulerAngles.x);
                }

                // two sided
                if (surroundingGrounds.FindAll(t => t != null).Count == 2 && !AreSidesCorners(surroundingGrounds))
                {
                    ReplaceTileWithTileType(tiles, x, y, TileTag.Wall_TwoSides);
                    Transform tile = tiles[x, y].TileObject.transform;
                    // look for the side that has ground to face
                    int rotation = surroundingGrounds.IndexOf(surroundingGrounds.Find(t => t != null));
                    tiles[x, y].TileObject.transform.eulerAngles = new Vector3(tile.eulerAngles.x, tile.eulerAngles.y + (-90f * rotation), tile.eulerAngles.x);
                }

                // corners
                if (surroundingGrounds.FindAll(t => t != null).Count == 2 && AreSidesCorners(surroundingGrounds))
                {
                    ReplaceTileWithTileType(tiles, x, y, TileTag.Wall_Corner);
                    Transform tile = tiles[x, y].TileObject.transform;
                    int rotation;

                    if (surroundingGrounds.IndexOf(surroundingGrounds.FindAll(t => t != null)[0]) == 0 &&
                        surroundingGrounds.IndexOf(surroundingGrounds.FindAll(t => t != null)[1]) == 3)
                    {
                        rotation = 0;
                    }
                    else
                    {
                        rotation = surroundingGrounds.IndexOf(surroundingGrounds.FindAll(t => t != null)[0]) + 1;
                    }

                    tiles[x, y].TileObject.transform.eulerAngles = new Vector3(tile.eulerAngles.x, tile.eulerAngles.y + (-90f * rotation), tile.eulerAngles.x);
                }

                // if it's 4-sided
                if(surroundingGrounds.FindAll(t => t != null).Count == 4)
                {
                    ReplaceTileWithTileType(tiles, x, y, TileTag.Wall_FourSides);
                }

                CheckForCenterTile(surroundingGrounds, tiles, x, y);
            }
            // if it's a no sides wall
            else
            {
                CheckForCenterTile(surroundingGrounds, tiles, x, y);
            }
        }

        private void CheckForCenterTile(List<TileData> surroundingGrounds, TileData[,] tiles, int x, int y)
        {
            // if there are any surrounding walls
            if (surroundingGrounds.FindAll(t => t != null).Count == 0)
            {
                ReplaceTileWithTileType(tiles, x, y, TileTag.Wall_NoSides);
            }
        }

        public TileData ReplaceTileWithTileType(TileData[,] tiles, int x, int y, TileTag wallTag)
        {
            GameObject tileObject = Instantiate(_manager.Styler.Tiles.Find(t => t.Tag == wallTag).Tiles[0], _manager.Room.transform);
            TileData tile = new TileData();
            tile.SetTileObject(tileObject, wallTag);

            TileData_Gate parent = tiles[x, y].Parent as TileData_Gate;
            tile.SetName(x, y);
            tile.TileObject.transform.position = tiles[x, y].TileObject.transform.position;
            // set it as a gate if it is a gate
            tile.SetGatePart(tiles[x, y].GatePart);

            tile.EnableBoxCollider(true);
            tile.SetArranger(this);

            if (parent != null)
            {
                tile.Parent = parent;
                parent.AddTile(tile);
                parent.AllTiles.Remove(tiles[x, y]);
            }

            Destroy(tiles[x, y].TileObject);
            tiles[x, y] = tile;
            return tile;
        }

        private bool AreSidesCorners(List<TileData> tiles)
        {
            TileData firstTile = tiles.FindAll(t => t != null)[0];
            TileData secondTile = tiles.FindAll(t => t != null)[1];

            if (tiles[3] != null && tiles[0] != null) return true;

            return Mathf.Abs(tiles.IndexOf(firstTile) - tiles.IndexOf(secondTile)) == 1;
        }

        public static TileData[,] GetSurroundingTiles(TileData[,] tiles, int x, int y, TileData emptyTile, bool includeCenter = false)
        {
            TileData[,] newTiles = new TileData[3, 3];
            int maxX = tiles.GetLength(0) - 1;
            int maxY = tiles.GetLength(1) - 1;

            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    int newX = x + offsetX;
                    int newY = y + offsetY;

                    if (newX < 0 || newX > maxX || newY < 0 || newY > maxY || tiles[newX, newY] == null)
                    {
                        //print($"{newX},{newY}");
                        newTiles[offsetX + 1, offsetY + 1] = emptyTile;
                    }
                    else
                    {
                        newTiles[offsetX + 1, offsetY + 1] = tiles[newX, newY];
                    }
                }
            }

            if (!includeCenter) newTiles[1, 1] = null;

            return newTiles;
        }

        public static List<TileData> ExistsOnSide(TileData[,] tiles, TileType tileType)
        {
            List<TileData> list = new List<TileData>() { null, null, null, null };

            if (tiles[1, 0] != null)
            {
                if (MapMakerManager.TagEquals(tiles[1, 0].Tag,tileType)) list[0] = tiles[1, 0];
            }

            if (tiles[0, 1] != null)
            {
                if (MapMakerManager.TagEquals(tiles[0, 1].Tag, tileType)) list[3] = tiles[0, 1];
            }

            if (tiles[2, 1] != null)
            {
                if (MapMakerManager.TagEquals(tiles[2, 1].Tag, tileType)) list[1] = tiles[2, 1];
            }

            if (tiles[1, 2] != null)
            {
                if (MapMakerManager.TagEquals(tiles[1, 2].Tag, tileType)) list[2] = tiles[1, 2];
            }

            return list;
        }

        public bool TileExistsIn2DArray(TileData[,] tiles, TileType tileType)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    if (tiles[x, y] != null && MapMakerManager.TagEquals(tiles[x, y].Tag, tileType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static Vector2Int IndexOf(TileData[,] tiles, TileData tile)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    if (tiles[x, y] == tile)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }

            return new Vector2Int(-1, -1);
        }
    }
}
