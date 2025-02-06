using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Environment;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class BossesTestSceneController : SceneController
    {
        [SerializeField] private BossRoom _bossRoom;

        private const string BOSSES_SCENE = "S_Bosses";

        public override void SetUp()
        {
            Debug.Log("SETUP BOSSES SCENE");
            StartCoroutine(SetupRoutine(BOSSES_SCENE));
        }

        protected override void SetUpManagers()
        {
            // Set current scene controller
            GameManager.Instance.SetCurrentSceneController(this);

            GameManager.Instance.TeleportationManager.SetUp();

            // Set bossRoom as the current room
            GameManager.Instance.RoomsManager.SetCurrentRoom(_bossRoom);

            // Enable gameplay UI screens
            GameManager.Instance.ResultsUIController.gameObject.SetActive(true);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(true);
            GameManager.Instance.HUDController.gameObject.SetActive(true);
            GameManager.Instance.WorkshopController.WorkshopUI.gameObject.SetActive(false);

            GameManager.Instance.RoomsManager.SetUp();

            GameManager.Instance.NotificationsManager.SetUp();
            GameManager.Instance.InputManager.SetUp();
            GameManager.Instance.DamagePopUpManager.SetUp();
            GameManager.Instance.VisualEffectsManager.SetUp();
            GameManager.Instance.MinimapManager.Dispose();

            GameManager.Instance.PlayersManager.SetUp();
            GameManager.Instance.BossesManager.SetUp();

            GameManager.Instance.PlayersManager.PlayerSpawner.SetUpPlayers();
            GameManager.Instance.PlayersManager.PlayerSpawner.SpawnPlayers();

            GameManager.Instance.CameraManager.SetUp();
            GameManager.Instance.RoomsManager.LoadBossRoom();
            GameManager.Instance.InteractableAreasManager.SetUp();
            GameManager.Instance.CollectableManager.SetUp();

            GameManager.Instance.BossKeysManager.SetUp();

            GameManager.Instance.ShopsManager.SetUp();
            GameManager.Instance.QuestsManager.SetUp();
            GameManager.Instance.BulletsManager.SetUp();
            GameManager.Instance.ScreenFreezer.SetUp();

            GameManager.Instance.InputManager.EnablePlayerInput();

            GameManager.Instance.PauseMenuManager.SetUp();
            GameManager.Instance.HUDController.SetUp();
            GameManager.Instance.EffectsUIController.SetUp();

            //Reset camera limit
            GameManager.Instance.CameraManager.ResetCameraLimit();

            // Re-set current room to move the room cover
            GameManager.Instance.RoomsManager.SetCurrentRoom(GameManager.Instance.RoomsManager.CurrentRoom);
        }

        public override void Dispose()
        {
            Debug.Log("DISPOSE BOSSES SCENE");
        }
    }
}
