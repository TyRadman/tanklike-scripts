using System.Collections;
using System.Collections.Generic;
using TankLike.UI.Inventory;
using UnityEngine;

namespace TankLike.UI
{
    public abstract class SelectableEntityUI : MonoBehaviour
    {
        public enum SelectableType
        {
            Cell, Group
        }

        [field: SerializeField] public SelectableType Type { get; private set; }

        public virtual void MoveSelection(Direction direction, ref SelectableEntityUI cell, int playerIndex = 0)
        {

        }

        public virtual void HighLight(bool highlight)
        {

        }

        /// <summary>
        /// This overload is for the tabs.
        /// </summary>
        /// <param name="hightlight"></param>
        /// <param name="setting"></param>
        public virtual void HighLight(bool hightlight, TabActivationSettings setting)
        {

        }
    }
}
