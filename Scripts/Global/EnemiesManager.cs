using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TankLike
{
    using UnitControllers;
    using Utils;
    using Environment;
    using TankLike.Environment.LevelGeneration;
    using Attributes;

    public class EnemiesManager : MonoBehaviour, IManager
    {
        [field: SerializeField, Range(0f, 1f)] public float Difficulty { get; private set; }
        [field: SerializeField, InSelf(true)] public EnemiesPainter Painter { get; private set; }
        public bool IsActive { get; private set; }
        public bool EnemiesAreActivated { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool _spawnEnemies = true;
        [SerializeField] private List<TankComponents> _spawnedEnemies = new List<TankComponents>();
        [SerializeField] private List<WaveData> _tutorialWaves;
        [SerializeField] private AnimationCurve _progressionCurve;
        [SerializeField] private int _currentMinProgression = 0;
        [SerializeField] private int _currentWaveIndex = 0;

        private Dictionary<EnemyType, Pool<UnitParts>> _enemyPartsPools = new Dictionary<EnemyType, Pool<UnitParts>>();
        private Dictionary<EnemyType, Pool<EnemyComponents>> _enemiesPools = new Dictionary<EnemyType, Pool<EnemyComponents>>();
        private List<WaveData> _wavesToSpawn = new List<WaveData>();
        private List<WaveData> _waves;
        private EnemiesDatabase _enemiesDatabase;
        private Vector2Int _wavesCapacityRange;
        private bool _fightActivated;
        private int _roomsWithNoSpawns = 0;

        private const int MAX_WAVES_PER_ROOM_COUNT = 3;
        private const float AVERAGE_WAVES_PER_ROOM = 2f;

        public void SetReferences(EnemiesDatabase enemiesDatabase)
        {
            _enemiesDatabase = enemiesDatabase;
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
            _currentWaveIndex = 0;
            _currentMinProgression = 0;

            _waves = new List<WaveData>(GameManager.Instance.LevelGenerator.RoomsBuilder.GetCurrentLevelData().Waves);

            List<WaveData> wavesToGetCapacityFrom = new List<WaveData>();
            _waves.ForEach(w => wavesToGetCapacityFrom.Add(w));
            _tutorialWaves.ForEach(w => wavesToGetCapacityFrom.Add(w));

            int minCapacity = wavesToGetCapacityFrom.OrderBy(w => w.Capacity).First().Capacity;
            int maxCapacity = wavesToGetCapacityFrom.OrderByDescending(w => w.Capacity).First().Capacity;
            _wavesCapacityRange = new Vector2Int(minCapacity, maxCapacity);

            InitPools();

            _spawnedEnemies.Clear();

            if (!_spawnEnemies)
            {
                return;
            }

            CreateWaves();

            // Enemies should be activated by default when they spawn
            EnemiesAreActivated = true;
        }

        public void Dispose()
        {
            IsActive = false;

            DisposePools();

            if(_waves != null)
            {
                _waves.Clear();
            }

            _wavesToSpawn.Clear();
        }
        #endregion

        public void CreateWaves()
        {
            List<Room> rooms = GameManager.Instance.RoomsManager.Rooms;
            _roomsWithNoSpawns = rooms.Count(r => r.RoomType == RoomType.Normal || r.RoomType == RoomType.BossGate);
            int numberOfWaves = Mathf.CeilToInt(_roomsWithNoSpawns * AVERAGE_WAVES_PER_ROOM);

            if (numberOfWaves < 3)
            {
                Debug.LogError("Number of rooms that can hold keys is less than the number of keys");
                return;
            }

            for (int i = 0; i < _tutorialWaves.Count; i++)
            {
                _wavesToSpawn.Add(Instantiate(_tutorialWaves[i]));
            }

            for (int i = _tutorialWaves.Count; i < numberOfWaves; i++)
            {
                float wavesProgression = _progressionCurve.Evaluate((float)i / (float)numberOfWaves);
                _currentMinProgression = (int)Mathf.Lerp(_wavesCapacityRange.x, _wavesCapacityRange.y, wavesProgression);

                WaveData waveToCreate = null;
                int startMargin = 0;

                while (waveToCreate == null)
                {
                    startMargin += 2;
                    waveToCreate = GetWaveWithinCapacityCount(_currentMinProgression, startMargin);
                }

                _wavesToSpawn.Add(Instantiate(waveToCreate));
            }
            
            // ones the waves (as data, not as spawned enemies) is created, add the key holders to it

            GameManager.Instance.BossKeysManager.DistributeKeysAcrossRemainingRooms(rooms);

            // set the game difficulty
            SetDifficulty(GameManager.Instance.GameData.Difficulty);
        }

        private WaveData GetWaveWithinCapacityCount(int capacityCount, int capacityMargin)
        {
            int minCapacity = capacityCount - capacityMargin;
            int maxCapacity = capacityCount + capacityMargin;
            return _waves.FindAll(w => w.Capacity >= minCapacity && w.Capacity <= maxCapacity).RandomItem();
        }

        public List<EnemyWave> GetWaves(Room room)
        {
            // TODO: have this as a helper method
            List<EnemyWave> waves = new List<EnemyWave>();
            int wavesLeft = _wavesToSpawn.Count - _currentWaveIndex;

            int minWavesCount = Mathf.Max(1, wavesLeft - (_roomsWithNoSpawns - 1) * MAX_WAVES_PER_ROOM_COUNT);

            int maxWavesCount = Mathf.Min(MAX_WAVES_PER_ROOM_COUNT, wavesLeft - (_roomsWithNoSpawns - 1));

            int wavesCount = Random.Range(minWavesCount, maxWavesCount + 1);

            _roomsWithNoSpawns--;

            for (int i = 0; i < wavesCount; i++)
            {
                waves.Add(GetEnemyWave(room));
            }

            return waves;
        }

        /// <summary>
        /// Called everytime the played enters a room that has enemies to spawn. Because we decide what enemies to spawn after the player enters a room to control progression.
        /// </summary>
        /// <param name="room"></param>
        public EnemyWave GetEnemyWave(Room room)
        {
            if (!_spawnEnemies)
            {
                return null;
            }

            WaveData waveToCreate = _wavesToSpawn[_currentWaveIndex];
            ++_currentWaveIndex;

            List<EnemyType> waveEnemies = waveToCreate.Enemies;

            // create an empty wave and an empty list for enemies
            EnemyWave wave = new EnemyWave();

            for (int j = 0; j < waveEnemies.Count; j++)
            {
                EnemyData selectedEnemyData = null;
                selectedEnemyData = _enemiesDatabase.GetAllEnemies().Find(e => e.EnemyType == waveEnemies[j]);

                if (selectedEnemyData == null)
                {
                    Debug.LogError($"No enemy data of type {waveEnemies[j]}");
                    break;
                }

                EnemySpawnProfile enemyProfile = new EnemySpawnProfile(selectedEnemyData.EnemyType);
                wave.Enemies.Add(enemyProfile);
            }

            wave.HasKey = waveToCreate.HasKey;

            Painter.PositionEnemies(wave, room);

            return wave;
        }

        #region Pools
        private void InitPools()
        {
            foreach (EnemyData enemy in _enemiesDatabase.GetAllEnemies())
            {
                _enemiesPools.Add(enemy.EnemyType, CreateEnemyPool(enemy));
            }

            foreach (EnemyData enemy in _enemiesDatabase.GetAllEnemies())
            {
                if (enemy.PartsPrefab == null)
                {
                    continue;
                }
                
                _enemyPartsPools.Add(enemy.EnemyType, CreateEnemyPartsPool(enemy));
            }
        }

        private void DisposePools()
        {
            foreach (KeyValuePair<EnemyType, Pool<EnemyComponents>> enemy in _enemiesPools)
            {
                enemy.Value.Clear();
            }

            foreach (KeyValuePair<EnemyType, Pool<UnitParts>> enemyParts in _enemyPartsPools)
            {     
                 enemyParts.Value.Clear();
            }

            _enemiesPools.Clear();
            _enemyPartsPools.Clear();
        }

        private int _poolIndex = 0;

        private Pool<EnemyComponents> CreateEnemyPool(EnemyData enemyData)
        {
            var pool = new Pool<EnemyComponents>(
                () =>
                {
                    GameObject obj = Instantiate(enemyData.Prefab);
                    obj.SetActive(false);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    EnemyComponents enemy = obj.GetComponent<EnemyComponents>();
                    enemy.gameObject.name = ((EnemyData)enemy.Stats).EnemyName + $" {_poolIndex++}";
                    enemy.SetUp();
                    return enemy;
                },
                (EnemyComponents obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (EnemyComponents obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (EnemyComponents obj) => obj.GetComponent<IPoolable>().Clear(),
                0
            );
            return pool;
        }

        private Pool<UnitParts> CreateEnemyPartsPool(EnemyData enemyData)
        {
            var pool = new Pool<UnitParts>(
                () =>
                {
                    var obj = Instantiate(enemyData.PartsPrefab);
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
        #endregion

        public UnitParts GetEnemyPartsByType(EnemyType type)
        {
            UnitParts parts = _enemyPartsPools[type].RequestObject(Vector3.zero, Quaternion.identity);
            return parts;
        }

        public EnemyComponents RequestEnemy(EnemyType type)
        {
            EnemyComponents enemy = _enemiesPools[type].RequestObject(Vector3.zero, Quaternion.identity);
            _spawnedEnemies.Add(enemy);

            return enemy;
        }

        public void AddEnemy(TankComponents enemy)
        {
            _spawnedEnemies.Add(enemy);
        }

        public void RemoveEnemy(TankComponents enemy)
        {
            _spawnedEnemies.Remove(enemy);
        }

        public List<Transform> GetSpawnedEnemies()
        {
            List<Transform> enemies = new List<Transform>();
            _spawnedEnemies.ForEach(e => enemies.Add(e.transform));
            return enemies;
        }

        public void DestroyAllEnemies()
        {
            for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
            {
                _spawnedEnemies[i].Health.Die();
            }
        }

        public void SetDifficulty(float difficulty)
        {
            Difficulty = difficulty;
        }

        public void EnableSpawnEnemies(bool enable)
        {
            _spawnEnemies = enable;
        }

        public void SetFightActivated(bool value)
        {
            _fightActivated = value;
        }

        public bool IsFightActivated()
        {
            return _fightActivated;
        }

        public void SetTutorialWaves(List<WaveData> waves)
        {
            _tutorialWaves = waves;
        }

        public void ActivateSpawnedEnemies()
        {
            _spawnedEnemies.ForEach(e => e.Activate());
            EnemiesAreActivated = true;
        }

        public void DeactivateSpawnedEnemies()
        {
            _spawnedEnemies.ForEach(e => e.Deactivate());
            EnemiesAreActivated = false;
        }
    }
}
