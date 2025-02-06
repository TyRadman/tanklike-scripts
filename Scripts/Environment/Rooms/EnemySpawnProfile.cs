using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment
{
    /// <summary>
    /// Holds information about the enemy to spawn such as the type of enemy, whether they can hold a key, and whether they're holding a key.
    /// </summary>
    [System.Serializable]
    public class EnemySpawnProfile
    {
        public EnemyType Enemy;
        public bool HasKey { get; set; }
        public bool CanHaveKey = true;
        public Vector3 SpawnPointPosition { get; private set; } = Vector3.one * 10f;

        public EnemySpawnProfile(EnemyType enemy)
        {
            // check whehter this enemy is one of the enemies that can't hold keys
            CanHaveKey = !GameManager.Instance.Constants.NotKeyHolderEnemyTags.Exists(t => t == enemy);
            Enemy = enemy;
            HasKey = false;
        }

        public void SetSpawnPoint(Vector3 position)
        {
            SpawnPointPosition = position;
        }
    }
}
