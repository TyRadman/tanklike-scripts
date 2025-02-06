using System.Collections;
using System.Collections.Generic;
using TankLike.Combat.Abilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class SelectSuperAbility : AbilitySelectionPanel
    {
        [System.Serializable]
        public class SuperAbilitySelection
        {
            public int Index;
            public SuperAbilityHolder Ability;
            public Image Image;
            public Button Button;
        }

        [SerializeField] private List<SuperAbilitySelection> _superAbilitySelections = new List<SuperAbilitySelection>();

        private SuperAbilitySelection _selectedItem;

        private void Start()
        {
            foreach (var item in _superAbilitySelections)
            {
                item.Image.sprite = item.Ability.GetIcon();
            }
        }


        public void SetSuperAbility(int index)
        {
            _selectedItem = _superAbilitySelections.Find(d => d.Index == index);
            SuperAbilityHolder ability = _selectedItem.Ability;
            _abilityNameText.text = ability.Name;
            _abilityNameDescription.text = ability.Description;
            PlayerPrefs.SetInt(nameof(_selectionData.SuperAbilityHolders), index);
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
