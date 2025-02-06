using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Utils;

    public class PlayerInventoryController : MonoBehaviour, IInput, IController, IConstraintedComponent
    {
        public bool IsActive { get; private set; }
        public bool IsConstrained { get; set; }

        private PlayerComponents _playerComponents;

        public void SetUp(IController controller)
        {
            PlayerComponents components = controller as PlayerComponents;

            if (components == null)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = components;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;

            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Inventory.name).performed += OpenInventory;

            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Inventory.name).performed += CloseInventory;
            UIMap.FindAction(c.UI.Exit.name).performed += CloseInventory;
            UIMap.FindAction(c.UI.Cancel.name).performed += CloseInventory;
            UIMap.FindAction(c.UI.Tab_Right.name).performed += TabRight;
            UIMap.FindAction(c.UI.Tab_Left.name).performed += TabLeft;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;

            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Inventory.name).performed -= OpenInventory;

            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Inventory.name).performed -= CloseInventory;
            UIMap.FindAction(c.UI.Exit.name).performed -= CloseInventory;
            UIMap.FindAction(c.UI.Cancel.name).performed -= CloseInventory;
            UIMap.FindAction(c.UI.Tab_Right.name).performed -= TabRight;
            UIMap.FindAction(c.UI.Tab_Left.name).performed -= TabLeft;
        }
        #endregion

        private void OpenInventory(InputAction.CallbackContext _)
        {
            if (IsConstrained)
            {
                return;
            }

            if (!GameManager.Instance.Inventory.IsActive)
            {
                return;
            }

            // disable the Pause menu controller so that the inputs don't interfere 
            _playerComponents.UIController.EnablePauseMenuController(false);
            GameManager.Instance.InputManager.DisableInputs();
            GameManager.Instance.InputManager.EnableUIInput(_playerComponents.PlayerIndex);
            GameManager.Instance.Inventory.Open(_playerComponents.PlayerIndex);

            GameManager.Instance.HUDController.HideHUD();
        }

        public void CloseInventory(InputAction.CallbackContext _)
        {
            if (!GameManager.Instance.Inventory.IsOpened)
            {
                return;
            }

            //print("Performed");
            // enable the input for the pause manager
            _playerComponents.UIController.EnablePauseMenuController(true);
            GameManager.Instance.InputManager.EnablePlayerInput();
            GameManager.Instance.Inventory.Close(_playerComponents.PlayerIndex);

            GameManager.Instance.HUDController.DisplayHUD();
        }

        public void TabRight(InputAction.CallbackContext _)
        {
            GameManager.Instance.Inventory.SwitchTabs(Direction.Right);
        }

        public void TabLeft(InputAction.CallbackContext _)
        {
            GameManager.Instance.Inventory.SwitchTabs(Direction.Left);
        }

        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canOpenInventory = (constraints & AbilityConstraint.Inventory) == 0;
            IsConstrained = !canOpenInventory;
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
            SetUpInput(_playerComponents.PlayerIndex);
        }

        public void Dispose()
        {
            DisposeInput(_playerComponents.PlayerIndex);
        }
        #endregion
    }
}
