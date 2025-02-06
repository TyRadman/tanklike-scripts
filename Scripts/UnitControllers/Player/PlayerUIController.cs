using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class PlayerUIController : MonoBehaviour, IController
    {
        [field: SerializeField] public PlayerInventoryController InventoryController { set; get; }
        [field: SerializeField] public PlayerPauseMenuController PauseMenuController { set; get; }
        [field: SerializeField] public PlayerConfirmPanelController PlayerConfirmPanelController { set; get; }

        private PlayerComponents _playerComponents;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;

            InventoryController.SetUp(_playerComponents);
            PauseMenuController.SetUp(_playerComponents);
            PlayerConfirmPanelController.SetUp(_playerComponents);

            int playerIndex = _playerComponents.PlayerIndex;
            GameManager.Instance.HUDController.PlayerHUDs[playerIndex].SetPlayerAvatar(((PlayerData)_playerComponents.Stats).Skins[playerIndex].Avatar);
        }

        public void EnableInventoryController(bool enable)
        {
            if (enable)
            {
                InventoryController.SetUpInput(_playerComponents.PlayerIndex);
            }
            else
            {
                InventoryController.DisposeInput(_playerComponents.PlayerIndex);
            }
        }

        public void EnablePauseMenuController(bool enable)
        {
            if (enable)
            {
                PauseMenuController.SetUpInput(_playerComponents.PlayerIndex);
            }
            else
            {
                PauseMenuController.DisposeInput(_playerComponents.PlayerIndex);
            }
        }

        public void EnableConfirmPanelController(bool enable)
        {
            if (enable)
            {
                PlayerConfirmPanelController.SetUpInput(_playerComponents.PlayerIndex);
            }
            else
            {
                PlayerConfirmPanelController.DisposeInput(_playerComponents.PlayerIndex);
            }
        }

        #region IController
        public bool IsActive { get; private set; }

        public void Activate()
        {
            InventoryController.Activate();
            PauseMenuController.Activate();
            PlayerConfirmPanelController.Activate();
        }

        public void Deactivate()
        {
            InventoryController.Deactivate();
            PauseMenuController.Deactivate();
            PlayerConfirmPanelController.Deactivate();
        }

        public void Restart()
        {
            InventoryController.Restart();
            PauseMenuController.Restart();
            PlayerConfirmPanelController.Restart();
        }

        public void Dispose()
        {
            InventoryController.Dispose();
            PauseMenuController.Dispose();
            PlayerConfirmPanelController.Dispose();
        }
        #endregion
    }
}
