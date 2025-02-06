using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Environment;
    using Sound;
    using Utils;

    public class PlayerInteractions : MonoBehaviour, IController, IInput
    {
        [SerializeField] private Audio _onSwitchAudio;

        private PlayerComponents _playerComponents;
        private InteractableArea _interactable;

        public bool IsActive { get; private set; }

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap interactionMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            interactionMap.FindAction(c.Player.Jump.name).performed += Interact;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap interactionMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            interactionMap.FindAction(c.Player.Jump.name).performed -= Interact;
        }
        #endregion

        public void OnInteractionAreaEnter(InteractableArea interactable, Transform buttonDisplayPosition, string interactionText, AbilityConstraint constraints)
        {
            _interactable = interactable;

            // enable the interaction input
            SetUpInput(_playerComponents.PlayerIndex);

            // disable shooting input by apply constraints
            _playerComponents.Constraints.ApplyConstraints(true, constraints);

            //GameManager.Instance.InputManager.EnableInteractionInput(true);
            GameManager.Instance.InteractableAreasManager.ActivateTextBox(buttonDisplayPosition, interactionText);
        }

        public void OnInteractionAreaExit(AbilityConstraint constraints)
        {
            _interactable = null;

            // disable input
            DisposeInput(_playerComponents.PlayerIndex);

            // enable shooting and other constraints that have been applied
            _playerComponents.Constraints.ApplyConstraints(false, constraints);

            //GameManager.Instance.InputManager.EnablePlayerInput();
            GameManager.Instance.InteractableAreasManager.DeactivateTextBox(_playerComponents.PlayerIndex);
        }

        public void Interact(InputAction.CallbackContext _)
        {
            _interactable.Interact(_playerComponents.PlayerIndex);
            GameManager.Instance.AudioManager.Play(_onSwitchAudio);
        }

        #region IController

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
        }

        public void Dispose()
        {
            DisposeInput(_playerComponents.PlayerIndex);
            
            // manually exit the interactable area if the player is inside one
            if (_interactable != null)
            {
                _interactable.StopInteraction();
            }
        }
        #endregion
    }
}
