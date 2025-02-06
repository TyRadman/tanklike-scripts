using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using TankLike.Utils;

    public class TankWiggler : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }
     
        [Header("Body References")]
        [SerializeField] private Transform _tankHolder;
        [SerializeField] private Transform _bodyRotationReference;

        [SerializeField] private bool _debug = false;

        private bool _isWiggling = false;
        private Quaternion _defaultRotation;


        private const float SWITCH_BETWEEN_WIGGLES_TIME = 0.1f;

        public void SetUp(IController controller)
        {
            _defaultRotation = _tankHolder.rotation;
        }

        public void WiggleBody(Wiggle wiggle)
        {
            if (!_isWiggling)
            {
                StartCoroutine(WigglingProcess(wiggle, _tankHolder, _bodyRotationReference));
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(SwitchWigglesProcess(wiggle, _tankHolder, _bodyRotationReference));
            }
        }

        private IEnumerator WigglingProcess(Wiggle wiggle, Transform bodyToWiggle, Transform referenceBody)
        {
            _isWiggling = true;
            float time = 0f;
            //Quaternion initialRotation = bodyToWiggle.rotation;
            Vector3 initialRotation = bodyToWiggle.eulerAngles;
            AnimationCurve curve = wiggle.Curve;
            float direction = wiggle.Backward ? -1 : 1;
            float duration = wiggle.Duration;
            float angle = wiggle.MaxAngle;

            Vector3 directionVector = Vector3.right;

            while (time < duration)
            {
                time += Time.deltaTime;
                float curveValue = curve.Evaluate(time / duration) * direction;
                float rotationAngle = curveValue * angle;

                Vector3 rotationDelta = Quaternion.AngleAxis(rotationAngle, referenceBody.right).eulerAngles;
                Vector3 finalRotation = initialRotation + rotationDelta;
                bodyToWiggle.eulerAngles = finalRotation;

                yield return null;
            }

            _isWiggling = false;
        }

        private IEnumerator SwitchWigglesProcess(Wiggle wiggle, Transform bodyToWiggle, Transform referenceBody)
        {
            float time = 0f;

            while (time < SWITCH_BETWEEN_WIGGLES_TIME)
            {
                time += Time.deltaTime;
                float t = Mathf.Min(time / SWITCH_BETWEEN_WIGGLES_TIME, 1f);
                bodyToWiggle.rotation = Quaternion.Lerp(bodyToWiggle.rotation, _defaultRotation, t);
                yield return null;
            }

            StartCoroutine(WigglingProcess(wiggle, bodyToWiggle, referenceBody));
        }

        private void ResetRotations()
        {
            _tankHolder.rotation = _defaultRotation;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
            ResetRotations();
        }

        public void Restart()
        {

        }

        public void Dispose()
        {
            ResetRotations();
        }
        #endregion
    }
}
