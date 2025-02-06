using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TankLike.Tutorial
{
    using UnitControllers;

    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "EnemyElimination", menuName = MENU_PATH + "Enemy Elimination")]
    public class EnemyEliminationAchievement : TutorialAchievement
    {
        public int EnemiesLeft = 0;

        public override void SetUp(WaypointMarker marker)
        {
            base.SetUp(marker);

            EnemiesLeft = marker.EnemiesToSpawn.Count;

            if (EnemiesLeft > 0)
            {
                _tutorialManager.OnEnemyDeathEvent += OnEnemiesDeath;
            }
        }

        private void OnEnemiesDeath()
        {
            EnemiesLeft--;

            if (!IsAchieved())
            {
                return;
            }

            _tutorialManager.OnEnemyDeathEvent = null;
            OnAchievementCompleted?.Invoke();
        }

        public override bool IsAchieved()
        {
            return EnemiesLeft <= 0;
        }
    }
}
