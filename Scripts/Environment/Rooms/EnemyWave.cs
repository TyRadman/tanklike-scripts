using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment
{
    /// <summary>
    /// One is held by every room that spawns enemies. Holds the profiles of the enemies to spawn and whether there is a key or not.
    /// </summary>
    [System.Serializable]
    public class EnemyWave
    {
        public List<EnemySpawnProfile> Enemies;
        public bool HasKey { get; set; } = false;

        public EnemyWave()
        {
            Enemies = new List<EnemySpawnProfile>();
        }
    }
}
