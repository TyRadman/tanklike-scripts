using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using Combat.SkillTree.Upgrades;
    using System;
    using UnitControllers;
    using Utils;

    [CreateAssetMenu(fileName = "W_Laser_NAME", menuName = DIRECTORY + "Laser")]
    public class LaserWeapon : Weapon
    {
        [Header("Laser Settings")]
        [SerializeField] private float _duration;
        [field: SerializeField] public float MaxLength;
        [field: SerializeField] public float Thickness;

        private List<Transform> _shootingPoints;

        public List<IPoolable> ActiveLasers { get; private set; } = new List<IPoolable>();

        public override void SetUp(UnitComponents components)
        {
            base.SetUp(components);

            ActiveLasers = new List<IPoolable>();

            if(_components.GetShooter() != null)
            {
                _shootingPoints = _components.GetShooter().GetShootingPoints();
            }
            else
            {
                Debug.LogError("No shooting points found for the laser weapon.");
            }
        }

        public override void OnShot(Transform shootingPoint = null, float angle = 0, bool freeRotation = false)
        {
            base.OnShot();

            Laser laser = GameManager.Instance.VisualEffectsManager.Lasers.GetLaser(BulletData.GUID);
            ShootLaser(laser, shootingPoint, angle);
        }

        #region Shooting Methods overloads
        public void ShootLaser(Laser laser, Transform shootingPoint = null, float angle = 0)
        {
            laser.SetValues(MaxLength, Thickness, Damage, _duration, CanBeDeflected);
            // create the bullet
            laser.gameObject.SetActive(true);

            // handle position and rotation
            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            if (shootingPoint == null)
            {
                if (_shootingPoints != null)
                {
                    shootingPoint = _shootingPoints[0];
                }
            }

            if (shootingPoint != null)
            {
                rotation = shootingPoint.rotation;
                position = shootingPoint.position;
            }

            if (angle != 0)
            {
                rotation *= Quaternion.Euler(0f, angle, 0f);
            }

            laser.transform.SetPositionAndRotation(position, rotation);
            laser.transform.parent = shootingPoint;
            laser.transform.localPosition = Vector3.zero;
            laser.SetUp(_components, RemoveFromActivePoolables);
            laser.SetTargetLayerMask(Helper.GetOpposingTag(_components.gameObject.tag));
            // activate laser
            laser.Activate();
            ActiveLasers.Add(laser);
        }

        public void ShootLaser(Laser laser, Vector3 position, Quaternion rotation, bool autoStop = true, float angle = 0f)
        {
            laser.SetValues(MaxLength, Thickness, Damage, _duration, CanBeDeflected);

            // create the bullet
            laser.gameObject.SetActive(true);

            if (angle != 0)
            {
                rotation *= Quaternion.Euler(0f, angle, 0f);
            }

            laser.transform.SetPositionAndRotation(position, rotation);
            laser.SetUp(_components, RemoveFromActivePoolables);
            laser.SetTargetLayerMask(Helper.GetOpposingTag(_components.gameObject.tag));
            // activate laser
            laser.Activate(autoStop);
            ActiveLasers.Add(laser);
        }
        #endregion

        public void StopLaser(Laser laserToStop)
        {
            laserToStop.Deactivate();
        }

        private void RemoveFromActivePoolables(IPoolable poolable)
        {
            if (ActiveLasers.Contains(poolable))
            {
                ActiveLasers.Remove(poolable);
            }
        }

        public void SetDuration(float duration)
        {
            _duration = duration;
        }

        public float GetLaserDuration()
        {
            return _duration;
        }

        public override void DisposeWeapon()
        {
            base.DisposeWeapon();

            if (ActiveLasers.Count > 0)
            {
                ActiveLasers.ForEach(e => e.TurnOff());
            }

            ActiveLasers.Clear();
        }

        public override void Upgrade(BaseWeaponUpgrade weaponUpgrade)
        {

        }
    }
}
