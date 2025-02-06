using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.Shields
{
    using Combat;
    using Utils;

    [CreateAssetMenu(fileName = FILE_NAME_ROOT + "LaserDeflector_", menuName = MENU_ROOT + "Laser Deflector")]
    public class SIMLaserDeflector : ShieldOnImpactModule
    {
        [SerializeField] private LaserWeapon _laserWeapon;
        private List<ActiveLaser> _activeLasers = new List<ActiveLaser>();
        private string _laserGUID;

        private class ActiveLaser
        {
            public Laser Laser;
            public Laser SourceLaser;
            public float ActivationDuration = Laser.DAMAGE_INTERVAL + 0.1f;
        }

        public override void SetUp(TankComponents components, Shield shield)
        {
            base.SetUp(components, shield);

            _laserWeapon.SetUp(components);
            _laserGUID = _laserWeapon.BulletData.GUID;
        }

        public override void OnImpact(Ammunition damageDealer, Vector3 direction, Vector3 impactPoint)
        {
            Laser laser = damageDealer as Laser;
            bool isValid = laser != null && damageDealer.CanBeDeflected;

            if (!isValid)
            {
                return;
            }

            ActiveLaser activeLaser = _activeLasers.Find(l => l.SourceLaser == damageDealer);

            if(activeLaser == null)
            {
                ActiveLaser newLaser = new ActiveLaser()
                {
                    Laser = GameManager.Instance.VisualEffectsManager.Lasers.GetLaser(_laserGUID),
                    SourceLaser = laser
                    
                };

                _activeLasers.Add(newLaser);
                _laserWeapon.ShootLaser(newLaser.Laser, impactPoint, Quaternion.identity, false);
                _components.StartCoroutine(LaserUpdateRoutine(newLaser));
            }
            else
            {
                activeLaser.ActivationDuration += Laser.DAMAGE_INTERVAL;
            }

        }

        private IEnumerator LaserUpdateRoutine(ActiveLaser deflectedLaserData)
        {
            Laser mainLaser = deflectedLaserData.Laser;
            Laser sourceLaser = deflectedLaserData.SourceLaser;
            float timer = 0f;
            Transform shieldCenter = _components.transform;
            GameObject shieldObject = _shield.gameObject;

            while(timer < deflectedLaserData.ActivationDuration && sourceLaser.ImpactedObject == shieldObject)
            {
                timer += Time.deltaTime;

                Vector3 impactPoint = sourceLaser.ImpactPoint;

                Vector3 sourceLaserDirection = sourceLaser.ForwardDirection;

                Vector3 normalDirection = impactPoint - shieldCenter.position;

                Vector3 newDirection = Vector3.Reflect(sourceLaserDirection, normalDirection);

                Quaternion rotation = Quaternion.LookRotation(newDirection);

                mainLaser.UpdatePositionAndRotation(impactPoint, rotation);

                yield return null;
            }

            _laserWeapon.StopLaser(mainLaser);
            _activeLasers.Remove(deflectedLaserData);
        }
    }
}
