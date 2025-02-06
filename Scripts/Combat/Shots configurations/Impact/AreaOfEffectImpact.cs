using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Combat;
    using Misc;

    [CreateAssetMenu(fileName = "OnImpact_AOE", menuName = MENU_MAIN + "Area Of Effect")]
    public class AreaOfEffectImpact : OnImpact
    {
        [SerializeField] private float _areaRadius = 2f;
        [SerializeField] private PoolableParticlesReference _onImpactAdditionalEffect;
        [SerializeField] private ImpactType _impactType;

        public float AreaRadius => _areaRadius;

        public override void Impact(Vector3 hitPoint, IDamageable target, int damage, LayerMask mask, Bullet bullet)
        {
            base.Impact(hitPoint, target, damage, mask, bullet);

            Collider[] targets = Physics.OverlapSphere(hitPoint, _areaRadius, mask);
            HashSet<GameObject> damaged = new HashSet<GameObject>();

            foreach (Collider t in targets)
            {
                if (!damaged.Contains(t.gameObject))
                {
                    damaged.Add(t.gameObject);

                    if (t.TryGetComponent(out IDamageable damagable))
                    {
                        if (damagable.IsInvincible)
                        {
                            continue;
                        }

                        DamageInfo damageInfo = DamageInfo.Create()
                            .SetDamage(damage)
                            .SetInstigator(bullet.GetInstigator())
                            .SetBulletPosition(bullet.transform.position)
                            .SetDamageDealer(bullet)
                            .SetDamageType(_impactType)
                            .Build();

                        damagable.TakeDamage(damageInfo); //not sure if we need the direction yet. Better have and not need it than need it and not have it :)
                    }
                }
            }

            if(_onImpactAdditionalEffect != null)
            {
                ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Impacts.GetImpact(_onImpactAdditionalEffect);
                vfx.transform.position = hitPoint;
                vfx.transform.localScale = Vector3.one * _areaRadius;
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }

            bullet.DisableBullet();
        }

        public override void Impact(Vector3 hitPoint, IDamageable target, int damage, LayerMask mask, UnitComponents instigator)
        {
            base.Impact(hitPoint, target, damage, mask, instigator);

            Collider[] targets = Physics.OverlapSphere(hitPoint, _areaRadius, mask);
            HashSet<GameObject> damaged = new HashSet<GameObject>();

            foreach (Collider t in targets)
            {
                if (!damaged.Contains(t.gameObject))
                {
                    damaged.Add(t.gameObject);

                    if (t.TryGetComponent(out IDamageable damagable))
                    {
                        if (damagable.IsInvincible)
                        {
                            continue;
                        }

                        DamageInfo damageInfo = DamageInfo.Create()
                            .SetDamage(damage)
                            .SetInstigator(instigator)
                            .SetBulletPosition(instigator.transform.position)
                            .SetDamageType(_impactType)
                            .Build();

                        damagable.TakeDamage(damageInfo); //not sure if we need the direction yet. Better have and not need it than need it and not have it :)
                    }
                }
            }

            if (_onImpactAdditionalEffect != null)
            {
                ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Impacts.GetImpact(_onImpactAdditionalEffect);
                vfx.transform.position = hitPoint;
                vfx.transform.localScale = Vector3.one * _areaRadius;
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }
        }

        public void SetAreaRadius(float radius)
        {
            _areaRadius = radius;
        }
    }
}
