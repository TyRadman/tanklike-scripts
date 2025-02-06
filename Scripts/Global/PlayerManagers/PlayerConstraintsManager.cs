using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using UnitControllers;

    public class PlayerConstraintsManager : MonoBehaviour, IManager
    {
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

        public void ApplyConstraints(AbilityConstraint constraints)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            GameManager.Instance.PlayersManager.GetPlayerProfiles().
                ForEach(p => p.Constraints.ApplyConstraints(true, constraints));
        } 

        public void RemoveConstraints(AbilityConstraint constraints)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            GameManager.Instance.PlayersManager.GetPlayerProfiles().
                ForEach(p => p.Constraints.ApplyConstraints(false, constraints));
        }
    }
}
