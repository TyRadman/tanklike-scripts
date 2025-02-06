using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TankLike.UI
{
    public class AbilitySelectionPanel : MonoBehaviour
    {
        [SerializeField] protected AbilitySelectionData _selectionData;
        [SerializeField] protected TextMeshProUGUI _abilityNameText;
        [SerializeField] protected TextMeshProUGUI _abilityNameDescription;

        [SerializeField] protected GameObject _nextButton;

        public void Setup()
        {
            gameObject.SetActive(false);
            _nextButton.SetActive(false);
        }

        public void ShowNextButton()
        {
            _nextButton.SetActive(true);
        }
    }
}
