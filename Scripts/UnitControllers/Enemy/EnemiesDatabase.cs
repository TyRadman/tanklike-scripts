using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "Enemies_DB_Default", menuName = Directories.ENEMIES + "Enemies DB")]
    public class EnemiesDatabase : ScriptableObject
    {
        [SerializeField] private List<EnemyData> _enemies;

        private Dictionary<EnemyType, EnemyData> _enemiesDB;

        private void OnEnable()
        {
            _enemiesDB = new Dictionary<EnemyType, EnemyData>();
            foreach (var enemy in _enemies)
                _enemiesDB.Add(enemy.EnemyType, enemy);
        }

        public EnemyData GetEnemyDataFromType(EnemyType type)
        {
            if (_enemiesDB.ContainsKey(type))
                return _enemiesDB[type];

            Debug.Log("Enemies DB does not contain an enemy with type -> " +  type);
            return null;
        }

        public List<EnemyData> GetAllEnemies()
        {
            return _enemies;
        }
    }
}
