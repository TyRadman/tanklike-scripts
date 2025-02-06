using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class PlayerCoinsManager : MonoBehaviour, IManager
    {
        public System.Action<int, int> OnCoinsAdded { get; set; }

        [field: SerializeField] public int CoinsAmount { get; private set; }

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;

            CoinsAmount = 0;
        }
        #endregion

        public void AddCoins(int amount)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            CoinsAmount += amount;

            if(amount > 0)
            {
                OnCoinsAdded?.Invoke(amount, CoinsAmount);
            }
        }
    }
}
