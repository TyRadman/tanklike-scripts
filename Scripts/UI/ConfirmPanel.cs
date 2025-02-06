using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace TankLike.UI
{
    public class ConfirmPanel : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private MenuSelectable _yesButton;
        [SerializeField] private MenuSelectable _noButton;
        [SerializeField] private string _defaultMainText;

        [Header("Animations")]
        [SerializeField] private Animator _animator;

        //[SerializeField] private MenuSelectable _firstSelectedItem;

        private MenuSelectable _currentSelectable;
        private int _currentPlayerIndex;

        private readonly int _showHash = Animator.StringToHash("Show");
        private readonly int _hideHash = Animator.StringToHash("Hide");

        public void Init(int playerIndex, System.Action OnYesButton, System.Action OnNoButton, string messageText)
        {
            // Set player index
            _currentPlayerIndex = playerIndex;

            // Set message text
            if (string.IsNullOrEmpty(messageText))
            {
                _messageText.text = _defaultMainText;
            }
            else
            {
                _messageText.text = messageText;
            }

            //// Set _noButton as the current selectable
            if (_currentSelectable != null && _currentSelectable == _yesButton)
            {
                _currentSelectable.Highlight(false);
            }

            _currentSelectable = _noButton;
            _currentSelectable.Highlight(true);

            // Add listeners to yes and no buttons
            _yesButton.GetMainAction().RemoveAllListeners();
            _noButton.GetMainAction().RemoveAllListeners();

            _yesButton.GetMainAction().AddListener(new UnityAction(OnYesButton));
            _yesButton.GetMainAction().AddListener(new UnityAction(OnPanelClose));

            if (OnNoButton != null)
            {
                _noButton.GetMainAction().AddListener(new UnityAction(OnNoButton));
            }

            _noButton.GetMainAction().AddListener(new UnityAction(OnPanelClose));

            // Play show animation
            _animator.Play(_showHash, -1, 0f);

            // Enable inputs
            GameManager.Instance.PlayersManager.GetPlayer(_currentPlayerIndex).UIController.EnableConfirmPanelController(true);
        }

        public void Select()
        {
            _currentSelectable.InvokeAction();
        }

        public void Return()
        {
            _noButton.InvokeAction();
        }

        public void Navigate(Direction direction)
        {
            _currentSelectable.InvokeAction(direction);
        }

        private void OnPanelClose()
        {
            _animator.Play(_hideHash, -1, 0f);
            // TODO: Check if we need this
            GameManager.Instance.PlayersManager.GetPlayer(_currentPlayerIndex).UIController.EnableConfirmPanelController(false);
        }

        public void HighLightSelectable(MenuSelectable cell)
        {
            // dehighlight the previous cell
            _currentSelectable.Highlight(false);
            // set the new cell and highlight it
            _currentSelectable = cell;
            cell.Highlight(true);
        }
    }
}
