using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Inventory
{
    [System.Serializable]
    public class TabReferenceUI : MonoBehaviour
    {
        public string TabName;
        public TabUI Tab;
        public Navigatable Navigator;

        public void SetUp()
        {
            Tab.Dehighlight();
            Tab.SetName(TabName);
            Navigator.gameObject.SetActive(false);
        }
    }
}
