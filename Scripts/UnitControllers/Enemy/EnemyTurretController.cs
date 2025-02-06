using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyTurretController : TankTurretController
    {
        [Header("Testing")]
        [SerializeField] private Transform _aimTarget;
        private bool _debugMode;

        public void SetDebugMode(bool value)
        {
            _debugMode = value;
        }

        private IEnumerator UpdateRoutine()
        {
            while (true)
            {
                HandleTurretRotation(_aimTarget);
                yield return null;
            }
        }

        //public void RotateToTarget(Vector3 targetPosition, bool interrupt = true)
        //{
        //    // the process of looking at a random target shouldn't interrupt the process of looking at the player
        //    if (!interrupt && _isRotatingToTarget) return;

        //    _isRotatingToTarget = true;
        //    _target = targetPosition;
        //    Vector3 direction = (_target - TankTransform.position).normalized;
        //    float angle = Vector3.SignedAngle(TankTransform.forward, direction, Vector3.up);
        //    _lookingDirection = Mathf.Sign(-angle);
        //}

        //private void RotationProcess()
        //{
        //    if (!_isRotatingToTarget) return;

        //    Vector3 direction = Vector3.Normalize(_target - TankTransform.position);
        //    float dot = Vector3.Dot(TankTransform.forward, direction);

        //    if (dot < DOT_DETECTION_THRESHOLD)
        //    {
        //        // to smooth the end of the rotation
        //        float t = Mathf.InverseLerp(_smoothingValuesDetectors.x, _smoothingValuesDetectors.y, dot);
        //        float multiplier = Mathf.Lerp(_smoothingValues.x, _smoothingValues.y, t);
        //        // rotating
        //        RotateTank(RotationSpeed * Mathf.Sign(-_lookingDirection) * multiplier);
        //    }
        //    else
        //    {
        //        // stop looking
        //        _isRotatingToTarget = false;
        //        // maybe send a message to shoot
        //    }
        //}

        public override void Activate()
        {
            base.Activate();

            if (_debugMode)
            {
                StartCoroutine(UpdateRoutine());
            }
        }
    }
}
