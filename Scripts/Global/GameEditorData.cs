using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using Environment;

    [CreateAssetMenu(fileName = "GameEditorData", menuName = Directories.EDITOR + "Game Editor Settings")]
    public class GameEditorData : ScriptableObject
    {
        [field: SerializeField] public int PlayersCount { get; set; }
        [field: SerializeField, Header("Enemies")] public bool SpawnEnemies{ get; set; }
        [field: SerializeField] public float Difficulty { get; set; }
        public List<WaveData> EnemyTutorialWaves = new List<WaveData>();
        [field: SerializeField] public RoomType StartRoomType { get; set; } = RoomType.Normal;
        [field: SerializeField] public bool SpawnShop { get; set; } = true;
        [field: SerializeField] public bool SpawnWorkshop { get; set; } = true;

        public void InitializeValues()
        {
            SetEnemies();
            SetEnemyWaves();
        }

        private void SetEnemies()
        {
            GameManager.Instance.EnemiesManager.EnableSpawnEnemies(SpawnEnemies);
            GameManager.Instance.EnemiesManager.SetDifficulty(Difficulty);
        }

        private void SetEnemyWaves()
        {
            GameManager.Instance.EnemiesManager.SetTutorialWaves(EnemyTutorialWaves);
        }
    }
}
