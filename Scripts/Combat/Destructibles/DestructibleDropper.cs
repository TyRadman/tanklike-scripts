using UnityEngine;
using System;

namespace TankLike.Combat.Destructible
{
    using Environment.LevelGeneration;
    using Sound;
    using UI.DamagePopUp;
    using UnitControllers;
    using ItemsSystem;
    using Environment;

    [RequireComponent(typeof(DamagePopUpAnchor))]
    public class DestructibleDropper : MonoBehaviour, IDamageable, IDropper, IAimAssistTarget
    {
        [field: SerializeField] public DropperTag DropperTag { get; private set; }
        [field: SerializeField] public DamagePopUpAnchor PopUpAnchor { get; private set; }
        public Transform Transform => transform;
        public bool IsInvincible { get; set; }
        public bool IsDead { get; private set; } = false;
        public Action<Transform> OnTargetDestroyed { get; set; }

        [Header("Stats")]
        [SerializeField] protected int _maxHealth = 100;
        [SerializeField] protected CollectablesDropSettings _dropSettings;
        
        [Header("Audio")]
        [SerializeField] protected Audio _destructionAudio;
        
        protected int _currentHealth;
        protected DestructibleDrop _drops;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void AssignAsTarget(Room room)
        {
            if (room == null)
            {
                Debug.LogError("Room is null");
                return;
            }

            room.Spawnables.AddDropper(transform);
            OnTargetDestroyed += room.Spawnables.RemoveDropper;
        }

        #region IDamagable

        public virtual void TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead)
            {
                return;
            }

            int damage = damageInfo.Damage;

            _currentHealth -= damage;

            GameManager.Instance.DamagePopUpManager.DisplayPopUp(DamagePopUpType.Damage, damage, PopUpAnchor.Anchor);

            if (_currentHealth <= 0)
            {
                Die();
                OnDestructibleDeath(damageInfo.Instigator);
            }
        }

        public virtual void Die()
        {
            IsDead = true;

            OnTargetDestroyed?.Invoke(transform);
            OnTargetDestroyed = null;
        }

        protected virtual void OnDestructibleDeath(UnitComponents tank)
        {
            GameManager.Instance.AudioManager.Play(_destructionAudio);
        }
        #endregion

        public void SetCollectablesToSpawn(DestructibleDrop drops)
        {
            _drops = drops;
        }

        public void SetDropSettings(CollectablesDropSettings settings)
        {
            _dropSettings = settings;
        }
    }
}
