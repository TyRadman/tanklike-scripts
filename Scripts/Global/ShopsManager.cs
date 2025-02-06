using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using UI;

    public class ShopsManager : MonoBehaviour, IManager
    { 
        [SerializeField] private ToolsNavigator _toolsShop;
        //[field: SerializeField] public Workshop_InteractableArea WorkShopArea { get; private set; }

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            if (_toolsShop != null)
            {
                _toolsShop.SetUp();
            }
        }

        public void Dispose()
        {
            IsActive = false;

            if (_toolsShop != null)
            {
                _toolsShop.Dispose();
            }
        }
        #endregion
    }
}
