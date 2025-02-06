using System.Collections;
using UnityEngine;

namespace TankLike
{
    using Combat.SkillTree.SkillSelection;
    using TankLike.Combat.SkillTree;

    public class GameplaySceneController : SceneController
    {
        [SerializeField] private GameEditorData _gameEditorData;
        [SerializeField] private bool _displaySkillsSelectionMenu = true;
        [SerializeField] private PlayerSkillsSelectionManager _playerSkillSelectionManager;

        private const string GAMEPLAY_SCENE = "S_Gameplay";

        public override void SetUp()
        {
            //Debug.Log(GAMEPLAY_SCENE);
            StartCoroutine(SetupRoutine(GAMEPLAY_SCENE));
        }

        protected override void SetUpManagers()
        {
            if (_gameEditorData != null)
            {
                _gameEditorData.InitializeValues();
            }

            GameManager gameManager = GameManager.Instance;

            if (gameManager == null)
            {
                Debug.LogError($"No game manager at scene {GAMEPLAY_SCENE}");
                return;
            }

            // Play level background music
            gameManager.AudioManager.SwitchBGMusic(gameManager.LevelGenerator.RoomsBuilder.GetCurrentLevelData().LevelMusic);
            gameManager.AudioManager.FadeInBGMusic();

            // Set current scene controller
            gameManager.SetCurrentSceneController(this);

            // Enable the main menu screen only
            gameManager.ResultsUIController.gameObject.SetActive(true);
            //gameManager.EffectsUIController.gameObject.SetActive(true);
            //gameManager.HUDController.gameObject.SetActive(true);
            gameManager.WorkshopController.WorkshopUI.gameObject.SetActive(true);
            gameManager.ShopsManager.gameObject.SetActive(true);
            gameManager.Inventory.gameObject.SetActive(true);
            gameManager.PauseMenuManager.gameObject.SetActive(true);

            Transform spawnablesParent = new GameObject("Spawnables").transform;
            gameManager.SetSpawanblesParent(spawnablesParent);

            // set up managers that don't need inputs
            gameManager.TeleportationManager.SetUp();
            gameManager.RoomsManager.SetUp();
            gameManager.BossKeysManager.SetUp();
            gameManager.DestructiblesManager.SetUp();
            gameManager.MinimapManager.SetUp();
            gameManager.LevelMap.SetUp();

            gameManager.VisualEffectsManager.SetUp();
            gameManager.GameplayRoomGenerator.SetUp();
            gameManager.LevelGenerator.SetUp();


            if (!_displaySkillsSelectionMenu)
            {
                SetUpInputManagers();
                OnSkillsSelected();
            }
            else
            {
                gameManager.CameraManager.PlayerCameraFollow.PositionTargetAtActiveRoomCenter();
                _playerSkillSelectionManager.Open();
                _playerSkillSelectionManager.OnSkillsSelected += OnSkillsSelected;
            }
        }

        private void SetUpInputManagers()
        {
            GameManager gameManager = GameManager.Instance;

            gameManager.EffectsUIController.gameObject.SetActive(true);

            gameManager.NotificationsManager.SetUp();
            gameManager.InputManager.SetUp();
            gameManager.DamagePopUpManager.SetUp();

            gameManager.PlayersManager.SetUp();
            gameManager.EnemiesManager.SetUp();
            gameManager.BossesManager.SetUp();

            gameManager.HUDController.OffScreenIndicator.SetUp(); // Needs to be called here although it's a subcomponent of the HUD

            gameManager.PlayersManager.PlayerSpawner.SpawnPlayerWithSkills = _displaySkillsSelectionMenu;
            gameManager.PlayersManager.PlayerSpawner.SetUpPlayers();

            gameManager.CameraManager.SetUp();

            gameManager.ReportManager.SetUp();
            gameManager.InteractableAreasManager.SetUp();
            gameManager.CollectableManager.SetUp();
            gameManager.SummonsManager.SetUp();

            gameManager.ShopsManager.SetUp();
            gameManager.QuestsManager.SetUp();
            gameManager.BulletsManager.SetUp();
            gameManager.ScreenFreezer.SetUp();

            gameManager.InputManager.EnablePlayerInput();

            gameManager.HUDController.gameObject.SetActive(true);
            gameManager.HUDController.SetUp();

            gameManager.PauseMenuManager.SetUp();
            gameManager.ResultsUIController.SetUp();
            gameManager.EffectsUIController.SetUp();

            gameManager.ToolShopUI.SetUp();
            gameManager.Inventory.SetUp();

            gameManager.CoroutineManager.SetUp();
            gameManager.PlayersManager.SetGameoverOnDeath(true);

            //gameManager.HUDController.PlayerHUDs.ForEach(h => h.ResetAbilitiesIcons());
        }

