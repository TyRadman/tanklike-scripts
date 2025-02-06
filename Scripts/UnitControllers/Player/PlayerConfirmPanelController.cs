using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerConfirmPanelController : MonoBehaviour, IInput, IController
    {
        public bool IsActive { get; private set; }

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
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Submit.name).performed += Select;
            UIMap.FindAction(c.UI.Cancel.name).performed += Return;
            UIMap.FindAction(c.UI.Exit.name).performed += Return;

            UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Submit.name).performed -= Select;
            UIMap.FindAction(c.UI.Cancel.name).performed -= Return;
            UIMap.FindAction(c.UI.Exit.name).performed -= Return;

            UIMap.FindAction(c.UI.Navigate_Left.name).performed -= NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed -= NavigateRight;
        }
        #endregion

        #region Button Methods
        public void Select(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Select();
        }

        public void NavigateUp(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Navigate(Direction.Up);
        }

        public void NavigateDown(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Navigate(Direction.Down);
        }

        public void NavigateLeft(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Navigate(Direction.Left);
        }

        public void NavigateRight(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Navigate(Direction.Right);
        }

        public void Return(InputAction.CallbackContext _)
        {
            GameManager.Instance.ConfirmPanel.Return();
        }
        #endregion

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
        }
        #endregion
    }
}
