using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment
{
    using UnitControllers;

    [CreateAssetMenu(fileName = "WD_00", menuName = "Level/Enemy wave data")]
    public class WaveData : ScriptableObject
    {
        [field : SerializeField] public bool HasKey { get; set; } = false;
        [Range(1, 5)] public int Difficulty = 1;
        public List<EnemyType> Enemies;
        public int Capacity;
        [SerializeField] private EnemiesDatabase _dataBase;

        public void SetCapacity()
        {
            Capacity = 0;

            for (int i = 0; i < Enemies.Count; i++)
            {
                EnemyData enemy = _dataBase.GetAllEnemies().Find(e => e.EnemyType == Enemies[i]);

                if (enemy == null)
                {
                    continue;
                }

                Capacity += enemy.Rank;
            }
        }

        public bool CanHaveKey()
        {
            // get the list of tags of the enemies that can't hold the key
            List<EnemyType> nonKeyHolders = GameManager.Instance.Constants.NotKeyHolderEnemyTags;

            // if the list of enemies that can't hold the key contain the current tag then the wave has a key holder
            if (Enemies.Exists(e => !nonKeyHolders.Exists(ee => ee == e)))
            {
                return true;
            }

            if(Enemies.Exists(e => e is EnemyType.Aimer or EnemyType.Archer or EnemyType.Laser))
            {
                Debug.Log($"WHYYYY is it false at {this.name}");
            }

            return false;
        }
    }
}
