using UnityEngine;

namespace TankLike.Combat.AirDrone
{
    using UnitControllers;

    public class AirDroneShooter : MonoBehaviour
    {
        [SerializeField] private Weapon _customShot;
        [SerializeField] private Transform _shootingPoint;
        [SerializeField] private float _bulletSpeed = 10f;
        [SerializeField] private GameTags _enemyTag;

        // to add credit to the owner of the drone
        private TankComponents _ownerShooter;
        private BulletData _bulletData;

        public void SetUp(TankComponents ownerShooter)
        {
            _ownerShooter = ownerShooter;

            _bulletData = new BulletData()
            {
                Speed = _bulletSpeed,
                Damage = _customShot.Damage,
                MaxDistance = 0,
                CanBeDeflected = false
            };
        }

        public void Shoot()
        {
            _customShot.OnShot(ShootAction, _shootingPoint, 0f);

        }

        private void ShootAction(Bullet bullet, Transform shootingPoint, float angle)
        {
            // create the bullet
            bullet.gameObject.SetActive(true);

            // handle position and rotation
            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            if (_shootingPoint != null)
            {
                rotation = _shootingPoint.rotation;
                position = _shootingPoint.position;
            }

            if (shootingPoint != null)
            {
                rotation = shootingPoint.rotation;
                position = shootingPoint.position;
            }

            if (angle != 0) rotation *= Quaternion.Euler(0f, angle, 0f);

            bullet.transform.SetPositionAndRotation(position, rotation);

            bullet.StartBullet(_ownerShooter);
            // give the bullet a target tag
            bullet.SetTargetLayerMask((TankAlignment)(int)_enemyTag);
            // set the bullet's values
            bullet.SetValues(_bulletData);
        }
    }
}
