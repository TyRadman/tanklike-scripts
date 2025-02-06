using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    [System.Serializable]
    public class TileData_Gate : TileData
    {

        public List<TileData> AllTiles = new List<TileData>();
        public GameObject Particles;

        public void AddTile(TileData tile)
        {
            AllTiles.Add(tile);
            tile.Parent = this;

            int startIndex = AllTiles.Count - 1;
            // check for empty slots in case they were removed during wall arrangement 
            for (int i = startIndex; i >= 0; i--)
            {
                if(AllTiles[i] == null)
                {
                    AllTiles.RemoveAt(i);
                }
            }
        }
    }
}
