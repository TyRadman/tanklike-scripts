using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "OnImpact_ProjectileSpawn", menuName = MENU_MAIN + "Spawn Projectiles")]
    public class ProjectileImpactSpawnProjectiles : OnImpact
    {
        [SerializeField] private Weapon _projectileWeapon;
        [SerializeField] private float _angle = 45f;
        [SerializeField] private int _projectilesCount = 3;

        public override void Impact(Vector3 hitPoint, IDamageable target, int damage, LayerMask mask, Bullet bullet)
        {
            base.Impact(hitPoint, target, damage, mask, bullet);
            float angleIncement = _angle / (_projectilesCount - 1);
            float currentAngle = bullet.transform.eulerAngles.y - _angle / 2;

            _projectileWeapon.SetUp(bullet.GetInstigator());

            for (int i = 0; i < _projectilesCount; i++)
            {
                Vector3 rotation = new Vector3(0f, currentAngle, 0f);
                _projectileWeapon.OnShot(bullet.transform.position, rotation);
                currentAngle += angleIncement;
            }

            bullet.DisableBullet();
        }
    }
}
