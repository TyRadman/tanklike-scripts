using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike.Environment
{
    /// <summary>
    /// Holds information about the room for the level generator like the gates it has, and the directions of each gate
    /// </summary>
    public class RoomGatesInfo : MonoBehaviour
    {
        [field: SerializeField] public List<GateInfo> Gates;

        public void SortGates()
        {
            Gates = Gates.OrderBy(g => (int)g.Direction).ToList();
            Gates.ForEach(g => g.Gate.GateID = (int)(g.Direction) / 90);
        }
    }
}
