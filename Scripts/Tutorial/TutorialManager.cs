using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Tutorial
{
    using Combat.Destructible;
    using UnitControllers;
    using Environment.LevelGeneration;
    using ItemsSystem;
    using Environment;

    public class TutorialManager : MonoBehaviour
    {
        public System.Action OnEnemyDeathEvent { get; set; }

        [SerializeField] private AbilityConstraint _startConstraints;
        [SerializeField] private TutorialInstructions _instructionsPanel;
        [SerializeField] private TutorialInstructionAsset _startingInstruction;
        [SerializeField] private Room _tutorialRoom;

        [Header("Crate")]
        [SerializeField] private Crate _crate;
        [SerializeField] private DestructibleDrop _crateDrops;
        [SerializeField] private CollectablesDropSettings _dropSettings;

        private PlayerComponents _playerComponents;
        private List<EnemyType> _enemiesToSpawn = new List<EnemyType>();
        private WaitForSeconds _particleWait;
        private WaitForSeconds _respawnDelay = new WaitForSeconds(1f);
        private Vector3 _respawnPoint;

        private const float START_HEALTH_PERCENTAGE = 0.7f;
        private const float ENERGY_HEAL_MULTIPLIER = 5f;


        public void SetUp()
        {
            _particleWait = new WaitForSeconds(GameManager.Instance.VisualEffectsManager.Misc.EnemySpawning.Particles.main.startLifetime.constant / 2);
            GameManager.Instance.CameraManager.Zoom.SetToFightZoom();

            SetUpPlayer();

            DisplayInstructions(_startingInstruction);
        }

        private void SetUpPlayer()
        {
            SetUpPlayerSpawn();

            _playerComponents.Constraints.ApplyConstraints(true, _startConstraints);

            _playerComponents.Health.SetHealthPercentage(START_HEALTH_PERCENTAGE);

            _playerComponents.Energy.SetHealMultiplier(ENERGY_HEAL_MULTIPLIER);

            GameManager.Instance.PlayersManager.GetPlayer(0).OnPlayerActivated -= SetUp;
        }

        public void ClearStartConstraints()
        {
            _playerComponents.Constraints.ApplyConstraints(false, _startConstraints);
        }

        #region Enemies
        public void SpawnEnemies(WaypointMarker marker)
        {
            // spawn enemies
            for (int i = 0; i < marker.EnemiesToSpawn.Count; i++)
            {
                SpawnEnemy(marker.EnemiesToSpawn[i]);
            }
        }

        public void SpawnEnemy(EnemySpawn spawn)
        {
            if(_enemiesToSpawn == null)
            {
                return;
            }

            StartCoroutine(SummoningProcess(spawn));
        }

        private IEnumerator SummoningProcess(EnemySpawn spawn)
        {
            GameManager.Instance.VisualEffectsManager.Misc.PlayEnemySpawnVFX(spawn.SpawnPoint.position);

            yield return _particleWait;

            EnemyComponents spawnedEnemy = GameManager.Instance.EnemiesManager.RequestEnemy(spawn.Enemy);

            spawnedEnemy.transform.position = spawn.SpawnPoint.position;
            spawnedEnemy.gameObject.SetActive(true);

            spawnedEnemy.Restart();
            spawnedEnemy.Health.OnDeath += OnEnemyDeath;
            spawnedEnemy.Activate();
            spawnedEnemy.ItemDrop.DisableDrops();

            spawn.Modifiers.ForEach(m => spawnedEnemy.DifficultyModifier.ApplyModifier(m));
        }

        private void OnEnemyDeath(TankComponents controller)
        {
            OnEnemyDeathEvent?.Invoke();
            controller.Health.OnDeath -= OnEnemyDeath;
        }

        public void DestroyEnemies()
        {
            GameManager.Instance.EnemiesManager.DestroyAllEnemies();
        }
        #endregion

        #region Event References
        public void EnableInfiniteSuperAbility(bool enable)
        {
            if (enable)
            {
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperRecharger.FullyChargeSuperAbility());
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperAbilities.EnableChargeConsumption(false));
            }
            else
            {
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperAbilities.EnableChargeConsumption(true));
            }
        }

        public void ReturnToMainMenu()
        {
            Invoke(nameof(ReturnToMainMenuDelayed), 2f);
        }
        #endregion

        private void ReturnToMainMenuDelayed()
        {
            GameManager.Instance.PlayersManager.GetPlayers().ForEach(p => p.Dispose());
            GameManager.Instance.DisposeCurrentSceneController();
            GameManager.Instance.SceneLoadingManager.SwitchScene(Scenes.TUTORIAL, Scenes.MAIN_MENU);
        }

        #region Respawn Player
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
        private AbilityConstraint _currentConstraints;

        private IEnumerator RespawnPlayerProcess()
        {
            yield return _respawnDelay;
            GameManager.Instance.PlayersManager.PlayerSpawner.RevivePlayer(0, _respawnPoint);
            SetUpPlayerSpawn();

            float spawnEffectDuration = GameManager.Instance.VisualEffectsManager.Misc.GetPlayerSpawnVFXDuration();
            Invoke(nameof(ApplyConstraintsToPlayer), spawnEffectDuration + 0.1f);
        }

        private void ApplyConstraintsToPlayer()
        {
            // apply constraints that were taking place when the player died
            GameManager.Instance.PlayersManager.Constraints.ApplyConstraints(_currentConstraints);
            _playerComponents.Health.SetHealthPercentage(START_HEALTH_PERCENTAGE);
        }

        public void SetCurrentConstraints(AbilityConstraint constaints)
        {
            _currentConstraints = constaints;
        }

        public void SetRespawnPoint(Transform respawnPoint)
        {
            _respawnPoint = respawnPoint.position;
        }
        #endregion

        #region Crate Creation
        public void SpawnCrate(Transform spawnPoint)
        {
            StartCoroutine(SpawnCrateProcess(spawnPoint));
        }

        private IEnumerator SpawnCrateProcess(Transform parent)
        {
            float spawnEffectDuration = GameManager.Instance.VisualEffectsManager.Misc.PlayPlayerSpawnVFX(parent.position);

            yield return new WaitForSeconds(spawnEffectDuration);

            Crate crateCreated = Instantiate(_crate, parent);

            crateCreated.SetDropSettings(_dropSettings);
            crateCreated.SetCollectablesToSpawn(_crateDrops);

            _tutorialRoom.Spawnables.AddDropper(crateCreated.transform);
        }
        #endregion

        public PlayerComponents GetPlayer()
        {
            return _playerComponents;
        }

        #region Instructions
        private bool _isKeyboard = true;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                _isKeyboard = !_isKeyboard;
            }
        }

        public void DisplayInstructions(TutorialInstructionAsset instruction)
        {
            _instructionsPanel.Display();
            _instructionsPanel.SetText(instruction.GetMessage(_isKeyboard? 0 : 1));
        }

        public void HideInstructions()
        {
            _instructionsPanel.Hide();
        }
        #endregion
    }
}
