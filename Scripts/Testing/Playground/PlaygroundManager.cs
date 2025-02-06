using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.Testing.Playground
{
    using Environment;
    using Sound;
    using UnitControllers;

    public class PlaygroundManager : MonoBehaviour, IInput
    {
        [Header("Player")]
        [SerializeField] private Transform _playerSpawnPoint;

        [Header("Enemies")]
        [SerializeField] private RoomUnitSpawner _roomUnitSpawner;
        [SerializeField] private Audio _spawnAudio;

        [Header("UI")]
        [SerializeField] private PlaygroundUIController _playgroundUIController;
        [SerializeField] private PlaygroundAbilitySelectionUIController _abilitySelectionUIController;
        [SerializeField] private PlaygroundSpecialUpgradesUIController _specialUpgradesUIController;

        [Header("Debug")]
        public Transform TestingSphere;

        private PlayerComponents _playerComponents;
        private WaitForSeconds _particleWait;
        private WaitForSeconds _respawnDelay = new WaitForSeconds(1f);
        private bool _menuIsOpen = false;
        private Vector3 _testingSphereOriginalPosition;

        public void SetUp()
        {
            _particleWait = new WaitForSeconds(GameManager.Instance.VisualEffectsManager.Misc.EnemySpawning.Particles.main.startLifetime.constant / 2);
            GameManager.Instance.CameraManager.Zoom.SetToFightZoom();

            SetUpInput(0);
            SetUpPlayer();
            _playgroundUIController.SetUp(this);
            _abilitySelectionUIController.SetUp(this);
            _specialUpgradesUIController.SetUp(this);

            _testingSphereOriginalPosition = TestingSphere.position;
        }

        #region Debug
        public void ReturnTestingSphere()
        {
            TestingSphere.position = _testingSphereOriginalPosition;
        }

        #endregion
        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;

            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            InputActionMap uiMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            playerMap.FindAction(c.Player.Inventory.name).performed += HandleMenuInput;
            uiMap.FindAction(c.UI.Inventory.name).performed += HandleMenuInput;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;

            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            InputActionMap uiMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            playerMap.FindAction(c.Player.Inventory.name).performed -= HandleMenuInput;
            uiMap.FindAction(c.UI.Inventory.name).performed -= HandleMenuInput;
        }
        #endregion

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

        #region Enemies
        public void SpawnEnemy(EnemyType type)
        {
            Vector3 point = _roomUnitSpawner.SpawnPoints.GetRandomSpawnPoint().position;
            StartCoroutine(SpawnEnemy(type, point));
        }

        private IEnumerator SpawnEnemy(EnemyType enemy, Vector3 spawnPoint)
        {
            // play the effect (Should be called from the pooling system later)
            GameManager.Instance.VisualEffectsManager.Misc.PlayEnemySpawnVFX(spawnPoint);
            GameManager.Instance.AudioManager.Play(_spawnAudio);

            yield return _particleWait;

            EnemyComponents spawnedEnemy = GameManager.Instance.EnemiesManager.RequestEnemy(enemy);

            spawnedEnemy.transform.position = spawnPoint;
            spawnedEnemy.gameObject.SetActive(true);
            spawnedEnemy.Restart();

            if (GameManager.Instance.EnemiesManager.EnemiesAreActivated)
            {
                spawnedEnemy.Activate();
            }
            else
            {
                spawnedEnemy.Deactivate();
            }
        }
        #endregion

        #region UI
        private void HandleMenuInput(InputAction.CallbackContext _)
        {
            if (_menuIsOpen)
            {
                _playgroundUIController.Close();
                GameManager.Instance.InputManager.EnablePlayerInput();
                GameManager.Instance.HUDController.DisplayHUD();
                _abilitySelectionUIController.Close();
            }
            else
            {
                _playgroundUIController.Open();
                GameManager.Instance.InputManager.EnableUIInput();
                GameManager.Instance.HUDController.HideHUD();
                _abilitySelectionUIController.Init();
                _specialUpgradesUIController.Init();
            }

            _menuIsOpen = !_menuIsOpen;
            Cursor.visible = _menuIsOpen;
            Cursor.lockState = _menuIsOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }

        #endregion
    }
}
