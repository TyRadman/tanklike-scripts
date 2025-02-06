using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;

namespace TankLike.UnitControllers
{
    public class DamageDetector : MonoBehaviour, IDamageable
    {
        #region Not Used
        public bool IsInvincible => _health.IsInvincible;
        public Transform Transform => _health.Transform;
        public bool IsDead => _health.IsDead;

        public void Die()
        {

        }

        public DamagePopUpAnchor PopUpAnchor { set; get; }
        #endregion

        [SerializeField] private TankHealth _health;

        public void TakeDamage(DamageInfo damageInfo)
        {
            if(_health == null)
            {
                Debug.LogError("Health reference is null");
                return;
            }

            _health.TakeDamage(damageInfo);
        }

#if UNITY_EDITOR
        public int GetLayerMask()
        {
            if(_health == null)
            {
                return -1;
            }

            TankComponents com = _health.GetComponent<TankComponents>();
        
            if(com is PlayerComponents)
            {
                return Constants.PlayerDamagableLayer;
            }
            else
            {
                return Constants.EnemyDamagableLayer;
            }
        }

        public bool HasHealthReference()
        {
            return _health != null;
        }

        public void SetHealth(TankHealth health)
        {
            _health = health;
        }
#endif
    }
}
