using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    public class TileData_New
    {
        public enum GatePartType
        {
            None = 0, PartOfGate = 1, DoublePartOfGate = 4
        }

        public TileTag TileTag;
        [field: SerializeField] public GatePartType GatePart { set; get; } = GatePartType.None;
        public TileType TileType;
        [SerializeField] protected BoxCollider _collider;
        [HideInInspector] public TilesArranger Arranger;
        [HideInInspector] public TileData Parent;
        [HideInInspector] public Vector2Int Dimension;
        public GameObject Tile;

        public void SetTile(GameObject tile)
        {
            Tile = tile;
            _collider = tile.GetComponent<BoxCollider>();
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
            Tile.name = $"{x},{y}";
            Dimension = new Vector2Int(x, y);
        }

        public void SetArranger(TilesArranger arranger)
        {
            Arranger = arranger;
        }
    }
}
