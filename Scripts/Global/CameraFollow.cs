using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;
using Cinemachine;

namespace TankLike.Cam
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] [Range(0f, 100f)] private float _snapSpeed = 0.1f;
        [SerializeField] private float _height;
        [SerializeField] private CameraLimits _limits;
        [SerializeField] private CameraLimits _offset;
        [SerializeField] private bool _interpolatePosition = true;
        private Transform _crosshair;
        private List<Transform> _followPoints = new List<Transform>();

        public class FollowTarget
        {
            public Transform CrossHair;
            public Transform Target;
        }

        public void SetUp()
        {
            if (!this.enabled) return;

            _crosshair = GameManager.Instance.PlayersManager.GetPlayer(0).CrosshairController.GetCrosshairTransform();
            _target.position = GameManager.Instance.PlayersManager.GetPlayer(0).transform.position;
        }

        private void Update()
        {
            if (_crosshair == null)
            {
                return;
            }

            OnCursorFollow();
        }

        public void OnCursorFollow()
        {
            Vector3 newPosition;

            if (_interpolatePosition)
            {
                newPosition = Vector3.Lerp(_target.position, _crosshair.position, _snapSpeed * Time.deltaTime);
            }
            else
            {
                newPosition = _crosshair.position;
            }

            newPosition.x = Mathf.Clamp(newPosition.x, _limits.HorizontalLimits.x, _limits.HorizontalLimits.y);
            newPosition.z = Mathf.Clamp(newPosition.z, _limits.VerticalLimits.x, _limits.VerticalLimits.y);
            newPosition.y = _height;

            _target.position = newPosition;
        }

        public void SetLimits(CameraLimits limits)
        {
            _limits.SetValuesWithOffset(limits, _offset, 1f);
        }

        // Used for test scenes
        public void ResetLimits(CameraLimits limits)
        {
            _limits.SetValues(limits);
        }

        public void EnableInterpolation(bool enable)
        {
            _interpolatePosition = enable;
        }
    }
}
