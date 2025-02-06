using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike
{
    public class BulletsManager : MonoBehaviour
    {
        private List<Bullet> _activeBullets = new List<Bullet>();

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;

            _activeBullets.Clear();
            //_activeBullets = null; //TODO: Test this
        }
        #endregion

        public void AddBullet(Bullet bullet)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            if (!_activeBullets.Contains(bullet))
                _activeBullets.Add(bullet);
        }

        public void RemoveBullet(Bullet bullet)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            if (_activeBullets.Contains(bullet))
                _activeBullets.Remove(bullet);
        }

        public void DeactivateBullets()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            foreach (var bullet in _activeBullets)
            {
                bullet.TurnOff();
            }

            _activeBullets.Clear();
        }
    }
}
