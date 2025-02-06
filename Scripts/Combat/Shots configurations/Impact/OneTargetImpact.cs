using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Combat;

    [CreateAssetMenu(fileName = "OnImpact_SingleTarget", menuName = MENU_MAIN + "Single Target")]
    public class OneTargetImpact : OnImpact
    {
        public override void Impact(Vector3 hitPoint, IDamageable target, int damage, LayerMask mask, Bullet bullet)
        {
            base.Impact(hitPoint, target, damage, mask, bullet);

            // whatever definition the target has for this method
            if (target != null)
            {
                Transform bulletTransform = bullet.transform;

                DamageInfo damageInfo = DamageInfo.Create()
                    .SetDamage(damage)
                    .SetInstigator(bullet.GetInstigator())
                    .SetBulletPosition(bulletTransform.position)
                    .SetDamageDealer(bullet)
                    .Build();

                target.TakeDamage(damageInfo); //not sure if we need the direction yet
            }

            // play the effect 
            bullet.DisableBullet();
        }
    }
}
