using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using Combat.SkillTree.Upgrades;
    using UnitControllers;

    [CreateAssetMenu(fileName = "W_AOE_Default", menuName = DIRECTORY + "Area Of Effect")]
    public class AOEWeapon : Weapon
    {
        [field: SerializeField] public float ExplosionRadius;

        private Transform _explosionCenter;
        //[SerializeField] private AreaOfEffectImpact _onImpactModule;
        [SerializeField] private ImpactType _impactType;

        public override void OnShot(Transform shootingPoint = null, float angle = 0, bool freeRotation = false)
        {
            base.OnShot();

            if (_explosionCenter == null)
            {
                _explosionCenter = _tankTransform;
            }

            Vector3 currenExplosionCenter = _explosionCenter.position;

            if (shootingPoint != null)
            {
                currenExplosionCenter = shootingPoint.position;
            }

            //_onImpactModule.Impact(currenExplosionCenter, null, Damage, _targetLayerMask, null);

            Collider[] targets = Physics.OverlapSphere(currenExplosionCenter, ExplosionRadius, _targetLayerMask);
            HashSet<GameObject> damaged = new HashSet<GameObject>();

            foreach (Collider t in targets)
            {
                if (!damaged.Contains(t.gameObject))
                {
                    damaged.Add(t.gameObject);

                    if (t.TryGetComponent(out IDamageable damagable))
                    {
                        DamageInfo damageInfo = DamageInfo.Create()
                            .SetDamage(Damage)
                            .SetInstigator(_components)
                            .SetBulletPosition(currenExplosionCenter)
                            .SetDamageType(_impactType)
                            .Build();

                        //Debug.Log(t.gameObject.name);
                        damagable.TakeDamage(damageInfo);
                    }
                }
            }
        }

        public void OnShot(Vector3 shootingPoint, float angle = 0)
        {
            base.OnShot();

            if (_explosionCenter == null)
            {
                _explosionCenter = _tankTransform;
            }

            Vector3 currenExplosionCenter = _explosionCenter.position;

            if (shootingPoint != null)
            {
                currenExplosionCenter = shootingPoint;
            }

            Collider[] targets = Physics.OverlapSphere(shootingPoint, ExplosionRadius, _targetLayerMask);
            HashSet<GameObject> damaged = new HashSet<GameObject>();

            foreach (Collider t in targets)
            {
                if (!damaged.Contains(t.gameObject))
                {
                    damaged.Add(t.gameObject);

                    if (t.TryGetComponent(out IDamageable damagable))
                    {
                        DamageInfo damageInfo = DamageInfo.Create()
                            .SetDamage(Damage)
                            .SetInstigator(_components)
                            .SetBulletPosition(currenExplosionCenter)
                            .SetDamageType(_impactType)
                            .Build();

                        damagable.TakeDamage(damageInfo);
                    }
                }
            }
        }

        public override void OnShot(System.Action<Bullet, Transform, float> spawnBulletAction, Transform shootingPoint, float angle)
        {
            base.OnShot(spawnBulletAction, shootingPoint, angle);
        }

        public void SetExplosionRadius(float radius)
        {
            ExplosionRadius = radius;
        }

        public void SetExplosionCenter(Transform center)
        {
            _explosionCenter = center;
        }

        public override void Upgrade(BaseWeaponUpgrade weaponUpgrade)
        {

        }
    }
}
