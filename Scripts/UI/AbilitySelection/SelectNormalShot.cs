using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI
{
    using Combat;
    using TMPro;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class SelectNormalShot : AbilitySelectionPanel
    {
        [System.Serializable]
        public class NormalShotSelection
        {
            public int Index;
            public Weapon Weapon;
            public Image Image;
            public Button Button;
        }

        [SerializeField] private List<NormalShotSelection> _normalShotSelections = new List<NormalShotSelection>();

        private NormalShotSelection _selectedItem;

        private void Start()
        {
            foreach (var item in _normalShotSelections)
            {
                item.Image.sprite = item.Weapon.GetIcon();
            }
        }

        public void SetNormalShot(int index)
        {
            _selectedItem = _normalShotSelections.Find(d => d.Index == index);
            Weapon selectedWeapon = _selectedItem.Weapon;

            // TODO: must use the holder's name and description
            //_abilityNameText.text = selectedWeapon.Name;
            //_abilityNameDescription.text = selectedWeapon.Description;
            PlayerPrefs.SetInt(nameof(_selectionData.Weapons), index);
            _nextButton.gameObject.SetActive(true);
        }

        public void OnPanelActivated()
        {
            if (_selectedItem != null)
            {
                EventSystem.current.SetSelectedGameObject(_selectedItem.Button.gameObject);
            }
        }
    }
}
