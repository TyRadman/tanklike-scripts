using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Inventory
{
    public class TabContent : MonoBehaviour
    {
        // the first thing to be selected. Could be a group, a cell, or nothing at all
        [SerializeField] protected SelectableEntityUI _firstSelectable;
        [SerializeField] protected GameObject _content;

        #region Input Methods
        public virtual void Navigate(Direction direction)
        {

        }

        public virtual void Select()
        {

        }

        public virtual void Return()
        {

        }
        #endregion
    }
}