        private void OnSkillsSelected()
        {
            SetUpInputManagers();
            GameManager.Instance.WorkshopController.SetUp();
            StartCoroutine(OnSkillsSelectedRoutine());
        }

        private IEnumerator OnSkillsSelectedRoutine()
        {
            yield return new WaitForSeconds(1f);

            GameManager.Instance.PlayersManager.PlayerSpawner.SpawnPlayers();

            yield return new WaitForSeconds(1f);

            GameManager.Instance.EffectsUIController.ShowLevelName(); // TODO: find the right place to call this

            // CRUCIAL: This is the line that is missing in the original code 
            //GameManager.Instance.SkillTreesManager.SetUp();
        }

        public override void Dispose()
        {
            //Debug.Log("DISPOSE GAMEPLAY SCENE");

            GameManager gameManager = GameManager.Instance;

            if(gameManager == null)
            {
                Debug.LogError($"No game manager at scene {GAMEPLAY_SCENE}"); 
            }

            // Stop bakcground music
            gameManager.AudioManager.StopBGMusic();

            // Disable UI screens
            gameManager.ResultsUIController.gameObject.SetActive(false);
            gameManager.EffectsUIController.gameObject.SetActive(false);
            gameManager.HUDController.gameObject.SetActive(false);
            gameManager.WorkshopController.WorkshopUI.gameObject.SetActive(false);
            gameManager.Inventory.gameObject.SetActive(false);
            gameManager.PauseMenuManager.gameObject.SetActive(false);

            gameManager.TeleportationManager.Dispose();
            gameManager.RoomsManager.Dispose();
            gameManager.BossKeysManager.Dispose();
            gameManager.CameraManager.Dispose();

            gameManager.DestructiblesManager.Dispose();
            gameManager.NotificationsManager.Dispose();
            gameManager.InputManager.Dispose();
            gameManager.DamagePopUpManager.Dispose();
            gameManager.VisualEffectsManager.Dispose();

            gameManager.PlayersManager.Dispose();
            gameManager.EnemiesManager.Dispose();
            gameManager.BossesManager.Dispose();

            //gameManager.ObstaclesVanisher.SetUp();
            gameManager.ReportManager.Dispose();
            gameManager.InteractableAreasManager.Dispose();
            gameManager.CollectableManager.Dispose();
            gameManager.SummonsManager.Dispose();

            gameManager.ShopsManager.Dispose();
            gameManager.QuestsManager.Dispose();
            gameManager.BulletsManager.Dispose();
            gameManager.ScreenFreezer.Dispose();
            
            //gameManager.InputManager.DisableInputs();

            gameManager.PauseMenuManager.Dispose();
            gameManager.HUDController.Dispose();
            gameManager.MinimapManager.Dispose();
            gameManager.EffectsUIController.Dispose();
            gameManager.LevelMap.Dispose();
            gameManager.ResultsUIController.Dispose();

            gameManager.ToolShopUI.Dispose();
            gameManager.Inventory.Dispose();

            gameManager.WorkshopController.Dispose();
            gameManager.CoroutineManager.Dispose();

            gameManager.HUDController.OffScreenIndicator.Dispose();

            //_skillSelectionMenu.Dispose();
            _playerSkillSelectionManager.Dispose();

            // TODO: Uncomment when we start using the skill tree
            //GameManager.Instance.SkillTreesManager.Dispose(); 
        }
    }
}
