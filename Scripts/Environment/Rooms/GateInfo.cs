using System.Collections;
using System.Collections.Generic;
using TankLike.UI;
using UnityEngine;

namespace TankLike.Environment
{
    [System.Serializable]
    public class GateInfo
    {
        [field: SerializeField] public GateDirection Direction { set; get; }
        [field: SerializeField] public GateDirection LocalDirection { set; get; }
        public bool IsConnected = false;
        public RoomGate Gate;

        public void SetLocalDirection(GateDirection direction)
        {
            LocalDirection = direction;
            Gate.name = $"{Direction}, {LocalDirection}";
        }

        public void SetDirection(GateDirection direction)
        {
            Direction = direction;
            Gate.Direction = direction;
            Gate.name = $"{Direction}, {LocalDirection}";
        }

        public void SetConnection(Room room, RoomGate gate)
        {
            IsConnected = true;
            Gate.SetConnection(room, gate);
        }
    }
}
