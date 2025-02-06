using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    /// <summary>
    /// Any cell that can be highlighted
    /// </summary>
    public interface ICellSelectable
    {
        public void Highlight();
        public void Unhighlight();
        public ICellSelectable Navigate(Direction direction);
    }

    [System.Serializable]
    public struct Connection
    {
        [HasInterface(typeof(ICellSelectable))] 
        public MonoBehaviour Target;
        public Direction Direction;
    }
}
