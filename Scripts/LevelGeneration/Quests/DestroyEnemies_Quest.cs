using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;
using TankLike.Utils;

namespace TankLike.LevelGeneration.Quests
{
    [CreateAssetMenu(fileName = "DestroyEnemiesQuest", menuName = "Other/ Quest Types/ Destroy Enemies")]
    public class DestroyEnemies_Quest : Quest_SO
    {
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private Vector2Int _enemyCountRange = new Vector2Int() {x = 2, y = 7};
        private int _currentDestroyedEnemiesCount;
        private int _requiredEnemiesCount;

        public override void SetUp(LevelQuestSettings settings)
        {
            base.SetUp(settings);

            _requiredEnemiesCount = _enemyCountRange.RandomValue();
            _currentDestroyedEnemiesCount = 0;
            // start listening to the enemies' manager
            GameManager.Instance.ReportManager.OnEnemyKill += OnProgress;
        }

        private void OnProgress(EnemyData data, int playerIndex)
        {
            if (data.EnemyType != _enemyType)
            {
                return;
            }

            _currentDestroyedEnemiesCount++;

            OnProgress(this);
        }

        public override void OnCompletion()
        {
            base.OnCompletion();

            // unsubscribe from the report manager's event
            GameManager.Instance.ReportManager.OnEnemyKill -= OnProgress;
            // remove the quest from the quest manager
            GameManager.Instance.QuestsManager.MarkQuestAsCompleted(this);
        }

        public override string GetQuestString()
        {
            return $"Destroy {_requiredEnemiesCount.ToString().Color(Color.red)} enemies";
        }

        public override string GetQuestProgressString()
        {
            return $"{_currentDestroyedEnemiesCount} / {_requiredEnemiesCount}";
        }

        public override float GetProgressAmount()
        {
            return (float)_currentDestroyedEnemiesCount / (float)_requiredEnemiesCount;
        }

        public override bool IsCompleted()
        {
            return _currentDestroyedEnemiesCount >= _requiredEnemiesCount;
        }

        public override void CopyValuesTo(Quest_SO quest)
        {
            DestroyEnemies_Quest newQuest = (DestroyEnemies_Quest)(quest);
            newQuest.SetValues(_enemyType, _enemyCountRange);
        }

        public void SetValues(EnemyType enemyType, Vector2Int enemyCountRange)
        {
            _enemyType = enemyType;
            _enemyCountRange = enemyCountRange;
        }
    }
}
