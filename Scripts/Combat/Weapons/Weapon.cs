using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using UnitControllers;
    using static IndicatorEffects;
    using Sound;
    using Combat.SkillTree.Upgrades;
    using Attributes;

    public abstract class Weapon : ScriptableObject
    {
        public const string DIRECTORY = Directories.AMMUNITION + "Weapons/";
        [field: SerializeField, AllowCreationIfNull] public AmmunationData BulletData { get; private set; }
        [field: SerializeField, Header("Weapon Settings")] public float CoolDownTime { get; protected set; }
        [field: SerializeField] public float AmmoRechargeSpeed { get; protected set; } = 0f;
        [field: SerializeField] public int AmmoCapacity { get; protected set; } = 5;
        [field: SerializeField] public int Damage { get; protected set; }
        [field: SerializeField] public bool CanBeDeflected { get; protected set; } = true;
        [field: SerializeField] public Audio ShotAudio { get; private set; }

        public System.Action OnWeaponShot { get; set; }

        [SerializeField] private Sprite _icon;

        protected Transform _tankTransform;
        protected UnitComponents _components;

        [Tooltip("Set this only if the \'OnShot\' method doesn't pass a tank component.")]
        [SerializeField] protected LayerMask _targetLayerMask;

        public virtual void SetUp(UnitComponents components)
        {
            _tankTransform = components.transform;
            _components = components;
        }

        public virtual void OnShot(Transform shootingPoint = null, float angle = 0, bool freeRotation = false)
        {
            if(ShotAudio != null)
            {
                GameManager.Instance.AudioManager.Play(ShotAudio);
            }
        }

        /// <summary>
        /// Shoots the projectile from the given point and with the given rotation plus an angle offset.
        /// </summary>
        /// <param name="spawnPoint"></param>
        /// <param name="rotation">the rotation must euler rotation.</param>
        /// <param name="angle"></param>
        public virtual void OnShot(Vector3 spawnPoint, Vector3 rotation, float angle = 0)
        {
            if (ShotAudio != null)
            {
                GameManager.Instance.AudioManager.Play(ShotAudio);
            }
            else
            {
                Debug.LogError($"No ShotAudio found for the weapon {this.name}");
            }
        }

        public virtual void OnShot(System.Action<Bullet, Transform, float> spawnBulletAction, Transform shootingPoint, float angle)
        {
        }

        public void SetDamage(int damage)
        {
            Damage = damage;
        }

        public virtual void SetSpeed(float speed)
        {
        }

        public void SetTargetLayer(LayerMask targetLayers)
        {
            _targetLayerMask = targetLayers;
        }

        public virtual void DisposeWeapon()
        {
            OnWeaponShot = null;
        }

        public Sprite GetIcon()
        {
            return _icon;
        }

        public abstract void Upgrade(BaseWeaponUpgrade weaponUpgrade);

        public virtual void UpgradeDamage(int damage)
        {
            Damage += damage;
        }

        internal virtual float GetCoolDownTime()
        {
            return CoolDownTime;
        }
    }
}
