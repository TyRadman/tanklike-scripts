using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    using Combat.Destructible;
    using Utils;

    public class OverlaysArranger : MonoBehaviour
    {
        [System.Serializable]
        public class OverlayInfo
        {
            public DestructableTag Tag;
            public GameObject Model;
            public Vector2Int PositionIndex;
        }

        [System.Serializable]
        public class Overlay
        {
            public List<DestructableTag> CurrentOverlayTypes = new List<DestructableTag>();
            [field: SerializeField] public Vector2Int Dimension { get; private set; }
            public List<OverlayInfo> Info = new List<OverlayInfo>();

            public void SetModel(List<OverlayInfo> info, Transform parent, Vector3 position)
            {
                for (int i = 0; i < info.Count; i++)
                {
                    float offset = MapMakerSelector.TILE_SIZE / 2f;

                    Vector3 modelPosition = position - new Vector3(offset, 0f, offset);

                    float x = TILE_SIZE + info[i].PositionIndex.x * TILE_SIZE;
                    float y = TILE_SIZE + info[i].PositionIndex.y * TILE_SIZE;

                    modelPosition += new Vector3(x, 0f, y);

                    OverlayInfo overlayInfo = new OverlayInfo();
                    overlayInfo.Model = Instantiate(info[i].Model, modelPosition, Quaternion.identity, parent);
                    overlayInfo.Model.SetActive(false);
                    overlayInfo.Tag = info[i].Tag;
                    Info.Add(overlayInfo);
                }
            }

            public void ShowVisual(DestructableTag tag)
            {
                Info.Find(t => t.Tag == tag).Model.SetActive(true);

                if (!CurrentOverlayTypes.Exists(t => t == tag))
                {
                    CurrentOverlayTypes.Add(tag);
                }
            }

            public GameObject GetDisplayTile(DestructableTag tag)
            {
                return Info.Find(t => t.Tag == tag).Model;
            }

            public void DisableVisuals()
            {
                CurrentOverlayTypes = new List<DestructableTag>();
                Info.ForEach(t => t.Model.SetActive(false)); 
            }

            public void SetDimension(int x, int y)
            {
                Dimension = new Vector2Int(x, y);
            }
        }

        public const float TILE_SIZE = 0.5f;
        [SerializeField] private List<OverlayInfo> OverlayInfos;
        [SerializeField] private List<Overlay> CurrentOverlays = new List<Overlay>();
        private Overlay DisplayOverlay = new Overlay();
        [SerializeField] private MapMakerManager _manager;
        [SerializeField] private GameObject _overlayBoxPrefab;
        public DestructableTag CurrentOverlayType;
        private Transform _parent;

        public void SetPointer(Transform parent)
        {
            return;
            _parent = parent;
        }

        public void SetUpOverlayBoxes()
        {
            int xNum = _manager.Selector.LevelDimensions.x;
            int yNum = _manager.Selector.LevelDimensions.y;

            Vector3 startingPosition = _manager.Selector.GetStartingPositionForTiles();

            if (_parent == null)
            {
                _parent = new GameObject("Overlays").transform;
            }

            if (CurrentOverlays != null)
            {
                CurrentOverlays.ForEach(o => o.Info.ForEach(i => Destroy(i.Model)));
            }

            CurrentOverlays.Clear();

            for (int i = 0; i < xNum; i++)
            {
                for (int j = 0; j < yNum; j++)
                {
                    Vector3 position = startingPosition + new Vector3(i, 0f, j) * MapMakerSelector.TILE_SIZE;

                    Overlay overlay = new Overlay();

                    overlay.SetDimension(i, j);

                    overlay.SetModel(OverlayInfos, _parent, position);
                    CurrentOverlays.Add(overlay);
                }
            }
        }

        public void CreateDisplayTiles()
        {
            Overlay overlay = new Overlay();
            overlay.SetModel(OverlayInfos, _manager.Selector.GetPointer(), Vector3.zero);
            DisplayOverlay = overlay;
        }

        public void PlaceTile(ref TileData[,] tiles, int x, int y, DestructableTag tag)
        {
            bool isGroundTile = MapMakerManager.TagEquals(tiles[x, y].Tag, TileType.Ground);
            bool isPartOfGate = tiles[x, y].GatePart != TileData.GatePartType.None;

            // if the tile we're setting as the one having a destructible is not a ground tile, then stop
            if (!isGroundTile || isPartOfGate)
            {
                _manager.UI.DisplayMessage($"Can only place {tag}s on ground tiles");
                return;
            }

            Overlay overlayToShow = CurrentOverlays.Find(t => t.Dimension.x == x && t.Dimension.y == y);

            if(overlayToShow == null)
            {
                Debug.Log($"No overlay at {x}, {y}".Color(Colors.Red));
                return;
            }

            bool tileHasSpawnPoint = overlayToShow.CurrentOverlayTypes.Exists(o => o == DestructableTag.SpawnPoint);
            bool isNewOverlaySpawnPoint = tag == DestructableTag.SpawnPoint;

            if (tileHasSpawnPoint && !isNewOverlaySpawnPoint)
            {
                _manager.UI.DisplayMessage($"Can't place {tag} on a spawn point");
                return;
            }

            // set the tile as one having a destructible
            overlayToShow.ShowVisual(tag);
        }

        public void RemoveTile(int x, int y)
        {
            Overlay overlay = CurrentOverlays.Find(t => t.Dimension.x == x && t.Dimension.y == y);
            overlay.CurrentOverlayTypes = new List<DestructableTag>();
            overlay.DisableVisuals();
        }

        public bool IsEmptyOverlay(int x, int y)
        {
            if(CurrentOverlays == null)
            {
                Debug.Log("Why");
            }

            if (CurrentOverlays.Find(t => t.Dimension.x == x && t.Dimension.y == y) == null)
            {
                Debug.Log($"No overlays at {x} && {y}");
            }

            return CurrentOverlays.Find(t => t.Dimension.x == x && t.Dimension.y == y).CurrentOverlayTypes.Count == 0;
        }

        public GameObject GetDisplayTile(DestructableTag tag)
        {
            DisplayOverlay.DisableVisuals();
            DisplayOverlay.ShowVisual(tag);
            return DisplayOverlay.GetDisplayTile(tag);
        }

        public List<DestructableTag> GetOverlayTypeAtIndex(int x, int y)
        {
            List<DestructableTag> destructibleTags = new List<DestructableTag>();
            CurrentOverlays.Find(t => t.Dimension.x == x && t.Dimension.y == y).CurrentOverlayTypes.ForEach(o => destructibleTags.Add(o));
            return destructibleTags;
        }

        public void ClearOverlays()
        {
            for (int i = 0; i < CurrentOverlays.Count; i++)
            {
                CurrentOverlays[i].DisableVisuals();
            }
        }
    }
}
