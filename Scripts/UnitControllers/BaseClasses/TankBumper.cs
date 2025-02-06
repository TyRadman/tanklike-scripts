using System.Collections;
using System.Collections.Generic;
using TankLike.Combat.Destructible;
using UnityEngine;

namespace TankLike
{
    public class TankBumper : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _sparklesParticles;
        [field: SerializeField] public bool IsActive { get; private set; }
        [SerializeField] private int _wallsCount = 0;
        [SerializeField] private Transform _rightPoint;
        [SerializeField] private Transform _leftPoint;
        [SerializeField] private LayerMask _hittables;
        private const int BUMPERLAYER = 21;
        private const int EMPTY_LAYER = 5;
        private bool _isPositioning = false;
        private float _frictionAngle = 0f;
        private const float BUMPER_SPEED_REDUCTION_AMOUNT = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            IBoostDestructible destructible = other.GetComponent<IBoostDestructible>();

            if(destructible != null)
            {
                destructible.Destruct();
                return;
            }

            _sparklesParticles.Play();
            _wallsCount++;

            if (!_isPositioning)
            {
                StartCoroutine(SparklePositioningProcess());
            }
        }

        private IEnumerator SparklePositioningProcess()
        {
            _isPositioning = true;

            while (_wallsCount > 0)
            {
                GetFrictionAngle();

                yield return null;
            }

            _isPositioning = false;
        }

        private void GetFrictionAngle()
        {
            bool wallInTheMiddle = ScanForWall(transform);

            if (wallInTheMiddle)
            {
                return;
            }

            bool wallOnTheLeft = ScanForWall(_leftPoint);

            if (wallOnTheLeft)
            {
                return;
            }

            ScanForWall(_rightPoint);
        }

        private bool ScanForWall(Transform startPoint)
        {
            Ray ray = new Ray(startPoint.position, startPoint.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 1f, _hittables))
            {
                Vector3 tankForward = startPoint.forward;
                Vector3 wallNormal = hit.normal;
                _frictionAngle = Vector3.Angle(tankForward, wallNormal);
                Vector3 cross = Vector3.Cross(tankForward, wallNormal);

                // determine if we need to take the clockwise angle
                if (Vector3.Dot(cross, Vector3.up) < 0)
                {
                    _frictionAngle = 360 - _frictionAngle;
                }

                _frictionAngle = _frictionAngle % 360 - 180;

                float t = Mathf.InverseLerp(-45f, 45f, _frictionAngle);
                _sparklesParticles.transform.position = Vector3.Lerp(_leftPoint.position, _rightPoint.position, t);
                
                return true;
            }

            return false;
        }


        private void OnTriggerExit(Collider other)
        {
            _wallsCount--;

            if (_wallsCount <= 0)
            {
                _sparklesParticles.Stop();
            }
        }

        public void EnableBump()
        {
            IsActive = true;
            gameObject.layer = BUMPERLAYER;
        }

        public void DisableBump()
        {
            gameObject.layer = EMPTY_LAYER;
            IsActive = false;
            _sparklesParticles.Stop();
        }

        public float GetBumperMultiplier()
        {
            if(_wallsCount < 1)
            {
                return 1;
            }

            float t = Mathf.InverseLerp(0f, 45f, Mathf.Abs(_frictionAngle));
            float multiplier = Mathf.Lerp(0f, 1f, t) * BUMPER_SPEED_REDUCTION_AMOUNT;
            return multiplier;
        }

        public void ResetWallsCount()
        {
            _wallsCount = 0;
        }
    }
}
