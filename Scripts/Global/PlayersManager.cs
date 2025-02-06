using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using TankLike.UI.DamagePopUp;
    using UI.HUD;
    using TankLike.UnitControllers;
    using TankLike.Utils;
    using System;

    /// <summary>
    /// This script holds all the information about the players, like their references for the enemies to access, their selected characters, upgrades and anything that other classes might need from the players.
    /// </summary>
    public class PlayersManager : MonoBehaviour, IManager
    {
        // TODO: should be taken out as a class of its own and cleaned
        public class PlayerTransforms
        {
            [field: SerializeField]
            public Transform PlayerTransform
            {
                get; private set;
            }
            [field: SerializeField]
            public Transform ImageTransform
            {
                get; private set;
            }
            public int PlayerIndex;
            [field: SerializeField]
            public PlayerPredictedPosition PredictedPosition
            {
                get; private set;
            }

            public PlayerTransforms(PlayerComponents components)
            {
                PlayerTransform = components.transform;
                PredictedPosition = components.PredictedPosition;
                ImageTransform = components.PredictedPosition.GetImage();
                PlayerIndex = components.PlayerIndex;
            }

            public Vector3 GetImageAtDistance(float distance)
            {
                return PredictedPosition.GetPositionAtDistance(distance);
            }
        }

        [field: SerializeField] public PlayerSpawner PlayerSpawner { get; private set; }
        [field: SerializeField] public PlayerConstraintsManager Constraints { get; private set; }
        [field: SerializeField] public PlayerCoinsManager Coins { get; private set; }
        public Action<PlayerComponents> OnPlayerSpawned { get; set; }
        public static int PlayersCount { get; private set; }
        public static int ActivePlayersCount { get; private set; }

        public bool IsActive { get; private set; }

        //[field: SerializeField] public int CoinsAmount { get; private set; }
        public static int PlayerLayer = 11;
        [field: SerializeField]
        public LayerMask PlayerLayerMask
        {
            get; private set;
        }

        [SerializeField] private List<GameplaySettings> _playersGameplaySettings;
        [SerializeField] private OffScreenIndicatorProfile[] _offScreenIndicatorProfiles;
        
        private Dictionary<PlayerType, Pool<UnitParts>> _playerPartsPools = new Dictionary<PlayerType, Pool<UnitParts>>();
        private List<PlayerComponents> _players = new List<PlayerComponents>();
        private List<MiniPlayerComponents> _miniPlayers = new List<MiniPlayerComponents>();
        private List<PlayerTransforms> _playersTransforms = new List<PlayerTransforms>();
        private GameplaySettings _defaultSettings;
        private PlayersDatabase _playersDatabase;
        private bool _gameEndsOnPlayersDeath;

        public void SetReferences(PlayersDatabase playersDatabase)
        {
            _playersDatabase = playersDatabase;

            PlayerSpawner.SetReferences(playersDatabase);
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            InitPools();

            PlayerSpawner.SetUp();
            Constraints.SetUp();
            Coins.SetUp();

            //PlayerSpawner.OnPlayersSetupStarted += OnPlayerSetupStarted;

            GameManager.Instance.ReportManager.OnEnemyEliminated += AddExperienceToPlayers;
        }

        //private void OnPlayerSetupStarted(PlayerComponents player)
        //{
        //    player.TankBodyParts.SetBodyParts(GetPlayerPartsByType((player.Stats as PlayerData).PlayerType));
        //}

        public void Dispose()
        {
            IsActive = false;

            DisposePools();

            _players.ForEach(p => p.Dispose());

            PlayerSpawner.Dispose();
            Constraints.Dispose();
            Coins.Dispose();

            _players.Clear();
            _playersTransforms.Clear();

            PlayersCount = 0;
            ActivePlayersCount = 0;

            GameManager.Instance.ReportManager.OnEnemyEliminated -= AddExperienceToPlayers;
        }
        #endregion

        public void AddPlayer(PlayerComponents player)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }
                
            player.IsAlive = true;
            _players.Add(player);
            AddPlayerTransform(player);
            OnPlayerSpawned?.Invoke(player);
            player.SetUpSettings();
            PlayersCount = _players.Count;
            SetColors(player);
            SetPlayerTexture(player);
        }

        public void OnTwoPlayersMode()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            AddPlayersAsOffScreenIndicatorTargets();
        }

        private void AddPlayersAsOffScreenIndicatorTargets()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                PlayerComponents player = _players[i];
                player.GetComponent<OffScreenIndicatorTarget>().SetColor((player.Stats as PlayerData).Skins[i].Color);
                player.GetComponent<OffScreenIndicatorTarget>().Enable();
            }
        }

        public void AddPlayerTransform(PlayerComponents player)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            PlayerTransforms playerTransform = new PlayerTransforms(player);
            _playersTransforms.Add(playerTransform);

            ActivePlayersCount = _playersTransforms.Count;
        }

        #region Get Methods
        public List<PlayerTransforms> GetPlayerTransforms()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            return _playersTransforms;
        }

        public Transform GetClosestPlayerTransform(Vector3 startPoint)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            if (ActivePlayersCount == 0)
            {
                return _players[0].transform;
            }

            if (ActivePlayersCount == 1)
            {
                return _playersTransforms[0].PlayerTransform;
            }

            float distanceToPlayerOne = (_playersTransforms[0].PlayerTransform.position - startPoint).sqrMagnitude;
            float distanceToPlayerTwo = (_playersTransforms[1].PlayerTransform.position - startPoint).sqrMagnitude;

            if(distanceToPlayerOne < distanceToPlayerTwo)
            {
                return _playersTransforms[0].PlayerTransform;
            }
            else
            {
                return _playersTransforms[1].PlayerTransform;
            }
        }

        public PlayerTransforms GetClosestPlayer(Vector3 startPoint)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            if (_playersTransforms.Count == 0)
            {
                return null;
            }

            if (ActivePlayersCount < 2)
            {
                return _playersTransforms[0];
            }

            float distanceToPlayerOne = (_playersTransforms[0].ImageTransform.position - startPoint).sqrMagnitude;
            float distanceToPlayerTwo = (_playersTransforms[1].ImageTransform.position - startPoint).sqrMagnitude;

            if (distanceToPlayerOne < distanceToPlayerTwo)
            {
                return _playersTransforms[0];
            }
            else
            {
                return _playersTransforms[1];
            }
        }

        public PlayerTransforms GetFarthestPlayer(Vector3 startPoint)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            if (ActivePlayersCount == 1)
            {
                return _playersTransforms[0];
            }

            float distanceToPlayerOne = (_playersTransforms[0].ImageTransform.position - startPoint).sqrMagnitude;
            float distanceToPlayerTwo = (_playersTransforms[1].ImageTransform.position - startPoint).sqrMagnitude;

            if (distanceToPlayerOne > distanceToPlayerTwo)
            {
                return _playersTransforms[0];
            }
            else
            {
                return _playersTransforms[1];
            }
        }

        public List<PlayerComponents> GetPlayerProfiles()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            return _players;
        }

        public PlayerComponents GetPlayer(int index)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            return _players[index];
        }

        public int GetPlayersCount()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return 0;
            }

            return _players.Count;
        }

        public int GetInactivePlayerIndex()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return 0;
            }

            if (ActivePlayersCount == PlayersCount)
            {
                return -1;
            }

            return _players.Find(p => !p.IsAlive).PlayerIndex;
        }

        public List<Transform> GetPlayersTransforms()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            List<Transform> transforms = new List<Transform>();
            _players.ForEach(p => transforms.Add(p.transform));
            return transforms;
        }

        public List<PlayerComponents> GetPlayers(bool ignoreDeadPlayers = false)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            if (ignoreDeadPlayers)
            {
                return _players.FindAll(p => p.IsAlive);
            }

            return _players;
        }

        /// <summary>
        /// Returns the players and the miniplayers
        /// </summary>
        /// <returns></returns>
        public List<UnitComponents> GetAllActivePlayerControllers()
        {
            List<UnitComponents> players = new List<UnitComponents>();

            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].IsAlive)
                {
                    players.Add(_players[i]);
                }
            }

            for (int i = 0; i < _miniPlayers.Count; i++)
            {
                if (_miniPlayers[i].IsActive)
                {
                    players.Add(_miniPlayers[i]);
                }
            }

            return players;
        } 

        public UnitParts GetPlayerPartsByType(PlayerType type)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            UnitParts parts = _playerPartsPools[type].RequestObject(Vector3.zero, Quaternion.identity);
            return parts;
        }
        #endregion

        public void SetAimSensitivity(float amount, int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _playersGameplaySettings[playerIndex].AimSensitivity = amount;
            _players[playerIndex].CrosshairController.SetAimSensitivity(amount);
        }

        public GameplaySettings GetGameplaySettings(int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            return _playersGameplaySettings[playerIndex];
        }

        public void EnablePauseInputForSecondPlayer(int currentPlayerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            if (PlayersCount == 1)
            {
                return;
            }

            int secondPlayerIndex = (currentPlayerIndex + 1) % 2;
            _players[secondPlayerIndex].UIController.PauseMenuController.SetUpInput(secondPlayerIndex);
        }

        public void DisablePauseInputForOtherPlayer(int currentPlayerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            if (PlayersCount == 1)
            {
                return;
            }

            int secondPlayerIndex = (currentPlayerIndex + 1) % 2;
            _players[secondPlayerIndex].UIController.PauseMenuController.DisposeInput(secondPlayerIndex);
        }

        public void ReportPlayerDeath(TankComponents components)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            PlayerComponents player = (PlayerComponents)components;
            int playerIndex = player.PlayerIndex;
            player.IsAlive = false;
            GameManager.Instance.CameraManager.PlayerCameraFollow.RemoveCameraFollow(playerIndex);
            _playersTransforms.Remove(_playersTransforms.Find(t => t.PlayerIndex == _players[playerIndex].PlayerIndex));
            ActivePlayersCount = _playersTransforms.Count;
            GameManager.Instance.HUDController.OffScreenIndicator.EnableOffScreenIndicatorForPlayer(playerIndex, false);
            bool allPlayersDead = _players.TrueForAll(p => !p.IsAlive);

            if (allPlayersDead && _gameEndsOnPlayersDeath)
            {
                GameManager.Instance.OnGameOver();
            }
            else
            {
                player.MiniPlayerSpawner.SpawnMiniPlayer();
                GameManager.Instance.HUDController.PlayerHUDs[player.PlayerIndex].SwitchToMiniPlayerHUD();
            }
        }

        public void SetGameoverOnDeath(bool value)
        {
            _gameEndsOnPlayersDeath = value;
        }

        private void SetColors(PlayerComponents player)
        {
            int index = ActivePlayersCount - 1;
            Color color = (player.Stats as PlayerData).Skins[index].Color;
            _players[index].CrosshairController.SetColor(color);
        }

        private void SetPlayerTexture(PlayerComponents player)
        {
            int index = ActivePlayersCount - 1;
            Texture2D texture = ((PlayerData)player.Stats).Skins[index].Texture;
            _players[index].Visuals.SetTextureForMainMaterial(texture);
            _players[index].TankBodyParts.SetTextureForMainMaterial(texture);
        }

        public float GetPlayersTotalHealth01()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return 0f;
            }

            float health = 0f;
            _players.ForEach(p => health += p.Health.GetHealthAmount01());
            return health;
        }

        /// <summary>
        /// Returns the the players' position. If it's a single player, then only the position of that player is returned, otherwise, the average of the position of both players is returned.
        /// </summary>
        public Vector3 GetPlayersPosition()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return Vector3.zero;
            }

            Vector3 position = Vector3.zero;
            _players.ForEach(p => position += p.transform.position);
            return position;
        }

        public void ApplyConstraints(bool apply, AbilityConstraint constraints)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _players.ForEach(p => p.Constraints.ApplyConstraints(apply, constraints));
        }

        public void AddExperienceToPlayers(EnemyData data, int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            if(playerIndex >= _players.Count)
            {
                Debug.LogError($"Player index {playerIndex} is higher than the number of players {_players.Count}");
                return;
            }

            if(playerIndex < 0)
            {
                return;
            }

            _players[playerIndex].Experience.AddExperience(data.ExperiencePerKill);
        }

        public void AddExperienceToPlayers(EntityEliminationReport report)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            if(report == null)
            {
                return;
            }

            int playerIndex = report.PlayerIndex;

            if (playerIndex >= _players.Count || playerIndex < 0)
            {
                //Debug.LogError($"Player index {playerIndex} is incorrect.");
                return;
            }

            if(report.Target is not TankComponents tank)
            {
                Debug.LogError($"Target is not a tank or entity.");
                return;
            }

            if (tank is EnemyComponents enemy)
            {
                //Vector3 position = report.Position;
                Vector3 position = _players[report.PlayerIndex].transform.position;

                GameManager.Instance.DamagePopUpManager.DisplayPopUp(DamagePopUpType.XP, (enemy.Stats as EnemyData).ExperiencePerKill, position);
                _players[playerIndex].Experience.AddExperience((enemy.Stats as EnemyData).ExperiencePerKill);
            }
            else if (tank is BossComponents boss)
            {
                //Vector3 position = report.Position;
                Vector3 position = _players[report.PlayerIndex].transform.position;

                GameManager.Instance.DamagePopUpManager.DisplayPopUp(DamagePopUpType.XP, (boss.Stats as EnemyData).ExperiencePerKill, position);
                _players[playerIndex].Experience.AddExperience((boss.Stats as EnemyData).ExperiencePerKill);
            }
        }

        internal bool HasDeadPlayers()
        {
            return _players.Exists(p => !p.IsAlive);
        }

        #region Statics
        private static Dictionary<Collider, PlayerComponents> _playerCollidersReferences = new Dictionary<Collider, PlayerComponents>();

        public static PlayerComponents GetPlayerComponentByCollider(Collider collider)
        {
            if (!_playerCollidersReferences.ContainsKey(collider))
            {
                PlayerComponents playerComponents = collider.GetComponent<PlayerComponents>();

                if(playerComponents != null)
                {
                    _playerCollidersReferences.Add(collider, playerComponents);
                }
                else
                {
                    Debug.LogError($"Collider of {collider.name} doesn't have a PlayerComponents.");
                    return null;
                }
            }

            return _playerCollidersReferences[collider];
        }
        #endregion

        #region MiniPlayers
        public void AddMiniPlayer(MiniPlayerComponents miniPlayer)
        {
            _miniPlayers.Add(miniPlayer);
        }
        #endregion

        #region Pools
        private void InitPools()
        {
            foreach (var player in _playersDatabase.GetAllPlayers())
            {
                if (player.PartsPrefab == null) continue;
                _playerPartsPools.Add(player.PlayerType, CreatePlayerPartsPool(player));
            }
        }

        private void DisposePools()
        {
            foreach (KeyValuePair<PlayerType, Pool<UnitParts>> playerParts in _playerPartsPools)
            {
                playerParts.Value.Clear();
            }

            _playerPartsPools.Clear();
        }

        private Pool<UnitParts> CreatePlayerPartsPool(PlayerData playerData)
        {
            var pool = new Pool<UnitParts>(
                () =>
                {
                    var obj = Instantiate(playerData.PartsPrefab);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj.GetComponent<UnitParts>();
                },
                (UnitParts obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (UnitParts obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (UnitParts obj) => obj.GetComponent<IPoolable>().Clear(),
                0
            );
            return pool;
        }

        internal void OnSinglePlayerMode()
        {
            if (_players[0] != null && _players[0].TryGetComponent(out OffScreenIndicatorTarget target))
            {
                target.Disable();
            }
        }
        #endregion
    }
}
