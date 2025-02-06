using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    using System.Linq;

namespace TankLike.Environment
{
    using Sound;
    using UnitControllers;
    using Utils;
    using static TankLike.Environment.RoomSpawnPoints;

    public class RoomUnitSpawner : MonoBehaviour
    {
        [field: SerializeField] public RoomSpawnPoints SpawnPoints { get; private set; }
        public List<EnemyComponents> CurrentlySpawnedEnemies { get; private set; }
        public bool HasKey { get; internal set; } = false;

        [SerializeField] private Vector2 _spawningDelaysRange;
        [SerializeField] private List<EnemyWave> _enemyWaves = new List<EnemyWave>();
        [SerializeField] private Audio _spawnAudio;

        private Room _room;
        private Transform[] _playerSpawnPoints = new Transform[2];
        private List<EnemyComponents> _enemiesSpawned = new List<EnemyComponents>();
        private WaitForSeconds _particleWait;
        private int _currentWaveIndex = 0;

        public void SetUp(Room room)
        {
            if(room == null)
            {
                Debug.LogError("Room is null");
                return;
            }

            _room = room;

            if (_room is not BossRoom)
            {
                SetPlayerSpawnPoints();
            }
        }

        public void SetParticleWait(float time)
        {
            _particleWait = new WaitForSeconds(time / 2);
        }

        public void CheckAndAddKeyHolder()
        {
            Debug.Log($"Room {_room.name} key status: {HasKey}");

            if (HasKey)
            {
                // cache all the waves that don't have an enemy holding a key and have at least one enemy that can hold keys
                // TODO: should there be waves that can't have keys?
                //List<EnemyWave> noKeyWaves = _enemyWaves.FindAll(w => w.Enemies.Exists(e => e.CanHaveKey));
                //GameManager.Instance.BossKeysManager.DistributeKeysAcrossRemainingRooms()

                if(_enemyWaves.Count == 0)
                {
                    Debug.LogError("No waves to check for key holders");
                    return;
                }

                EnemySpawnProfile enemy = _enemyWaves.RandomItem().Enemies.FindAll(e => !e.HasKey).RandomItem();

                // if all the waves are have an enemy holding a key in them, then selected an enemy that doesn't hold a key at least
                enemy.HasKey = true;
            }
        }

        public void ActivateSpawnedEnemies()
        {
            if (_enemyWaves.Count <= 0)
            {
                return;
            }

            SpawnWave();
        }

        private void SpawnWave()
        {
            // reset the list to start counting down the enemies again to know when to trigger the next wave or stop the waves if this is the last wave
            CurrentlySpawnedEnemies = new List<EnemyComponents>();
            EnemyWave wave = _enemyWaves[_currentWaveIndex];

            Vector3 playerPosition = GameManager.Instance.PlayersManager.GetPlayer(0).transform.position;
            int spawnPointsCount = Mathf.CeilToInt(Mathf.Max((float)SpawnPoints.Points.Count * 0.2f, (float)wave.Enemies.Count * 1.5f));
            List<Transform> spawnPoints = SpawnPoints.GetFurthestSpawnPointsFromPosition(playerPosition, spawnPointsCount);

            _enemiesSpawned.Clear();

            foreach (EnemySpawnProfile enemy in wave.Enemies)
            {
                //Vector3 spawnPosition = SpawnPoints.GetFurthestSpawnPointFromPosition(GameManager.Instance.PlayersManager.GetPlayer(0).transform.position).position;
                //Transform spawnPoint = spawnPoints.RandomItem();
                //spawnPoints.Remove(spawnPoint);

                if(enemy == null)
                {
                    Debug.LogError("Enemy is null");
                    continue;
                }

                //if(spawnPoint == null)
                //{
                //    Debug.LogError("Spawn point is null");
                //    continue;
                //}


                EnemyComponents spawnedEnemy = GameManager.Instance.EnemiesManager.RequestEnemy(enemy.Enemy);

                _enemiesSpawned.Add(spawnedEnemy);

                // Vector3 position = spawnPoint.position;
                Vector3 position = enemy.SpawnPointPosition;
                StartCoroutine(SpawnEnemy(spawnedEnemy, position, enemy.HasKey));
            }

            GameManager.Instance.EnemiesCombatManager.AddEnemyWave(_enemiesSpawned);
        }

