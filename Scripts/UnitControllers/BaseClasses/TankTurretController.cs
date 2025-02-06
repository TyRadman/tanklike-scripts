using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankTurretController : MonoBehaviour, IController
    {
        [Header("Turret")]
        [SerializeField] protected float _rotationSpeed = 50f;
        [SerializeField] private float _minimumThreshold = 0.8f;

        public bool IsActive { get; protected set; }
        
        protected Transform _turret;
        protected Transform Body;
        protected bool _canRotate = true;

        protected const float ROTATION_CORRECTION_THRESHOLD = 0.1f;

        public void SetUp(IController controller)
        {
            if (controller is not TankComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            TankBodyParts parts = components.TankBodyParts;

            _turret = parts.GetBodyPartOfType(BodyPartType.Turret).transform;
        }

        public virtual void HandleTurretRotation(Transform crosshair)
        {
            if (!IsActive)
            {
                Debug.LogError($"IController {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            Vector3 direction = (crosshair.position - _turret.position).normalized;
            Vector3 tankForward = _turret.forward.normalized;
            Vector3 crossDot = Vector3.Cross(tankForward, direction);
            float rotationAmount;
            float cross = crossDot.y;

            // TODO: make this a utility for every turret rotator
            Vector2 dotProductAccuracy = new Vector2(_minimumThreshold, ROTATION_CORRECTION_THRESHOLD);
            Vector2 frameRateRange = new Vector2Int(30, 60);
            int fp = (int)(1f / Time.deltaTime);

            float t = Mathf.InverseLerp(frameRateRange.x, frameRateRange.y, fp);
            float dotProductThreshold = 1 - dotProductAccuracy.Lerp(t);

            if (cross > dotProductThreshold)
            {
                rotationAmount = 1f;
            }
            else if (cross < -dotProductThreshold)
            {
                rotationAmount = -1f;
            }
            else
            {
                if (cross > 0)
                {
                    rotationAmount = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(dotProductThreshold, 0f, cross));
                }
                else if (cross < 0)
                {
                    rotationAmount = Mathf.Lerp(-1f, 0f, Mathf.InverseLerp(-dotProductThreshold, 0f, cross));
                }
                else
                {
                    rotationAmount = 0f;
                }
            }

            _turret.Rotate(_rotationSpeed * rotationAmount * Time.deltaTime * Vector3.up);
        }

        public virtual void HandleTurretRotation(Vector3 crosshair)
        {
            if (!IsActive)
            {
                Debug.LogError($"IController {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            Vector3 direction = (crosshair - _turret.position).normalized;
            Vector3 tankForward = _turret.forward.normalized;
            Vector3 crossDot = Vector3.Cross(tankForward, direction);
            float rotationAmount;
            float cross = crossDot.y;

            // TODO: make this a utility for every turret rotator
            Vector2 dotProductAccuracy = new Vector2(_minimumThreshold, ROTATION_CORRECTION_THRESHOLD);
            Vector2 frameRateRange = new Vector2Int(30, 60);
            int fp = (int)(1f / Time.deltaTime);

            float t = Mathf.InverseLerp(frameRateRange.x, frameRateRange.y, fp);
            float dotProductThreshold = 1 - dotProductAccuracy.Lerp(t);

            if (cross > dotProductThreshold)
            {
                rotationAmount = 1f;
            }
            else if (cross < -dotProductThreshold)
            {
                rotationAmount = -1f;
            }
            else
            {
                if (cross > 0)
                {
                    rotationAmount = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(dotProductThreshold, 0f, cross));
                }
                else if (cross < 0)
                {
                    rotationAmount = Mathf.Lerp(-1f, 0f, Mathf.InverseLerp(-dotProductThreshold, 0f, cross));
                }
                else
                {
                    rotationAmount = 0f;
                }
            }

            _turret.Rotate(_rotationSpeed * rotationAmount * Time.deltaTime * Vector3.up);
        }

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {

        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
