using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class MenuSelectable : MonoBehaviour
    {
        [SerializeField] private SelectableAction _selectionAction;
        [SerializeField] private List<SelectableAction> _actions = new List<SelectableAction>()
        {
            new SelectableAction(){Direction = Direction.Up, Name = Direction.Up.ToString()},
            new SelectableAction(){Direction = Direction.Down, Name = Direction.Down.ToString()},
            new SelectableAction(){Direction = Direction.Left, Name = Direction.Left.ToString()},
            new SelectableAction(){Direction = Direction.Right, Name = Direction.Right.ToString()},
        };

        [Header("References")]
        [Tooltip("Other elements that should get highlighted when this object is highlighted")]
        [SerializeField] private List<Graphic> _highlightedGraphics;
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _highlightedColor;
        [SerializeField] private Animator _highlightAnimator;

        private readonly int _selectHash = Animator.StringToHash("Select");
        private readonly int _idleHash = Animator.StringToHash("Idle");

        public virtual void Highlight(bool highlight)
        {
            Color newColor = highlight ? _highlightedColor : _normalColor;
            _highlightedGraphics.ForEach(g => g.color = newColor);

            if(_highlightAnimator != null)
            {
                int animation = highlight ? _selectHash : _idleHash;
                _highlightAnimator.Play(animation);

            }
        }

        public virtual void InvokeAction(Direction direction = Direction.None)
        {
            if(direction == Direction.None)
            {
                _selectionAction.Action.Invoke();
            }
            else
            {
                _actions.Find(a => a.Direction == direction).Action.Invoke();
            }
        }

        public List<SelectableAction> GetActions()
        {
            return _actions;
        }

        public SelectableAction GetMainAction()
        {
            return _selectionAction;
        }

        public void SetText(string text)
        {
            if(_buttonText == null)
            {
                Debug.LogError($"No button text at {gameObject.name}");
                return;
            }

            _buttonText.text = text;
        }
    }
}
