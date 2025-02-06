using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TankLike.UI.Signifiers
{
    public class UIActionSignifier : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _actionText;
        [SerializeField] private TextMeshProUGUI _actionKeyText;
        public bool IsActive { get; private set; } = false;
        public bool IsParentSignifier { get; set; } = false;

        public void SetText(string action, string key)
        {
            _actionText.enabled = true;
            _actionKeyText.enabled = true;

            _actionText.text = action;
            _actionKeyText.text = key;

            IsActive = true;
        }

        public void Disable()
        {
            _actionText.enabled = false;
            _actionKeyText.enabled = false;
            IsActive = false;
        }
    }
}
