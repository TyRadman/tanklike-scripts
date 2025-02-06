using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using UnitControllers;
    using ItemsSystem;
    using Combat.Destructible;

    public class ReportManager : MonoBehaviour, IManager
    {
        public System.Action<Collectable, int> OnCollectableCollected { get; set; }
        public System.Action<DropperTag, int> OnObjectDestroyed;
        public System.Action<BossData, int> OnBossKill { get; set; }
        public System.Action<EntityEliminationReport> OnEnemyEliminated { get; set; }
        public System.Action<EnemyData, int> OnEnemyKill { get; set; }

        public bool IsActive { get; private set; }


        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        #region Reports
        public void ReportCollection(Collectable collectable, int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            OnCollectableCollected?.Invoke(collectable, playerIndex);
        }

        public void ReportEnemyElimination(EntityEliminationReport report)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            OnEnemyEliminated?.Invoke(report);
        }

        public void ReportDestroyingObject(DropperTag tag, int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            OnObjectDestroyed?.Invoke(tag, playerIndex);
        }

        public void ReportBossKill(TankComponents components)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            OnBossKill?.Invoke((BossData)components.Stats, ((BossHealth)components.Health).PlayerIndex);
        }
        #endregion
    }
}
