using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace TankLike
{
    public class PlayerSkillsSelectionReadyButton : MonoBehaviour, ICellSelectable
    {
        [SerializeField] private List<Connection> _connections;
        [SerializeField] private Color _onClickedColor;
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI _inputSignifierText;
        [Header("Events")]
        [SerializeField] private UnityEvent _onHighlightAction;
        [SerializeField] private UnityEvent _onUnhighlightAction;

        public void Initiate()
        {
            Unhighlight();
        }

        public void Highlight()
        {
            _text.transform.localScale = Vector3.one * 1.1f;
            _onHighlightAction?.Invoke();
        }

        public ICellSelectable Navigate(Direction direction)
        {
            MonoBehaviour target = _connections.Find(c => c.Direction == direction).Target;

            if (target is not null and ICellSelectable selectable)
            {
                return selectable;
            }

            return null;
        }

        public void Unhighlight()
        {
            _text.transform.localScale = Vector3.one;
            _onUnhighlightAction?.Invoke();
        }

        internal void OnButtonClicked()
        {
            _text.color = _onClickedColor;
        }

        internal void OnButtonUnclicked()
        {
            _text.color = Colors.White;
        }

        public void UpdateSignifierText(string text)
        {
            _inputSignifierText.text = text;
        }
    }
}
