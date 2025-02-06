using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike
{
    using UnitControllers;
    using Utils;
    using Sound;
    using static Environment.RoomSpawnPoints;
    using Cinemachine.Utility;

    public class PlayerSpawner : MonoBehaviour, IManager
    {
        [SerializeField] private Audio _spawnAudio;

        // for playtest
        public bool SpawnPlayerWithSkills { get; set; } = true;
        public System.Action<PlayerComponents> OnPlayersSetupStarted { get; set; }

        private WaitForSeconds _initialWait = new WaitForSeconds(SPAWN_INITIAL_DELAY);
        private PlayersDatabase _playersDatabase;

        private const float SPAWN_INITIAL_DELAY = 0.5f;

        public bool IsActive { get; private set; }

        public void SetReferences(PlayersDatabase playersDatabase)
        {
            _playersDatabase = playersDatabase;
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }
        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        /// <summary>
        /// Instantiates the player/s, resets their values, and sets them up
        /// </summary>
        public void SetUpPlayers()
        {
            Helper.CheckForManagerActivity(IsActive, GetType());

            int playersCount = GameManager.Instance.GameData.PlayersCount;
            //List<SpawnPoint> spawnPoints = GameManager.Instance.RoomsManager.CurrentRoom.Spawner.SpawnPoints.Points;
            //_lastPosition = GameManager.Instance.RoomsManager.CurrentRoom.transform.position;

            for (int i = 0; i < playersCount; i++)
            {
                //SpawnPoint selectedSpawnPoint = spawnPoints.FindAll(s => !s.Taken).OrderBy(s =>
                //(s.Point.position - _lastPosition).sqrMagnitude).FirstOrDefault();
                //selectedSpawnPoint.Taken = true;

                //_lastPosition = selectedSpawnPoint.Point.position;
                Vector3 spawnPoint = GameManager.Instance.RoomsManager.CurrentRoom.Spawner.GetPlayerSpawnPoint(i);

                PlayerData selectedCharacter = _playersDatabase.GetPlayerDataByType(PlayerType.PlayerOne); // TODO: Get selected character

                PlayerComponents player = Instantiate(selectedCharacter.Prefab, spawnPoint, Quaternion.identity);

                OnPlayersSetupStarted?.Invoke(player);

                player.gameObject.name = $"Player {i + 1}";
                player.SetIndex(i);
                player.StartWithDefaultSkills = SpawnPlayerWithSkills;
                player.SetUp();
                player.Restart();
                player.gameObject.SetActive(false);

                GameManager.Instance.PlayersManager.AddPlayer(player);
            }

            if (playersCount == 2)
            {
                GameManager.Instance.PlayersManager.OnTwoPlayersMode();
            }
            else
            {
                GameManager.Instance.PlayersManager.OnSinglePlayerMode();
            }
        }

        /// <summary>
        /// Positions the players in the level and plays the spawn effect.
        /// </summary>
        public void SpawnPlayers()
        {
            Helper.CheckForManagerActivity(IsActive, GetType());

            int playersCount = GameManager.Instance.GameData.PlayersCount;

            for (int i = 0; i < playersCount; i++)
            {
                SpawnPlayer(i);
            }
        }

        private void SpawnPlayer(int playerIndex)
        {
            StartCoroutine(SpawnProcess(playerIndex));
        }

        private IEnumerator SpawnProcess(int playerIndex)
        {
            PlayerComponents player = GameManager.Instance.PlayersManager.GetPlayer(playerIndex);
            Vector3 position = player.transform.position;
            position.y += 2f;
            player.transform.position = position;

            yield return _initialWait;

            float effectDuration = PlaySpawnEffects(position);

            yield return new WaitForSeconds(effectDuration);

            player.gameObject.SetActive(true);
            player.Activate();
            player.SpawnMinimapIcon();

            //GameManager.Instance.EffectsUIController.ShowLevelName(); // TODO: find the right place to call this
        }

        public void RevivePlayer(int playerIndex, Vector3 respawnPosition, int healthAmount = -1)
        {
            Helper.CheckForManagerActivity(IsActive, GetType());

            StartCoroutine(RevivalProcess(playerIndex, respawnPosition, healthAmount));
        }

        private IEnumerator RevivalProcess(int playerIndex, Vector3 respawnPosition, int healthAmount)
        {
            respawnPosition.y += 2f;

            float effectDuration = PlaySpawnEffects(respawnPosition);

            yield return new WaitForSeconds(effectDuration);

            PlayerComponents player = GameManager.Instance.PlayersManager.GetPlayer(playerIndex);

            player.transform.position = respawnPosition;
            player.gameObject.SetActive(true);
            player.OnRevived(healthAmount);

            GameManager.Instance.PlayersManager.AddPlayerTransform(player);

            // make the camera follow the newly added player too
            GameManager.Instance.CameraManager.PlayerCameraFollow.AddCameraFollower(playerIndex);
            GameManager.Instance.HUDController.OffScreenIndicator.EnableOffScreenIndicatorForPlayer(playerIndex, true);
        }

        private float PlaySpawnEffects(Vector3 position)
        {
            GameManager.Instance.AudioManager.Play(_spawnAudio);
            float effectDuration = GameManager.Instance.VisualEffectsManager.Misc.PlayPlayerSpawnVFX(position);
            return effectDuration;
        }
    }
}
