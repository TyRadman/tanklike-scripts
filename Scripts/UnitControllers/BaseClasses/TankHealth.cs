using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Combat;
    using UI.DamagePopUp;
    using Cam;
    using ScreenFreeze;
    using Utils;

    [RequireComponent(typeof(DamagePopUpAnchor))]
    public abstract class TankHealth : MonoBehaviour, IController, IDamageable, IConstraintedComponent, IUnitDataReciever
    {
        public System.Action OnHit { get; set; }
        public System.Action<TankComponents> OnDeath { get; set; }
        public System.Action OnHealthFullyCharged { get; set; }

        [Header("Settings")]
        [SerializeField] protected int _maxHealth;
        [SerializeField] protected bool _canTakeDamage = true;
        [field: SerializeField] public bool IsInvincible { get; protected set; } = false;

        [Header("References")]
        [SerializeField] protected TankHealthEffect _damageEffects;
        [field: SerializeField] public DamagePopUpAnchor PopUpAnchor { get; protected set; }
        [field: SerializeField] public ScreenFreezeData DeathScreenFreeze { get; protected set; }

        public bool IsDead { get; protected set; } = true;
        public bool IsActive { get; protected set; }

        public delegate void OnHealthValueChangedDelegate(HealthChangeArgs args);
        // we add an 'event' so that it can't be reset, or invoked externally 
        public event OnHealthValueChangedDelegate OnHealthValueChanged;

        private TankComponents _components;
        protected UnitData _stats;
        protected List<DamageDetector> _damageDetectors;
        protected int _currentHealth;
        protected int _lastCurrentHealth;
        protected float _damageMultiplier = 0f;

        public const float DAMAGE_POP_UP_OFFSET = 2f;

        [HideInInspector] public Transform Transform => transform;

        public bool IsConstrained { get; set; }

        public virtual void SetUp(IController controller)
        {
            if (controller is not TankComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _components = components;
            TankBodyParts parts = _components.TankBodyParts;
            _stats = _components.Stats;

            _damageDetectors = ((TankBody)parts.GetBodyPartOfType(BodyPartType.Body)).DamageDetectors;
        }

        public virtual void TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead)
            {
                return;
            }

            if (!_canTakeDamage || IsConstrained)
            {
                return;
            }

            OnHit?.Invoke();

            int damage = damageInfo.Damage;

            // if it's damage that we're dealing with, not health increment, then we apply the multiplier to it
            if (damage > 0)
            {
                damage += Mathf.RoundToInt(damage * _damageMultiplier);
            }

            // TODO: should we have all the damage values be negative to begin with to avoid converting them to negative here?
            DamageInfo newDamageInfo = DamageInfo.Create()
                .SetDamage(damage * -1)
                .SetDirection(damageInfo.Direction)
                .SetInstigator(damageInfo.Instigator)
                .SetBulletPosition(damageInfo.BulletPosition)
                .SetDamageDealer(damageInfo.DamageDealer)
                .SetDamageType(damageInfo.DamageType)
                .SetDisplayPopUp(damageInfo.DisplayPopUp)
                .Build();

            AddToHealthValue(newDamageInfo);
            _damageEffects.SetIntensity((float)_currentHealth / (float)_maxHealth);

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }

            if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }
        }

        public virtual void Die()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            GameManager.Instance.ScreenFreezer.FreezeScreen(DeathScreenFreeze);
            _components.VisualEffects.PlayDeathEffects();
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.EXPLOSION);
            OnDeath?.Invoke(_components);
        }

        public virtual void Heal(int amount)
        {
            if (_currentHealth >= _maxHealth)
            {
                return;
            }

            DamageInfo damageInfo = DamageInfo.Create()
                .SetDamage(amount)
                .Build();

            AddToHealthValue(damageInfo);

            _damageEffects.SetIntensity((float)_currentHealth / (float)_maxHealth);

            if (_currentHealth >= _maxHealth)
            {
                OnHealthFullyCharged?.Invoke();
                _currentHealth = _maxHealth;
            }
        }

        public void SetDamageIntakeMultiplier(float value)
        {
            _damageMultiplier = value;
        }

        public void AddToMaxHealth(int amount)
        {
            float t = Mathf.InverseLerp(0, _maxHealth, _currentHealth);
            _maxHealth += amount;
            // adjust current health in accordance
            _currentHealth = (int)Mathf.Lerp(0, _maxHealth, t);
        }

        public void SetMaxHealth(int health, bool refillHealth = true)
        {
            _maxHealth = health;

            if (refillHealth)
            {
                _currentHealth = health;
            }
        }

        public void SetDamageDetectorsLayer(int layer)
        {
            _damageDetectors.ForEach(d => d.gameObject.layer = layer);
        }

        public int GetDamageDetectorsLayer()
        {
            if (_damageDetectors.Count == 0)
            {
                Debug.LogError($"No damage detectors at {gameObject.name}");
                return -1;
            }

            return _damageDetectors[0].gameObject.layer;
        }

        public void ReplenishFullHealth()
        {
            Heal(_maxHealth - _currentHealth);
        }

        public bool IsFull()
        {
            return _currentHealth == _maxHealth;
        }

        public virtual void RestoreFullHealth()
        {
            int healthToAdd = _maxHealth - _currentHealth;

            DamageInfo damageInfo = DamageInfo.Create()
                .SetDamage(healthToAdd)
                .SetDisplayPopUp(false)
                .Build();

            AddToHealthValue(damageInfo);
        }

        public int GetMaxHP()
        {
            return _maxHealth;
        }

        #region Utilities
        public virtual void SetHealthPercentage(float value, bool playEffects = true)
        {
            value = Mathf.Clamp01(value);

            int expectedHealth = Mathf.CeilToInt((float)_maxHealth * value);

            // if the current health is greater than the new health, then the change is a damage, otherwise, it's a heal
            if (_currentHealth >= expectedHealth)
            {
                int damageAmountToInflict = _currentHealth - expectedHealth;

                DamageInfo damageInfo = DamageInfo.Create()
                    .SetDamage(damageAmountToInflict)
                    .Build();

                TakeDamage(damageInfo);
            }
            else
            {
                int amountTohealth = expectedHealth - _currentHealth;
                Heal(amountTohealth);
            }
        }

        /// <summary>
        /// Changes the health value by the given amount.
        /// </summary>
        /// <param name="value"></param>
        protected virtual void AddToHealthValue(DamageInfo damageInfo)
        {
            int value = damageInfo.Damage;

            _lastCurrentHealth = _currentHealth;
            _currentHealth += value;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

            // Display pop up
            if (PopUpAnchor != null && damageInfo.DisplayPopUp)
            {
                DamagePopUpType type = value < 0 ? DamagePopUpType.Damage : DamagePopUpType.Heal;
                GameManager.Instance.DamagePopUpManager.DisplayPopUp(type, value, PopUpAnchor.Anchor);
            }

            HealthChangeArgs args = new HealthChangeArgs()
            {
                CurrentHP = _currentHealth,
                MaxHP = _maxHealth,
                LastHP = _lastCurrentHealth
            };

            OnHealthValueChanged?.Invoke(args);
        }

        protected virtual void SetHealthValue(int value)
        {
            _lastCurrentHealth = _currentHealth;
            _currentHealth = value;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

            if (PopUpAnchor != null)
            {
                DamagePopUpType type = value < 0 ? DamagePopUpType.Damage : DamagePopUpType.Heal;
                GameManager.Instance.DamagePopUpManager.DisplayPopUp(type, value, PopUpAnchor.Anchor);
            }

            HealthChangeArgs args = new HealthChangeArgs()
            {
                CurrentHP = _currentHealth,
                MaxHP = _maxHealth,
                LastHP = _lastCurrentHealth
            };

            OnHealthValueChanged?.Invoke(args);
        }

        internal int GetHealthAmount()
        {
            return _currentHealth;
        }

        internal float GetHealthAmount01()
        {
            return (float)_currentHealth / (float)_maxHealth;
        }

        internal float GetMaxHealth()
        {
            return (float)_maxHealth;
        }

        internal void SetHealthAmount(int value, int enforcedLastValue = 0)
        {
            if (enforcedLastValue != 0)
            {
                _currentHealth = enforcedLastValue;
            }

            SetHealthValue(value);
        }
        #endregion

        #region Constraints
        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canTakeDamage = (constraints & AbilityConstraint.TakingDamage) == 0;
            IsConstrained = !canTakeDamage;
        }
        #endregion

        #region IController
        public virtual void Activate()
        {
            IsDead = false;
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {
            IsDead = false;
            IsInvincible = false;
            IsConstrained = false;

            OnHit += _components.Visuals.OnHitHandler;
        }

        public virtual void Dispose()
        {
            OnHit -= _components.Visuals.OnHitHandler;
        }
        #endregion

        #region IUnitDataReciever
        public virtual void ApplyData(UnitData data)
        {

        }
        #endregion
    }

    public class HealthChangeArgs
    {
        public int CurrentHP;
        public int MaxHP;
        public int LastHP;
        public bool UpdateBar = true;
        public bool DisplayPopUp = true;
    }
}
