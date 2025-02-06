using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    public abstract class TilesArranger : MonoBehaviour
    {
        [HideInInspector] public TileData TileToBuild; 
        protected MapMakerManager _manager;

        protected virtual void Awake()
        {
            _manager = GetComponent<MapMakerManager>();
        }

        public virtual void PlaceTile(ref TileData[,] tiles, int x, int y)
        {
            if (tiles[x, y] == null)
            {
                Debug.LogError($"No tiles at {x}, {y}");
            }

            if (tiles[x, y].Arranger == null)
            {
                Debug.LogError($"No arrange at tile {x}, {y}");
            }

            // remove the tile that exists on the same axis
            tiles[x, y].Arranger.RemoveTile(ref tiles, x, y);
        }

        public virtual void RemoveTile(ref TileData[,] tiles, int x, int y)
        {

        }

        public virtual void OnBuild()
        {

        }
    }
}
