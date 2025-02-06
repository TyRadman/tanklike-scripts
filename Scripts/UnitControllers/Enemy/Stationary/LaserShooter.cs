using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Cam;
    using Combat;
    using Misc;

    public class LaserShooter : EnemyShooter
    {
        public System.Action OnLaserFacedTarget;

        [Header("Laser Settings")]
        [SerializeField] private LayerMask _wallLayers;
        [SerializeField] private IndicatorEffects.IndicatorType _indicatorType;

        public override void DefaultShot(Transform shootingPoint = null, float angle = 0)
        {
            if (_currentWeapon == null)
            {
                return;
            }

            OnShootStarted?.Invoke();

            foreach (Transform point in ShootingPoints)
            {
                _currentWeapon.OnShot(point, angle);
            }

            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);
        }

        protected override IEnumerator TelegraphRoutine()
        {
            float duration = 0f;
            List<IPoolable> activeEffects = new List<IPoolable>();
            List<Indicator> activeIndicators = new List<Indicator>();

            float timer = 0f;

            foreach (Transform point in ShootingPoints)
            {
                ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Telegraphs.EnemyTelegraph;
                vfx.transform.SetLocalPositionAndRotation(point.position, point.rotation);
                vfx.transform.position += vfx.transform.forward * _telegraphOffset;
                vfx.transform.parent = point;
                vfx.gameObject.SetActive(true);
                vfx.Play(vfx.Particles.main.duration / _telegraphDuration);
                _activePoolables.Add(vfx);
                activeEffects.Add(vfx);

                if(_indicatorType != IndicatorEffects.IndicatorType.None)
                {
                    Indicator indicator = GameManager.Instance.VisualEffectsManager.Indicators.GetIndicatorByType(_indicatorType);
                    indicator.gameObject.SetActive(true);
                    var pos = point.position;
                    pos.y = 0.52f; // dirty 
                    indicator.transform.SetPositionAndRotation(pos, point.rotation);         
                    indicator.transform.parent = point;
                    indicator.Play();
                    _activePoolables.Add(indicator);
                    activeIndicators.Add(indicator);
                }
               
                if (duration == 0f) duration = vfx.Particles.main.duration;
            }

            // Update indicators
            while (timer < _telegraphDuration)
            {
                for (int i = 0; i < ShootingPoints.Count; i++)
                {
                    var point = ShootingPoints[i];
                    Ray ray = new Ray(point.position, point.forward);
                    bool cast = Physics.SphereCast(ray, ((LaserWeapon)_currentWeapon).Thickness, out RaycastHit hit, 50, _wallLayers);
                    Vector3 hitPoint = point.position + point.forward * ((LaserWeapon)_currentWeapon).MaxLength;

                    if (cast && (1 << hit.transform.gameObject.layer & _wallLayers) == 1 << hit.transform.gameObject.layer)
                    {
                        hitPoint = hit.point;
                    }

                    float dist = Vector3.Distance(point.position, hitPoint);
                    Vector3 indicatorSize = new Vector3(((LaserWeapon)_currentWeapon).Thickness, 0f, dist);

                    activeIndicators[i].transform.localScale = indicatorSize;
                }
                
                timer += Time.deltaTime;
                yield return null;
            }

            OnTelegraphFinished?.Invoke();

            foreach (var effect in activeEffects)
            {
                effect.TurnOff();

                if (_activePoolables.Contains(effect))
                {
                    _activePoolables.Remove(effect);
                }
            }

            foreach (var indicator in activeIndicators)
            {
                indicator.TurnOff();

                if (_activePoolables.Contains(indicator))
                {
                    _activePoolables.Remove(indicator);
                }
            }

            activeEffects.Clear();
            activeIndicators.Clear();
        }

        public virtual void AimLaserAtTarget(Transform target)
        {
            float rotationAmount;
            float rotationSpeed = 50f;

            Vector3 direction = (target.position - transform.position).normalized;

            Vector3 closestDirection = ShootingPoints[0].forward;
            float maxDot = Vector3.Dot(direction, closestDirection);

            for (int i = 1; i < ShootingPoints.Count; i++)
            {
                float dot = Vector3.Dot(direction, ShootingPoints[i].forward);

                if (dot > maxDot)
                {
                    maxDot = dot;
                    closestDirection = ShootingPoints[i].forward;
                }
            }

            float angle = Vector3.SignedAngle(direction, closestDirection, Vector3.up);
            float dirDot = Vector3.Dot(direction, closestDirection);

            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            // Calculate an angle threshold based on the distance
            float angleThreshold = Mathf.Lerp(13f, 2.3f, distanceToPlayer / 13f);

            float angleToTarget = Quaternion.Angle(Quaternion.LookRotation(direction), Quaternion.LookRotation(closestDirection));

            if (angleToTarget < angleThreshold)
            {
                OnLaserFacedTarget?.Invoke();
            }

            //if (dirDot >= 0.999f)
            //{
            //    OnLaserFacedTarget?.Invoke();
            //}

            if (angle > 1f)
            {
                rotationAmount = -1f;
            }
            else if (angle < 1f)
            {
                rotationAmount = 1f;
            }
            else
            {
                rotationAmount = 0f;
            }

            _turret.Rotate(rotationSpeed * rotationAmount * Time.deltaTime * Vector3.up);
        }

        #region IController
        public override void Restart()
        {
            base.Restart();
        }
        #endregion
    }
}
