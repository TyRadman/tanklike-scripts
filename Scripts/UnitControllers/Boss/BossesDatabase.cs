using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "Bosses_DB_Default", menuName = Directories.BOSSES + "Bosses DB")]
    public class BossesDatabase : ScriptableObject
    {
        [SerializeField] private List<BossData> _bosses;

        private Dictionary<BossType, BossData> _bossesDB;

        private void OnEnable()
        {
            _bossesDB = new Dictionary<BossType, BossData>();
            foreach (var boss in _bosses)
                _bossesDB.Add(boss.BossType, boss);
        }

        public BossData GetBossDataByType(BossType type)
        {
            if (_bossesDB.ContainsKey(type))
                return _bossesDB[type];

            Debug.Log("Bosses DB does not contain a boss with type -> " + type);
            return null;
        }

        public List<BossData> GetAllBosses()
        {
            return _bosses;
        }
    }
}
