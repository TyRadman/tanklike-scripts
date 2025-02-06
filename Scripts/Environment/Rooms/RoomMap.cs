using System.Collections;
using System.Collections.Generic;
using TankLike.Environment.MapMaker;
using UnityEngine;

namespace TankLike.Environment
{
    [CreateAssetMenu(fileName = "Room Map", menuName = "Level/ Room Map")]
    public class RoomMap : ScriptableObject
    {
        public TileData[,] Tiles;

        public void FillMap(TileData[,] tiles)
        {
            
        }
    }
}
