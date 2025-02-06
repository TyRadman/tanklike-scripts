using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class BossesManager : MonoBehaviour, IManager
    {
        [Header("Boss Room")]
        [SerializeField] private Transform _bossRoomCenter;
        [SerializeField] private Vector3 _bossRoomSize;

        private List<BossComponents> _bosses = new List<BossComponents>();
        private BossesDatabase _bossesDatabase;
        private Dictionary<BossType, Pool<UnitParts>> _bossPartsPools = new Dictionary<BossType, Pool<UnitParts>>();

        public bool IsActive { get; private set; }
        public Transform BossRoomCenter => _bossRoomCenter;
        public Vector3 BossRoomSize => _bossRoomSize;

        public void SetReferences(BossesDatabase bossesDatabase)
        {
            _bossesDatabase = bossesDatabase;
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            InitPools();
        }

        public void Dispose()
        {
            IsActive = false;

            DisposePools();
        }
        #endregion

        public void SpawnBoss()
        {

        }

        public void AddBoss(BossComponents boss)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _bosses.Add(boss);
            boss.SetUp();
            boss.Restart();
        }

        public void RemoveBoss(TankComponents boss)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _bosses.Remove((BossComponents)boss);
        }

        public BossComponents GetBoss(int index)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return null;
            }

            return _bosses[index];
        }

        public UnitParts GetBossPartsByType(BossType type)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return null;
            }

            UnitParts parts = _bossPartsPools[type].RequestObject(Vector3.zero, Quaternion.identity);
            return parts;
        }

        public void SwitchBackBGMusic()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            StartCoroutine(SwitchBackBGMusicRoutine());
        }

        private IEnumerator SwitchBackBGMusicRoutine()
        {
            GameManager.Instance.AudioManager.FadeOutBGMusic();
            yield return new WaitForSeconds(1f);
            GameManager.Instance.AudioManager.SwitchBGMusic(GameManager.Instance.LevelGenerator.RoomsBuilder.GetCurrentLevelData().LevelMusic);
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.AudioManager.FadeInBGMusic();
        }

        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.DrawWireCube(BossRoomCenter.position, _bossRoomSize);
        //}

        #region Pools
        private void InitPools()
        {
            foreach (var boss in _bossesDatabase.GetAllBosses())
            {
                if (boss.PartsPrefab == null) continue;
                _bossPartsPools.Add(boss.BossType, CreateBossPartsPool(boss));
            }
        }

        private void DisposePools()
        {
            foreach (KeyValuePair<BossType, Pool<UnitParts>> bossParts in _bossPartsPools)
            {
                bossParts.Value.Clear();
            }

            _bossPartsPools.Clear();
        }

        private Pool<UnitParts> CreateBossPartsPool(BossData bossData)
        {
            var pool = new Pool<UnitParts>(
                () =>
                {
                    var obj = Instantiate(bossData.PartsPrefab);
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
    }
}
