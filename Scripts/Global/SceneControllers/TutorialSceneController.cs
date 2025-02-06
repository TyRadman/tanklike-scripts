using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using TankLike.Tutorial;
using UnityEngine;

namespace TankLike
{
    using UnitControllers;
    using Sound;

    public class TutorialSceneController : SceneController
    {
        [SerializeField] private TutorialManager _tutorialManager;
        [SerializeField] private NormalRoom _tutorialRoom;
        [SerializeField] private Audio _bgMusicAudio;

        public override void SetUp()
        {
            StartCoroutine(SetupRoutine(Scenes.TUTORIAL));
        }

        protected override void SetUpManagers()
        {
            // Set current scene controller
            GameManager.Instance.SetCurrentSceneController(this);

            GameManager.Instance.TeleportationManager.SetUp();

            // Set tutorial room as the current room
            GameManager.Instance.RoomsManager.SetCurrentRoom(_tutorialRoom);

            // Enable the main menu screen only
            GameManager.Instance.ResultsUIController.gameObject.SetActive(true);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(true);
            GameManager.Instance.HUDController.gameObject.SetActive(true);
            GameManager.Instance.WorkshopController.WorkshopUI.gameObject.SetActive(false);
            GameManager.Instance.PauseMenuManager.gameObject.SetActive(true);

            GameManager.Instance.DestructiblesManager.SetUp();
            //GameManager.Instance.NotificationsManager.SetUp();
            GameManager.Instance.InputManager.SetUp();
            GameManager.Instance.DamagePopUpManager.SetUp();
            GameManager.Instance.VisualEffectsManager.SetUp();

            GameManager.Instance.PlayersManager.SetUp();
            GameManager.Instance.EnemiesManager.EnableSpawnEnemies(false);
            GameManager.Instance.EnemiesManager.SetUp();
            GameManager.Instance.BossesManager.SetUp();

            GameManager.Instance.PlayersManager.PlayerSpawner.SpawnPlayerWithSkills = true;
            GameManager.Instance.PlayersManager.PlayerSpawner.SetUpPlayers();
            // remove the tools component so that it's not used and we don't need to add it to every constaint
            GameManager.Instance.PlayersManager.GetPlayers().ForEach(p => p.RemoveComponent(typeof(PlayerTools)));
            GameManager.Instance.PlayersManager.PlayerSpawner.SpawnPlayers();

            GameManager.Instance.CameraManager.SetUp();
            //GameManager.Instance.ObstaclesVanisher.SetUp();
            GameManager.Instance.ReportManager.SetUp();
            GameManager.Instance.InteractableAreasManager.SetUp();
            GameManager.Instance.CollectableManager.SetUp();

            GameManager.Instance.ShopsManager.SetUp();
            //GameManager.Instance.QuestsManager.SetUp();
            GameManager.Instance.BulletsManager.SetUp();
            GameManager.Instance.ScreenFreezer.SetUp();

            GameManager.Instance.InputManager.EnablePlayerInput();

            GameManager.Instance.PauseMenuManager.SetUp();
            GameManager.Instance.HUDController.SetUp();
            GameManager.Instance.EffectsUIController.SetUp();

            GameManager.Instance.NotificationsManager.SetUp();

            GameManager.Instance.PlayersManager.GetPlayer(0).OnPlayerActivated += _tutorialManager.SetUp;
            
            GameManager.Instance.HUDController.OffScreenIndicator.SetUp();
            GameManager.Instance.CoroutineManager.SetUp();

            // to hide the minimap
            GameManager.Instance.MinimapManager.Hide();
            GameManager.Instance.PlayersManager.SetGameoverOnDeath(false);

            GameManager.Instance.AudioManager.SwitchBGMusic(_bgMusicAudio);
            GameManager.Instance.AudioManager.FadeInBGMusic();

            //GameManager.Instance.PlayersManager.GetPlayers().ForEach(p => p.Energy.EnableFuelUsage(false));
        }

        public override void Dispose()
        {
            //Debug.Log("DISPOSE TUTORIAL SCENE");

            // Disable UI screens
            GameManager.Instance.ResultsUIController.gameObject.SetActive(false);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(false);
            GameManager.Instance.HUDController.gameObject.SetActive(false);
            GameManager.Instance.WorkshopController.WorkshopUI.gameObject.SetActive(false);
            GameManager.Instance.Inventory.gameObject.SetActive(false);
            GameManager.Instance.PauseMenuManager.gameObject.SetActive(false);

            GameManager.Instance.TeleportationManager.Dispose();
            GameManager.Instance.RoomsManager.Dispose();
            GameManager.Instance.CameraManager.Dispose();

            GameManager.Instance.DestructiblesManager.Dispose();
            GameManager.Instance.NotificationsManager.Dispose();
            GameManager.Instance.InputManager.Dispose();
            GameManager.Instance.DamagePopUpManager.Dispose();
            GameManager.Instance.VisualEffectsManager.Dispose();

            GameManager.Instance.PlayersManager.Dispose();
            GameManager.Instance.EnemiesManager.Dispose();
            GameManager.Instance.BossesManager.Dispose();

            //GameManager.Instance.ObstaclesVanisher.SetUp();
            GameManager.Instance.ReportManager.Dispose();
            GameManager.Instance.InteractableAreasManager.Dispose();
            GameManager.Instance.CollectableManager.Dispose();

            GameManager.Instance.ShopsManager.Dispose();
            GameManager.Instance.QuestsManager.Dispose();
            GameManager.Instance.BulletsManager.Dispose();
            GameManager.Instance.ScreenFreezer.Dispose();

            //GameManager.Instance.InputManager.DisableInputs();

            GameManager.Instance.PauseMenuManager.Dispose();
            GameManager.Instance.HUDController.Dispose();
            GameManager.Instance.EffectsUIController.Dispose();

            GameManager.Instance.NotificationsManager.Dispose();

            GameManager.Instance.LevelMap.Dispose();
            GameManager.Instance.ResultsUIController.Dispose();

            GameManager.Instance.HUDController.OffScreenIndicator.Dispose();
            GameManager.Instance.CoroutineManager.Dispose();

            GameManager.Instance.AudioManager.StopBGMusic();
        }
    }
}
