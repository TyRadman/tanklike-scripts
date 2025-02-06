using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TankLike.UI
{
    using TankLike.Combat.Abilities;

    public class SelectBoostAbility : AbilitySelectionPanel
    {
        [System.Serializable]
        public class BoostAbilitySelection
        {
            public int Index;
            public BoostAbilityHolder Ability;
            public Image Image;
            public Button Button;
        }

        [SerializeField] private List<BoostAbilitySelection> _boostAbilitySelections = new List<BoostAbilitySelection>();

        private BoostAbilitySelection _selectedItem;

        private void Start()
        {
            foreach (var item in _boostAbilitySelections)
            {
                item.Image.sprite = item.Ability.GetIcon();
            }
        }

        public void SetBoostAbility(int index)
        {
            _selectedItem = _boostAbilitySelections.Find(d => d.Index == index);
            BoostAbilityHolder ability = _selectedItem.Ability;
            _abilityNameText.text = ability.Name;
            _abilityNameDescription.text = ability.Description;
            PlayerPrefs.SetInt(nameof(_selectionData.BoostAbilityHolders), index);
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
