using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TankLike.UI
{
    using TankLike.Combat.Abilities;

    public class SelectHoldAbility : AbilitySelectionPanel
    {
        [System.Serializable]
        public class HoldAbilitySelection
        {
            public int Index;
            public HoldAbilityHolder Ability;
            public Image Image;
            public Button Button;
        }

        [SerializeField] private List<HoldAbilitySelection> _holdAbilitySelections = new List<HoldAbilitySelection>();

        private HoldAbilitySelection _selectedItem;

        private void Start()
        {
            foreach (var item in _holdAbilitySelections)
            {
                item.Image.sprite = item.Ability.GetIcon();
            }
        }

        public void SetHoldAbility(int index)
        {
            _selectedItem = _holdAbilitySelections.Find(d => d.Index == index);
            HoldAbilityHolder ability = _selectedItem.Ability;
            _abilityNameText.text = ability.Name;
            _abilityNameDescription.text = ability.Description;
            PlayerPrefs.SetInt(nameof(_selectionData.HoldAbilityHolders), index);
            _nextButton.gameObject.SetActive(true);
        }

        public void OnPanelActivated()
        {
            if(_selectedItem != null)
            {
                EventSystem.current.SetSelectedGameObject(_selectedItem.Button.gameObject);
            }
        }
    }
}
