using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TankLike.UI.InGame
{
    using UI.Signifiers;

    public class InteractableMenuNavigatable : Navigatable, IInput
    {
        [SerializeField] private InteractableMenuButton _buttonPrefab;
        [SerializeField] private Transform _buttonsParent;
        [SerializeField] private TextMeshProUGUI _messageText;

        private List<InteractableMenuButton> _buttons = new List<InteractableMenuButton>();
        private MenuSelectable _highlightedButton;
        private Transform _camera;

        private const float MESSAGE_DISPLAY_DURATION = 2f;

        private void Start()
        {
            _camera = Camera.main.transform;
        }

        void Update()
        {
            // so that the text only rotates on the x-axis
            Vector3 targetPostition = new Vector3(transform.position.x, _camera.position.y, _camera.position.z);
            transform.LookAt(targetPostition);
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            GameManager.Instance.InputManager.DisableInputs((playerIndex + 1) % 2);
            GameManager.Instance.InputManager.EnableUIInput(playerIndex);

            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Cancel.name).performed += Return;
            UIMap.FindAction(c.UI.Submit.name).performed += Select;
            UIMap.FindAction(c.UI.Navigate_Up.name).performed += NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed += NavigateDown;

            SetUpSignifiers();
        }

        public void DisposeInput(int playerIndex)
        {
            GameManager.Instance.InputManager.EnableInput(ActionMap.Player);

            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Cancel.name).performed -= Return;
            UIMap.FindAction(c.UI.Submit.name).performed -= Select;
            UIMap.FindAction(c.UI.Navigate_Up.name).performed -= NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed -= NavigateDown;
        }

        public override void NavigateDown(InputAction.CallbackContext _)
        {
            Navigate(Direction.Down);
        }

        public override void NavigateUp(InputAction.CallbackContext _)
        {
            Navigate(Direction.Up);
        }

        private void Return(InputAction.CallbackContext _)
        {
            OnClosed?.Invoke();
            Close(PlayerIndex);
        }  
        
        private void Select(InputAction.CallbackContext _)
        {
            _highlightedButton.InvokeAction();
        }
        #endregion

        /// <summary>
        /// Selects a non-active button, creates one if there are no active buttons, sets it up and returns its Event to be manually set up.
        /// </summary>
        /// <param name="text">The text that the button will display.</param>
        public UnityEvent SetUpButton(string text)
        {
            InteractableMenuButton button = _buttons.Find(b => !b.IsActive);

            if(button == null)
            {
                AddButton();
                button = _buttons.Find(b => !b.IsActive);
            }

            button.Button.SetText(text);

            button.IsActive = true;
            UnityEvent mainActionEvent = button.Button.GetMainAction().Action;
            return mainActionEvent;
        }

        private void AddButton()
        {
            // if there is a free button, then don't create an additional button
            if (_buttons.Exists(b => !b.IsActive))
            {
                return;
            }

            InteractableMenuButton button = Instantiate(_buttonPrefab, _buttonsParent);
            button.gameObject.name = $"{_buttons.Count}";
            _buttons.Add(button);
        }

        public void ConnectButtonsInput()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                InteractableMenuButton currentHolder = _buttons[i];
                InteractableMenuButton nextHolder = _buttons[(i + 1) % _buttons.Count];
                InteractableMenuButton previousHolder = _buttons[i == 0 ? _buttons.Count - 1 : i - 1];

                ConnectHolderToHolderWithDirection(currentHolder, nextHolder, Direction.Down);
                ConnectHolderToHolderWithDirection(nextHolder, currentHolder, Direction.Up);
                ConnectHolderToHolderWithDirection(currentHolder, previousHolder, Direction.Up);
                ConnectHolderToHolderWithDirection(previousHolder, currentHolder, Direction.Down);
            }
        }

        private void ConnectHolderToHolderWithDirection(InteractableMenuButton baseHolder, InteractableMenuButton holderToConnectTo, Direction direction)
        {
            UnityEvent currentDirectionAction = baseHolder.Button.GetActions().Find(a => a.Direction == direction).Action;
            currentDirectionAction.RemoveAllListeners();
            currentDirectionAction.AddListener(() => HighlightButton(holderToConnectTo));
        }

        public void HighlightButton(InteractableMenuButton holderToHighlight)
        {
            _highlightedButton.Highlight(false);
            _highlightedButton = holderToHighlight.Button;
            _highlightedButton.Highlight(true);
        }

        #region Navigation
        public override void Navigate(Direction direction)
        {
            _highlightedButton.InvokeAction(direction);
        }
        #endregion


        #region Open and Close
        public override void Open(int playerIndex)
        {
            base.Open(playerIndex);

            SetPlayerIndex(playerIndex);

            SetUpInput(playerIndex);

            ConnectButtonsInput();

            HighlightFirstButton();

            DisableMessageText();
        }

        public override void Close(int playerIndex)
        {
            base.Close(playerIndex);

            DisposeInput(playerIndex);

            _buttons.ForEach(b => b.IsActive = false);


            gameObject.SetActive(false);
        }
        #endregion

        #region Others
        public override void DehighlightCells()
        {
            base.DehighlightCells();
        }

        public void HighlightFirstButton()
        {
            _buttons.ForEach(b => b.Button.Highlight(false));

            if (_buttons.Count > 0)
            {
                _highlightedButton = _buttons[0].Button;
                _highlightedButton.Highlight(true);
            }
        }

        public override void SetUp()
        {
            base.SetUp();
        }

        public override void SetUpActionSignifiers(ISignifierController signifierController)
        {
            base.SetUpActionSignifiers(signifierController);
        }
        public override void SetUpSignifiers()
        {
            base.SetUpSignifiers();
        }

        public void DisplayMessage(string message)
        {
            CancelInvoke();
            _messageText.enabled = true;
            _messageText.text = message;
            Invoke(nameof(DisableMessageText), MESSAGE_DISPLAY_DURATION);
        }

        private void DisableMessageText()
        {
            _messageText.enabled = false;
        }
        #endregion
    }
}