        private IEnumerator SpawnEnemy(EnemyComponents spawnedEnemy, Vector3 spawnPoint, bool hasKey)
        {
            // perform slight delay before the enemy spawns (for variety)
            yield return new WaitForSeconds(_spawningDelaysRange.RandomValue());

            // play the effect (Should be called from the pooling system later)
            GameManager.Instance.VisualEffectsManager.Misc.PlayEnemySpawnVFX(spawnPoint);
            GameManager.Instance.AudioManager.Play(_spawnAudio);

            yield return _particleWait;

            spawnedEnemy.transform.position = spawnPoint;

            spawnedEnemy.gameObject.SetActive(true);

            spawnedEnemy.Restart();

            spawnedEnemy.OnEnemyDeath += OnEnemyDeathHandler;

            // add the enemy to the currently spawned enemies list
            CurrentlySpawnedEnemies.Add(spawnedEnemy);

            spawnedEnemy.Activate();

            // assign this enemy as a key holder if it was chosen to be one
            if (hasKey)
            {
                Debug.Log($"Enemy {spawnedEnemy.name} is a key holder");
                spawnedEnemy.ItemDrop.SetAsKeyHolder();
            }
        }

        public void OnEnemyDeathHandler(EnemyComponents enemy)
        {
            // remove the enemy if it's a part of the currently spawned enemies
            if (CurrentlySpawnedEnemies.Contains(enemy))
            {
                CurrentlySpawnedEnemies.Remove(enemy);
                enemy.OnEnemyDeath -= OnEnemyDeathHandler;
            }

            // if there are no enemies left, check for the next wave or open the room
            if (CurrentlySpawnedEnemies.Count <= 0)
            {
                OnRoomCleared();
            }
        }

        #region On Room Cleared
        public void OnRoomCleared()
        {
            // check if there are any waves left
            if (_currentWaveIndex < _enemyWaves.Count - 1)
            {
                OnSpawnNextWave();
            }
            else
            {
                OnLastWaveCleared();
            }
        }

        private void OnSpawnNextWave()
        {
            _currentWaveIndex++;
            SpawnWave();
            // set all the points to not taken
            SpawnPoints.SetAllPointsAsNotTaken();
        }

        private void OnLastWaveCleared()
        {
            _room.GatesInfo.Gates.ForEach(g => g.Gate.OpenGate());
            _room.PlayOpenGateAudio();
            GameManager.Instance.CameraManager.Zoom.SetToNormalZoom();
            GameManager.Instance.EnemiesManager.SetFightActivated(false);

            GameManager.Instance.WorkshopController.SpawnWorkshopInRoom();
        }
        #endregion

        public void SetRoomEnemyWaves(List<EnemyWave> waves)
        {
            _enemyWaves = waves;
        }

        public bool HasEnemies()
        {
            return _enemyWaves.Count > 0;
        }

        private void SetPlayerSpawnPoints()
        {
            if(_room == null)
            {
                Debug.LogError("Current room is null");
                return;
            }

            List<SpawnPoint> spawnPoints = SpawnPoints.Points.ToList();
            Vector3 lastPosition = _room.transform.position;

            if(spawnPoints.Count == 0)
            {
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                SpawnPoint selectedSpawnPoint = spawnPoints.FindAll(s => !s.Taken).OrderBy(s =>
                (s.Point.position - lastPosition).sqrMagnitude).FirstOrDefault();
                selectedSpawnPoint.Taken = true;

                _playerSpawnPoints[i] = selectedSpawnPoint.Point;
            }
        }

        public Vector3 GetPlayerSpawnPoint(int playerIndex)
        {
            return _playerSpawnPoints[playerIndex].position;
        }
    }
}
