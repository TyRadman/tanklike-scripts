using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.Testing.LevelDesign
{
    using System.Linq;
    using TankLike.Attributes;
    using TankLike.Combat.Destructible;
    using TankLike.Environment;
    using TankLike.Environment.LevelGeneration;
    using TankLike.Environment.MapMaker;
    using TankLike.Utils;
    using UnitControllers;
    using static TankLike.Environment.LevelGeneration.GameplayRoomGenerator;

    public class LevelDesignManager : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Transform _playerSpawnPoint;

        [Header("Map Generator")]
        [field: SerializeField] public MapTiles_SO MapToBuild;
        [field: SerializeField] public LevelData LevelData;
        [field: SerializeField] public NormalRoom RoomReference;
        [field: SerializeField] public BuildConfigs Configs;
        [SerializeField] private RoomGate _gateToBuild;

        [Header("Painters")]
        [SerializeField] private PainterType _typesToPerform;
        [SerializeField, InSelf] private ExplosivesPainter _explosivesPainter;
        [SerializeField, InSelf] private DroppersPainter _droppersPainter;
        [SerializeField, InSelf] private GrassPainter _grassPainter;
        [SerializeField, InSelf] private DestructibleWallsPainter _destructibleWallsPainter;
        [SerializeField, InSelf] private BaseTilesPainter _baseTilesPainter;

        private Dictionary<PainterType, RoomPainter> _roomPainters;

        private PlayerComponents _playerComponents;
        private WaitForSeconds _particleWait;
        private WaitForSeconds _respawnDelay = new WaitForSeconds(1f);

        public void SetUp()
        {
            _particleWait = new WaitForSeconds(GameManager.Instance.VisualEffectsManager.Misc.EnemySpawning.Particles.main.startLifetime.constant / 2);
            GameManager.Instance.CameraManager.Zoom.SetToFightZoom();

            SetUpPlayer();
        }

        #region Player
        private void SetUpPlayer()
        {
            SetUpPlayerSpawn();

            GameManager.Instance.PlayersManager.GetPlayer(0).OnPlayerActivated -= SetUp;
        }

        private void SetUpPlayerSpawn()
        {
            _playerComponents = GameManager.Instance.PlayersManager.GetPlayer(0);

            _playerComponents.Health.OnDeath += RespawnPlayer;
            // remove displaying the gameover screen from the OnDeath subscribers
            _playerComponents.Health.OnDeath -= GameManager.Instance.PlayersManager.ReportPlayerDeath;
        }

        private void RespawnPlayer(TankComponents tank)
        {
            OnPlayerDeath?.Invoke();
            StartCoroutine(RespawnPlayerProcess());
        }

        public System.Action OnPlayerDeath;

        private IEnumerator RespawnPlayerProcess()
        {
            yield return _respawnDelay;
            GameManager.Instance.PlayersManager.PlayerSpawner.RevivePlayer(0, _playerSpawnPoint.position);
            SetUpPlayerSpawn();

        }
        #endregion

        public void BuildRoom(MapTiles_SO map, LevelData level, Room room, BuildConfigs configs = null)
        {
            MapTiles_SO mapClone = Instantiate(map);

            _roomPainters = new Dictionary<PainterType, RoomPainter>()
            {
                { PainterType.BaseTiles, _baseTilesPainter},
                { PainterType.Explosives, _explosivesPainter},
                { PainterType.Droppers, _droppersPainter},
                { PainterType.Grass, _grassPainter},
                { PainterType.DestructibleWalls, _destructibleWallsPainter},
            };

            if (_typesToPerform.HasFlag(PainterType.BaseTiles))
            {
                _baseTilesPainter.SetConfigurations(configs);
            }

            foreach (var kvp in _roomPainters)
            {
                if (_typesToPerform.HasFlag(kvp.Key))
                {
                    kvp.Value.SetUp();
                    kvp.Value.PaintRoom(mapClone, room);
                }
            }

            // set camera limits
            //SetLevelLimits(map, room);

            // sort the gates' list
            room.GatesInfo.SortGates();
        }

        public GateDirection GetGateDirection(int maxX, int maxY, Vector2Int tileDimension)
        {
            if (tileDimension.x == maxX) return GateDirection.East;
            else if (tileDimension.x == 0) return GateDirection.West;
            else if (tileDimension.y == maxY) return GateDirection.North;
            else return GateDirection.South;
        }
    }
}
