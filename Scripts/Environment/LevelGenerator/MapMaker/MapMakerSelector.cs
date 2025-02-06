using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TankLike.Environment.MapMaker
{
    using Combat.Destructible;

    /// <summary>
    /// Handles the interactions between the user and the map maker.
    /// </summary>
    public class MapMakerSelector : MonoBehaviour
    {
        public const int TILE_SIZE = 2;
        public bool IsActive = true;

        [field: SerializeField] public Vector2Int LevelDimensions { get; private set; } = new Vector2Int(DEFAULT_DIMENSION, DEFAULT_DIMENSION);
        public List<TileData> DisplayTiles { get; private set; }

        [Header("References")]
        [SerializeField] private Room _roomPrefab;
        [SerializeField] private Transform _pointer;
        [SerializeField] private LayerMask _layerMaskForTiles;

        [Tooltip("The tags of the tiles that will be instantiated at start to be displayed whenever this type of tile is chosen to build.")]
        [SerializeField] private List<TileTag> _tagsToDisplay;
        [SerializeField] private List<TileTag> _tagsWithSizes;
        [SerializeField] private MapMakerManager _manager;

        private MapMakerMirroringController _mirrorController;
        private Camera _cam;
        private GameObject _tileToBuild;
        private TileType _currentType;
        private TileType _lastTileType;
        private Vector2 _lastPaintedDimensions;
        private Vector2Int _currentTileIndex;
        private int _currentBrushSize = 1;

        private const float RAY_DISTANCE = 300f;
        private const int DEFAULT_DIMENSION = 30;
        private const int BRUSH_SIZES_COUNT = 2;

        private void Awake()
        {
            _cam = Camera.main;
            BuildNewRoom();
            SetUpGround();
            InstantiatePrefabs();
        }

        public void SetUp(MapMakerManager manager)
        {
            _mirrorController = manager.MirroringController;

            _manager.Overlays.SetPointer(_pointer);
            _manager.Overlays.SetUpOverlayBoxes();
            _manager.Overlays.CreateDisplayTiles();

            _currentBrushSize = 0;
        }

        /// <summary>
        /// Updates the room's dimensions.
        /// </summary>
        /// <param name="x">Width</param>
        /// <param name="y">Height</param>
        public void UpdateLevelDimension(Vector2Int newDimensions)
        {
            LevelDimensions = newDimensions;

            BuildNewRoom();
            SetUpGround();
            
            _manager.Overlays.SetUpOverlayBoxes();
        }

        public void UpdateLevelDimensionsWithoutBuildingRoom(Vector2Int newDimensions)
        {
            LevelDimensions = newDimensions;

            BuildNewRoom();
            _manager.Overlays.SetUpOverlayBoxes();
            //SetUpGround();
        }

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                // if pointer is over a UI element, then skip
                return;
            }

            CheckForPointerPosition();
            CheckForInput();
        }

        private void CheckForInput()
        {
            if (Input.GetMouseButton(0))
            {
                bool isActionRepeated = _lastPaintedDimensions == _currentTileIndex && _currentType == _lastTileType;

                if (_tileToBuild == null || isActionRepeated)
                {
                    return;
                }

                _lastPaintedDimensions = _currentTileIndex;
                _lastTileType = _currentType;

                // apply mirroring to the build
                _mirrorController.BuildTile(_currentTileIndex.x, _currentTileIndex.y);
            }
        }

        public void BuildTileAtAxis(int x, int y)
        {
            if(_currentType == TileType.Overlay)
            {
                BuildOneTile(x, y);
                return;
            }

            int startHorizontalIndex = Mathf.Max(0, x - _currentBrushSize);
            int endHorizontalIndex = Mathf.Min(x + _currentBrushSize, LevelDimensions.x - 1);

            int startVerticalIndex = Mathf.Max(0, y - _currentBrushSize);
            int endVerticalIndex = Mathf.Min(y + _currentBrushSize, LevelDimensions.y - 1);

            for (int i = startHorizontalIndex; i <= endHorizontalIndex; i++)
            {
                for (int j = startVerticalIndex; j <= endVerticalIndex; j++)
                {
                    BuildOneTile(i, j);
                }
            }
        }

        private void BuildOneTile(int x, int y)
        {
            bool hasNoOverlays = _manager.Overlays.IsEmptyOverlay(x, y);
            bool isTileToPlaceAlreadyExists = _currentType == MapMakerManager.ToTileType(_manager.AllTiles[x, y].Tag);

            // if the tile we're placing is the same type as the tile already placed AND there are no overlays, then stop
            if (isTileToPlaceAlreadyExists && hasNoOverlays)
            {
                return;
            }

            if (_tileToBuild == null)
            {
                return;
            }

            switch (_currentType)
            {
                case TileType.Gate:
                    _manager.GateArranger.PlaceTile(ref _manager.AllTiles, x, y);
                    break;
                case TileType.Wall:
                    _manager.WallArranger.PlaceTile(ref _manager.AllTiles, x, y);
                    break;
                case TileType.Ground:
                    _manager.GroundArranger.PlaceTile(ref _manager.AllTiles, x, y);
                    break;
                case TileType.Overlay:
                    _manager.Overlays.PlaceTile(ref _manager.AllTiles, x, y, _manager.Overlays.CurrentOverlayType);
                    break;
            }
        }

        public void CheckForPointerPosition()
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, RAY_DISTANCE, _layerMaskForTiles))
            {
                GameObject tile = hit.collider.gameObject;

                if (tile == null)
                {
                    return;
                }

                _pointer.position = new Vector3(tile.transform.position.x, 0f, tile.transform.position.z);
                TileData tileData = FindTileDataByObject(tile);
                _currentTileIndex = IndexOfTile(tileData, _manager.AllTiles);
            }
        }

        public void BuildNewRoom()
        {
            // create a parent for the room
            if (_manager.Room != null)
            {
                Destroy(_manager.Room.gameObject);
            }

            _manager.Room = Instantiate(_roomPrefab, Vector3.zero, Quaternion.identity);
            // set dimensions for the array
            _manager.AllTiles = new TileData[LevelDimensions.x, LevelDimensions.y];
        }

        /// <summary>
        /// Lay down the ground tiles based on the dimensions and center them in the middle of the world
        /// </summary>
        public void SetUpGround()
        {
            // set the starting position at the lower left corner to make building easier
            Vector3 startingPosition = GetStartingPositionForTiles();
            GameObject groundTile = _manager.Styler.GetTile(TileTag.Ground);

            // build the tiles
            for (int i = 0; i < LevelDimensions.y; i++)
            {
                for (int j = 0; j < LevelDimensions.x; j++)
                {
                    Vector3 position = startingPosition + new Vector3(j, 0f, i) * TILE_SIZE;
                    GameObject tileObject = Instantiate(groundTile, _manager.Room.transform);
                    tileObject.transform.position = position;

                    TileData tile = new TileData();
                    tile.SetTileObject(tileObject, TileTag.Ground);

                    tile.SetArranger(_manager.GroundArranger);
                    tile.SetName(j, i);
                    _manager.AllTiles[j, i] = tile;
                }
            }
        }

        /// <summary>
        /// Gets the bottom left tile in the level tiles.
        /// </summary>
        /// <returns>The position of the first tile.</returns>
        public Vector3 GetStartingPositionForTiles()
        {
            float x = -((float)(LevelDimensions.x / 2f)) * TILE_SIZE + TILE_SIZE / 2;
            float y = -((float)(LevelDimensions.y / 2f)) * TILE_SIZE + TILE_SIZE / 2;

            return new Vector3(x, 0f, y);
        }

        private void InstantiatePrefabs()
        {
            DisplayTiles = new List<TileData>();

            for (int i = 0; i < _tagsToDisplay.Count; i++)
            {
                GameObject tileObject = Instantiate(_manager.Styler.Tiles.Find(t => t.Tag == _tagsToDisplay[i]).Tiles[0], _pointer);

                TileData tile = new TileData();
                TileType type = MapMakerManager.ToTileType(_tagsToDisplay[i]);

                int brushSize = 0;

                if(_tagsWithSizes.Contains(_tagsToDisplay[i]))
                {
                    brushSize = BRUSH_SIZES_COUNT;
                }

                tile.SetTileObject(tileObject, _tagsToDisplay[i], brushSize, type, _pointer.position);
                DisplayTiles.Add(tile);
                tile.TileObject.SetActive(false);
                tile.EnableBoxCollider(false);
            }
        }

        #region Bruch size
        public void UpdateBrushSize(int size)
        {
            _currentBrushSize = size;

            TileData tilesData = DisplayTiles.Find(t => t.Type == _currentType);
            tilesData.BrushObjects.ForEach(og => og.ForEach(o => o.SetActive(false)));

            if (_currentBrushSize == 0)
            {
                return;
            }

            int brushCounts = Mathf.Min(_currentBrushSize, tilesData.BrushObjects.Count);

            for (int i = 0; i < brushCounts; i++)
            {
                tilesData.BrushObjects[i].ForEach(o => o.SetActive(true));
            }
        }
        #endregion

        public void SetTileToBuild(TileType type)
        {
            // cache the display tile that has the same type to change to 
            GameObject tileObject = DisplayTiles.Find(t => MapMakerManager.TagEquals(t.Tag, type)).TileObject;

            TileData tilesData = DisplayTiles.Find(t => t.Type == _currentType);

            if (tilesData != null)
            {
                tilesData.DisableBrushes();
            }

            // set the selected tile as the current selection tile
            SetCurrentSelection(type, tileObject);

            TileData data = DisplayTiles.Find(t => t.Type == _currentType);
            data.BrushObjects.ForEach(og => og.ForEach(o => o.SetActive(false)));

            if (_currentBrushSize == 0)
            {
                return;
            }

            int brushCounts = Mathf.Min(_currentBrushSize, data.BrushObjects.Count);

            for (int i = 0; i < brushCounts; i++)
            {
                data.BrushObjects[i].ForEach(o => o.SetActive(true));
            }
        }

        public void SetOverLayToBuild(DestructableTag type)
        {
            TileData data = DisplayTiles.Find(t => t.Type == _currentType);

            if (data != null)
            {
                data.DisableBrushes();
            }

            SetCurrentSelection(TileType.Overlay, _manager.Overlays.GetDisplayTile(type));
            _manager.Overlays.CurrentOverlayType = type;
        }

        private void SetCurrentSelection(TileType type, GameObject tileObject)
        {
            if (_tileToBuild != null)
            {
                _tileToBuild.SetActive(false);
            }

            _tileToBuild = tileObject;

            _tileToBuild.SetActive(true);
            _currentType = type;
        }

        #region Utilities
        public static Vector2Int IndexOfTile(TileData tile, TileData[,] tiles)
        {
            Vector2Int index = Vector2Int.zero;

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tile == tiles[i, j])
                    {
                        index = new Vector2Int(i, j);
                        return index;
                    }
                }
            }

            return index;
        }

        public TileData FindTileDataByObject(GameObject tileObject)
        {
            TileData[,] tiles = _manager.AllTiles;

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    if (tiles[i, j].TileObject == tileObject)
                    {
                        return tiles[i, j];
                    }
                }
            }

            Debug.LogError($"Tile object {tileObject.name} doesn't exist in the mapmaker manager list");
            return null;
        }

        public Transform GetPointer()
        {
            return _pointer;
        }
        #endregion
    }
}