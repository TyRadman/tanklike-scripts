using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    /// <summary>
    /// For the gates only
    /// </summary>
    public class MapMakerGateArranger : TilesArranger
    {
        [SerializeField] private int _gateFromEdgeOffset = 4;
        [SerializeField] private int _gateWidth = 3;
        private int _cornerLimit;
        [SerializeField] private GameObject _gateParticle;
        [SerializeField] private TileData_Gate _currentTile;
        [SerializeField] private int _spacing = 1;

        protected override void Awake()
        {
            base.Awake();
            // make sure the gate width is an even number
            if (_gateWidth % 2 == 1) _gateWidth++;
            _cornerLimit = _gateFromEdgeOffset + _gateWidth / 2;
        }

        public override void PlaceTile(ref TileData[,] tiles, int x, int y)
        {
            if (!TileIsValid(tiles, x, y))
            {
                return;
            }

            base.PlaceTile(ref tiles, x, y);

            // create the gate 
            ReplaceTileWithTileType(ref tiles, x, y);

            // set the tiles around the gate as wall tiles
            SetTilesWithinRangeToGateTile(tiles, x, y);
            _manager.CorrectWalls();
        }

        public override void RemoveTile(ref TileData[,] tiles, int x, int y)
        {
            print($"{tiles[x, y].TileObject.name} is of type {tiles[x, y].Tag}");
            List<TileData> allTiles = ((TileData_Gate)tiles[x, y]).AllTiles;
            Destroy((tiles[x, y] as TileData_Gate).Particles);

            for (int i = 0; i < allTiles.Count; i++)
            {
                if (allTiles[i].GatePart == TileData.GatePartType.DoublePartOfGate)
                {
                    allTiles[i].SetGatePart(TileData.GatePartType.PartOfGate);
                    continue;
                }

                Vector3 position = allTiles[i].TileObject.transform.position;
                Vector2Int name = allTiles[i].Dimension;
                TileData tileToBuild = _manager.Selector.DisplayTiles.Find(t => MapMakerManager.TagEquals(t.Tag, TileType.Ground));
                //TileData_New tileToBuild = _manager.Tiles.TilesPrefabs.Find(t => t.TileType == TileType.Ground);

                GameObject tileObject = Instantiate(tileToBuild.TileObject, position, tileToBuild.TileObject.transform.rotation);
                TileData tile = new TileData();
                tile.SetTileObject(tileObject, TileTag.Ground);
                //TileData_New tile = Instantiate(tileToBuild, position, tileToBuild.transform.rotation);

                // find the index of the tile in the set of the AllTiles
                Vector2Int indices = MapMakerSelector.IndexOfTile(allTiles[i], tiles);
                tiles[indices.x, indices.y] = tile;
                tile.TileObject.transform.parent = _manager.Room.transform;

                tile.EnableBoxCollider(true);
                tile.SetGatePart(TileData.GatePartType.None);
                tile.SetName(name.x, name.y);
                tile.SetArranger(_manager.GroundArranger);
                Destroy(allTiles[i].TileObject);
            }
        }

        private void ReplaceTileWithTileType(ref TileData[,] tiles, int x, int y)
        {
            GameObject tileObject = Instantiate(_manager.Styler.GetTile(TileTag.Gate), _manager.Room.transform);
            _currentTile = new TileData_Gate();
            _currentTile.SetTileObject(tileObject, TileTag.Gate);

            _currentTile.SetArranger(this);
            _currentTile.SetName(x, y);
            _currentTile.TileObject.transform.position = tiles[x, y].TileObject.transform.position;
            tiles[x, y] = _currentTile;
            _currentTile.EnableBoxCollider(true);

            // set the gate's rotation
            float angle = GetTileAngle(ref tiles, x, y);
            _currentTile.TileObject.transform.eulerAngles = new Vector3(0f, angle, 0f);
        }

        private float GetTileAngle(ref TileData[,] tiles, int x, int y)
        {
            float angle = 0f;

            if (x == 0) angle = -90f;
            else if (x == tiles.GetLength(0) - 1) angle = 90f;
            else if (y == 0) angle = 180f;
            else if (y == tiles.GetLength(1) - 1) angle = 0f;

            return angle;
        }

        public bool IsNotOnEdge(TileData[,] tiles, int x, int y)
        {
            int rows = tiles.GetLength(0) - 1;
            int columns = tiles.GetLength(1) - 1;

            //bool IsUpperLeftCorner = x <= _gateWidth && y > _gateWidth && y > ;
            bool IsLowerLeftCorner = x <= _cornerLimit && y == 0 || y <= _cornerLimit && x == 0;
            bool IsLowerRightCorner = x >= rows - _cornerLimit && y == 0 || y <= _cornerLimit && x == rows;
            bool IsUpperRightCorner = x >= rows - _cornerLimit && y == columns || y >= columns - _cornerLimit && x == rows;
            bool IsUpperLeftCorner = x <= _cornerLimit && y == columns || y >= columns - _cornerLimit && x == 0;

            bool IsNotOnEdge = x > 0 && x < rows && y > 0 && y < columns;

            return IsLowerLeftCorner || IsLowerRightCorner || IsUpperRightCorner || IsUpperLeftCorner || IsNotOnEdge;
        }

        private bool IsPartOfGate(TileData[,] tiles, int x, int y)
        {
            int loops = _gateWidth + _spacing * 2;

            for (int i = -loops / 2; i <= loops / 2; i++)
            {
                if (x + i > tiles.GetLength(0) - 1 || x + i < 0) continue;

                if (tiles[x + i, y].GatePart == TileData.GatePartType.PartOfGate) return true;
            }

            for (int i = -loops / 2; i <= loops / 2; i++)
            {
                if (y + i > tiles.GetLength(1) - 1 || y + i < 0) continue;

                if(tiles[x, y + i].GatePart == TileData.GatePartType.PartOfGate) return true;
            }

            return false;
        }

        private bool TileIsValid(TileData[,] tiles, int x, int y)
        {
            if (IsNotOnEdge(tiles, x, y))
            {
                _manager.UI.DisplayMessage("Selected one of the level's edges");
                return false;
            }

            if (IsPartOfGate(tiles, x, y))
            {
                _manager.UI.DisplayMessage("This tile is a part of a gate");
                return false;
            }

            return true;
        }

        private void SetTilesWithinRangeToGateTile(TileData[,] tiles, int xIndex, int yIndex)
        {
            int rows = tiles.GetLength(0);
            int cols = tiles.GetLength(1);

            // we set the tiles that fall within the gate's width as a part of a gate, and also neighbouring cells so that two gates don't overlap, plus one extra tile for the wall seperating the gates
            if (xIndex == 0)
            {
                for (int x = 0; x < _gateFromEdgeOffset; x++)
                {
                    for (int y = yIndex - _gateWidth / 2 - 1; y <= yIndex + _gateWidth / 2 + 1; y++)
                    {
                        PlaceWall(ref tiles, x, y, y == yIndex - _gateWidth / 2 - 1 || y == yIndex + _gateWidth / 2 + 1);
                        CreateParticles(tiles, x, y, true, x == _gateFromEdgeOffset - 1 && y == yIndex);
                        SetGatePartType(tiles, x, y, y <= yIndex + _gateWidth / 2 + 1 && y >= yIndex - _gateWidth / 2 - 1);
                        if (tiles[x, y] != null) _currentTile.AddTile(tiles[x, y]);
                    }
                }
            }
            else if(xIndex == rows - 1)
            {
                for (int x = rows - _gateFromEdgeOffset; x < rows; x++)
                {
                    for (int y = yIndex - _gateWidth / 2 - 1; y <= yIndex + _gateWidth / 2 + 1; y++)
                    {
                        PlaceWall(ref tiles, x, y, y == yIndex - _gateWidth / 2 - 1 || y == yIndex + _gateWidth / 2 + 1);
                        CreateParticles(tiles, x, y, true, x == rows - _gateFromEdgeOffset && y == yIndex);
                        SetGatePartType(tiles, x, y, y < yIndex + _gateWidth / 2 + 1 && y > yIndex - _gateWidth / 2 - 1);
                        if (tiles[x, y] != null) _currentTile.AddTile(tiles[x, y]);
                    }
                }
            }
            else if (yIndex == 0)
            {
                for (int y = 0; y < _gateFromEdgeOffset; y++)
                {
                    for (int x = xIndex - _gateWidth / 2 - 1; x <= xIndex + _gateWidth / 2 + 1; x++)
                    {
                        PlaceWall(ref tiles, x, y, x == xIndex - _gateWidth / 2 - 1 || x == xIndex + _gateWidth / 2 + 1);
                        CreateParticles(tiles, x, y, false, y == _gateFromEdgeOffset - 1 && x == xIndex);
                        SetGatePartType(tiles, x, y, x < xIndex + _gateWidth / 2 + 1 && x > xIndex - _gateWidth / 2 - 1);
                        if (tiles[x, y] != null) _currentTile.AddTile(tiles[x, y]);
                    }
                }
            }
            else if (yIndex == cols - 1)
            {
                for (int y = cols - _gateFromEdgeOffset; y < cols; y++)
                {
                    for (int x = xIndex - _gateWidth / 2 - 1; x <= xIndex + _gateWidth / 2 + 1; x++)
                    {
                        PlaceWall(ref tiles, x, y, x == xIndex - _gateWidth / 2 - 1 || x == xIndex + _gateWidth / 2 + 1);
                        CreateParticles(tiles, x, y, false, y == cols - _gateFromEdgeOffset - 1 && x == xIndex);
                        SetGatePartType(tiles, x, y, x < xIndex + _gateWidth / 2 + 1 && x > xIndex - _gateWidth / 2 - 1);
                        if (tiles[x, y] != null) _currentTile.AddTile(tiles[x, y]);
                    }
                }
            }
        }

        private void PlaceWall(ref TileData[,] tiles, int x, int y, bool condition)
        {
            if (condition)
            {
                _manager.WallArranger.PlaceTile(ref tiles, x, y);
            }
            // check if the other tiles are ground tiles
            else if(tiles[x, y] != _currentTile && !MapMakerManager.TagEquals(tiles[x, y].Tag, TileType.Ground))
            {
                _manager.GroundArranger.PlaceTile(ref tiles, x, y);
            }
        }

        private void CreateParticles(TileData[,] tiles, int x, int y, bool rotate, bool condition)
        {
            if (!condition) return;

            Transform particle = Instantiate(_gateParticle, tiles[x, y].TileObject.transform.position, 
                tiles[x, y].TileObject.transform.rotation).transform;

            if (rotate) particle.eulerAngles += Vector3.up * 90f;

            _currentTile.Particles = particle.gameObject;
        }

        private void SetGatePartType(TileData[,] tiles, int x, int y, bool condition)
        {
            tiles[x, y].SetGatePart(TileData.GatePartType.PartOfGate);
        }
    }
}
