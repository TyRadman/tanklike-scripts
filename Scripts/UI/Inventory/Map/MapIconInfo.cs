using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.Minimap
{
    public class MapIconInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _icon;

        public void Fill(string text, Sprite sprite, Color color)
        {
            _text.text = text;
            _icon.sprite = sprite;
            _icon.color = color;
        }
    }
}
