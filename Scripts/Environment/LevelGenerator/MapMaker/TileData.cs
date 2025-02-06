using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    [System.Serializable]
    public class TileData : TileParent
    {
        public enum GatePartType
        {
            None = 0, PartOfGate = 1, DoublePartOfGate = 4
        }

        [field: SerializeField] public GatePartType GatePart { set; get; } = GatePartType.None;
        [SerializeField] protected BoxCollider _collider;
        [HideInInspector] public TilesArranger Arranger;
        [HideInInspector] public TileData Parent;
        [HideInInspector] public Vector2Int Dimension;
        [field: SerializeField] public GameObject TileObject { get; private set; }

        /// <summary>
        /// The objects represent the excess tiles that communicate the size of the brush.
        /// </summary>
        public List<List<GameObject>> BrushObjects { get; private set; } = new List<List<GameObject>>();
        public bool HasBrushes { get; private set; }

        public void SetTileObject(GameObject tile, TileTag tileTag)
        {
            TileObject = tile;
            _collider = tile.GetComponent<BoxCollider>();
            Tag = tileTag;
        }

        public void SetTileObject(GameObject tile, TileTag tileTag, int brushSizes, TileType type, Vector3 position)
        {
            TileObject = tile;
            TileObject.transform.position = position;
            _collider = tile.GetComponent<BoxCollider>();
            Tag = tileTag;
            Type = type;

            int tilesCount = 8;
            float tileSize = MapMakerSelector.TILE_SIZE;

            HasBrushes = brushSizes > 0;

            for (int i = 0; i < brushSizes; i++)
            {
                List<GameObject> sizeObjects = new List<GameObject>();
                int loops = i * 2 + 3;

                for (int k = 0; k < loops; k++)
                {
                    for (int l = 0; l < loops; l++)
                    {
                        bool isOnBorder = k == 0 || k == loops - 1 || l == 0 || l == loops - 1;

                        if (!isOnBorder)
                        {
                            continue;
                        }

                        float x = (k - (loops - 1) / 2) * tileSize;
                        float y = (l - (loops - 1) / 2) * tileSize;
                        Vector3 newPosition = new Vector3(x, 0f, y);
                        GameObject createdTile = Object.Instantiate(tile, newPosition, Quaternion.identity, tile.transform.parent);
                        sizeObjects.Add(createdTile);
                        createdTile.GetComponent<BoxCollider>().enabled = false;
                        createdTile.SetActive(false);
                    }
                }

                BrushObjects.Add(sizeObjects);
                tilesCount += 8;
            }
        }

        public void EnableBoxCollider(bool enable)
        {
            _collider.enabled = enable;
        }

        public void SetGatePart(GatePartType gatePart)
        {
            GatePart = gatePart;
        }

        public void SetName(int x, int y)
        {
            TileObject.name = $"{x},{y}";
            Dimension = new Vector2Int(x, y);
        }

        public void SetArranger(TilesArranger arranger)
        {
            Arranger = arranger;
        }

        internal void DisableBrushes()
        {
            if (HasBrushes)
            {
                BrushObjects.ForEach(og => og.ForEach(o => o.SetActive(false)));
            }
        }
    }
}
