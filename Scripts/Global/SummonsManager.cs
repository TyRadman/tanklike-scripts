using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike
{
    public class SummonsManager : MonoBehaviour, IManager
    {
        private List<SummonAIController> _activeSummons = new List<SummonAIController>();

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;

            _activeSummons.Clear();
        }
        #endregion

        public void AddSummon(SummonAIController summon)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            if (!_activeSummons.Contains(summon))
                _activeSummons.Add(summon);
        }

        public void RemoveSummon(SummonAIController summon)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            if (_activeSummons.Contains(summon))
                _activeSummons.Remove(summon);
        }

        public SummonAIController GetSummon(int index)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            return _activeSummons[index];
        }

        public int GetActiveSummonsCount()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return 0;
            }

            return _activeSummons.Count;
        }
    }
}
