using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UI.Inventory;
using UnityEngine.InputSystem;

namespace TankLike.Minimap
{
    public class MapInfoDisplayer : MonoBehaviour
    {
        [SerializeField] private List<MapIconInfo> _iconInfos;

        private void FillIconsNamings()
        {
            List<MinimapManager.MinimapIcon> icons = GameManager.Instance.MinimapManager.GetMinimapIcons().FindAll(i => i.Type != MinimapIconType.Wall && i.Type != MinimapIconType.Gate);

            for (int i = 0; i < icons.Count; i++)
            {
                _iconInfos[i].Fill(icons[i].Type.ToString(), icons[i].Icon, icons[i].Color);
            }
        }
    }
}
