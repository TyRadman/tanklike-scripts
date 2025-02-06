using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.HUD
{
    public class PlayerStatsModifiersDisplayer : MonoBehaviour
    {
        [SerializeField] private List<StatModifierIcon> _icons = new List<StatModifierIcon>();

        private int _nextAvailableIconIndex = 0;

        public void SetUp()
        {
            _icons.ForEach(i => i.Disable());
        }

        // TODO: doing it with sprites isn't the cleanest way. Maybe enums?
        /// <summary>
        /// Adds an icon that represents a stat modifier to the list of icons that will be display on top of the health bar.
        /// </summary>
        /// <param name="statIcon"></param>
        public void AddIcon(StatIconReference statIcon)
        {
            if(statIcon == null)
            {
                Debug.LogError("An icon or a stat modifier missing.");
                return;
            }

            // if there is a stat icon of the type pass, then there is no need to add another one
            if(_icons.Exists(s => s.StatModifierSprite == statIcon))
            {
                return;
            }

            StatModifierIcon icon = _icons[_nextAvailableIconIndex];

            icon.SetIconSprite(statIcon);
            icon.Enable();

            _nextAvailableIconIndex++;
        }

        public void RemoveIcon(StatIconReference statIcon)
        {
            StatModifierIcon icon = _icons.Find(i => i.StatModifierSprite == statIcon);

            if (icon == null)
            {
                return;
            }

            icon.Disable();
            _nextAvailableIconIndex--;
        }
    }
}
