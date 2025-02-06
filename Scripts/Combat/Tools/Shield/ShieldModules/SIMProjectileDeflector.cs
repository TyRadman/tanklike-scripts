using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.Shields
{
    using Combat;

    /// <summary>
    /// A shield module that spawns projectiles when impacted by projectiles. More like a projectile deflector but we're keep it broad.
    /// </summary>
    [CreateAssetMenu(fileName = FILE_NAME_ROOT + "ProjectileDeflector_", menuName = MENU_ROOT + "Projectile Deflector")]
    public class SIMProjectileDeflector : ShieldOnImpactModule
    {
        [SerializeField] private Clonable<Weapon> _projectileWeapon;
        [Header("Damage")]
        [SerializeField] private bool _isDamageIndependent = true;
        [SerializeField] private float _damageMultiplier = 0.7f;

        public override void SetUp(TankComponents components, Shield shield)
        {
            base.SetUp(components, shield);

            _projectileWeapon.Initiate();

            _projectileWeapon.Instance.SetUp(components);
        }

        public override void OnImpact(Ammunition damageDealer, Vector3 direction, Vector3 impactPoint)
        {
            Bullet projectile = damageDealer as Bullet;
            bool isValid = projectile != null && damageDealer.CanBeDeflected;

            if (!isValid)
            {
                return;
            }

            Vector3 impactNormal = (impactPoint - _components.transform.position).normalized;

            Vector3 deflectionDirection = Vector3.Reflect(direction, impactNormal);

            // must convert the direction to a euler rotation because OnShot only takes this.
            Vector3 deflectionEuler = Quaternion.LookRotation(deflectionDirection).eulerAngles;

            if (!_isDamageIndependent)
            {
                int bulletDamage = (int)((float)projectile.Damage * _damageMultiplier);
                _projectileWeapon.Instance.SetDamage(bulletDamage);
            }
            
            _projectileWeapon.Instance.OnShot(impactPoint, deflectionEuler, 0f);
        }

        public void SetDamageMultiplier(float multiplier)
        {
            _damageMultiplier = multiplier;
        }

        public void SetDamageIndependence(bool isIndependent)
        {
            _isDamageIndependent = isIndependent;
        }

        public Weapon GetProjectileWeapon()
        {
            return _projectileWeapon;
        }

        public float GetDamageMultiplier()
        {
            return _damageMultiplier;
        }
    }
}
