using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.UI.Inventory
{
    public class InputKeyDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _keyText;
        [SerializeField] private TextMeshProUGUI _acitonText;

        public void Fill(string key, string action)
        {
            _keyText.text = key + ":";
            _acitonText.text = action;
        }
    }
}
