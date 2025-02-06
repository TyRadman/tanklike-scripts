using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TankLike
{
    using UI;
    using Utils;
    using UI.Notifications;
    using Combat;
    using Minimap;
    using Environment.LevelGeneration;
    using UI.Map;
    using Sound;
    using UnitControllers;
    using ScreenFreeze;
    using UI.PauseMenu;
    using UI.DamagePopUp;
    using UI.Inventory;
    using Combat.SkillTree;
    using Cam;
    using Environment.Shops;
    using Attributes;

    public class GameManager : Singleton<GameManager>
    {
        #region Managers
        [SerializeField] private UnityEvent _onGameStarted;
        [field: SerializeField, Header("Managers")] public PoolingManager PoolingManager { get; private set; }
        [field: SerializeField, InChildren] public PlayersManager PlayersManager { get; private set; }
        [field: SerializeField] public ReportManager ReportManager { get; private set; }
        [field: SerializeField] public CollectableManager CollectableManager { get; private set; }
        [field: SerializeField] public CameraManager CameraManager { get; private set; }
        [field: SerializeField] public InputManager InputManager { get; private set; }
        [field: SerializeField] public InteractableAreasManager InteractableAreasManager { get; private set; }
        [field: SerializeField] public DebugManager DebugManager { get; private set; }
        [field: SerializeField] public VisualEffectsManager VisualEffectsManager { get; private set; }
        [field: SerializeField] public EnemiesManager EnemiesManager { get; private set; }
        [field: SerializeField, InChildren, Required] public EnemiesCombatManager EnemiesCombatManager { get; private set; }
        [field: SerializeField] public QuestsManager QuestsManager { get; private set; }
        [field: SerializeField] public RoomsManager RoomsManager { get; private set; }
        [field: SerializeField] public BossKeysManager BossKeysManager { get; private set; }
        [field: SerializeField] public SummonsManager SummonsManager { get; private set; }
        [field: SerializeField] public LevelGenerator LevelGenerator { get; private set; }
        [field: SerializeField] public ObstaclesVanisher ObstaclesVanisher { get; private set; }
        [field: SerializeField] public ShopsManager ShopsManager { get; private set; }
        [field: SerializeField] public BulletsManager BulletsManager { get; private set; }
        [field: SerializeField] public DestructiblesManager DestructiblesManager { get; private set; }
        [field: SerializeField] public GameplayRoomGenerator GameplayRoomGenerator { get; private set; }
        [field: SerializeField] public AudioManager AudioManager { get; private set; }
        [field: SerializeField] public ScreenFreezer ScreenFreezer { get; private set; }
        [field: SerializeField] public DamagePopUpManager DamagePopUpManager { get; private set; }
        [field: SerializeField] public BossesManager BossesManager { private set; get; }
        [field: SerializeField] public TeleportationManager TeleportationManager { private set; get; }
        [field: SerializeField] public SceneLoadingManager SceneLoadingManager { private set; get; }
        [field: SerializeField] public WorkshopController WorkshopController { private set; get; }
        [field: SerializeField] public CoroutinesManager CoroutineManager { private set; get; }
        #endregion

        #region UI Controllers
        [field: SerializeField, Header("UI Controllers")] public Canvas MainCanvas { get; private set; }
        [field: SerializeField] public HUDController HUDController { get; private set; }
        [field: SerializeField] public NotificationsManager NotificationsManager { get; private set; }
        [field: SerializeField] public MinimapManager MinimapManager { get; private set; }
        [field: SerializeField] public EffectsUIController EffectsUIController { get; private set; }
        [field: SerializeField] public FadeUIController FadeUIController { get; private set; }
        [field: SerializeField] public LevelMapDisplayer LevelMap { get; private set; }
        [field: SerializeField] public ResultsUIController ResultsUIController { get; private set; }
        [field: SerializeField] public InventoryController Inventory { get; private set; }
        [field: SerializeField] public PauseMenuManager PauseMenuManager { get; private set; }
        [field: SerializeField] public ToolsNavigator ToolShopUI { get; private set; }
        [field: SerializeField] public SkillTreesManager SkillTreesManager { get; private set; }
        [field: SerializeField] public ConfirmPanel ConfirmPanel { get; private set; }
        #endregion

        #region Databases
        [field: SerializeField, Header("Databases")] public ToolsDatabase ToolsDatabase { get; private set; }
        [field: SerializeField] public AmmunitionDatabase BulletsDatabase { get; private set; }
        [field: SerializeField] public EnemiesDatabase EnemiesDatabase { get; private set; }
        [field: SerializeField] public ConstantsManager Constants { get; private set; }
        [field: SerializeField] public GameEditorData GameData { get; private set; }
        [field: SerializeField] public BossesDatabase BossesDatabase { get; private set; }
        [field: SerializeField] public PlayersDatabase PlayersDatabase { get; private set; }
        [field: SerializeField] public InputIconsDatabase InputIconsDatabase { get; private set; }
        [field: SerializeField] public StatIconReferenceDB StatIconReferenceDB { get; internal set; }
        [field: SerializeField] public PlayerStartingSkillsPackage StartingSkillsDB { get; internal set; }

        #endregion

        protected Transform _spawnablesParent;
        private SceneController _currentSceneController;

        [Header("Debug")]
        [SerializeField] private bool _generateLevel;
        [SerializeField] private bool _enableDebug = true;
        [SerializeField] private bool _showCursor = true;
        [SerializeField] private bool _testingScene;
        [SerializeField] private int _frameRate = 60;

        [HideInInspector] public Dictionary<string, bool> FoldoutStates = new Dictionary<string, bool>();
        // an event that test scripts can subscribe to
        
        private void Awake()
        {
            SetManagersReferences();
            SetUIControllersReferences();
            SetUpGlobalManagers();
            SetUpCursor();

            Debug.unityLogger.logEnabled = _enableDebug;
        }

        private void SetUpGlobalManagers()
        {
            AudioManager.SetUp();
            SceneLoadingManager.SetUp();
            FadeUIController.SetUp();
        }

        private void SetUIControllersReferences()
        {
            PauseMenuManager.SetReferences();
            NotificationsManager.SetReferences();
            ResultsUIController.SetReferences();
            Inventory.SetReferences();
            ToolShopUI.SetReferences();
        }

        private void SetManagersReferences()
        {
            InputManager.SetReferences(InputIconsDatabase);
            VisualEffectsManager.SetReferences(BulletsDatabase);
            EnemiesManager.SetReferences(EnemiesDatabase);
            BossesManager.SetReferences(BossesDatabase);
            PlayersManager.SetReferences(PlayersDatabase);
            CameraManager.SetReferences();
            HUDController.SetReferences();
        }

        private void SetUpCursor()
        {
            if (_showCursor)
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void SetCurrentSceneController(SceneController sceneController)
        {
            _currentSceneController = sceneController;
        }

        public void DisposeCurrentSceneController()
        {
            if (_currentSceneController != null)
            {
                _currentSceneController.Dispose();
            }
        }

        public void SetSpawanblesParent(Transform spawnablesParent)
        {
            _spawnablesParent = spawnablesParent;
        }

        public void SetParentToSpawnables(GameObject obj)
        {
            obj.transform.parent = _spawnablesParent;
        }

        public void SetParentToRoomSpawnables(GameObject obj)
        {
            obj.transform.parent = RoomsManager.CurrentRoom.Spawnables.SpawnablesParent;
        }

        [SerializeField] private TMPro.TextMeshProUGUI _debugText;

        public void OnGameOver()
        {
            ResultsUIController.DisplayGameoverScreen();
            CameraManager.PlayerCameraFollow.StopCameraFollowProcess();
            Inventory.Close();
        }
    }

    public enum Direction
    {
        Up = 0, Left = 1, Down = 2, Right = 3, None = 4
    }
}
