using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;

    public class PlayerPredictedPosition : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }
        [SerializeField] private Transform _image;
        private Transform _playerTransform;
        private PlayerComponents _playerComponents;
        private PlayerMovement _movement;
        [SerializeField] private float _distance = 3f;
        [SerializeField] private float _interpolationSpeed = 0.1f;

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            UpdatePosition();
        }

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _playerTransform = _playerComponents.transform;
            _movement = _playerComponents.Movement as PlayerMovement;
        }

        private void UpdatePosition()
        {
            float multiplier = _playerComponents.Movement.CurrentSpeed;
            Vector3 direction = _movement.LastMovementInput;
            Vector3 newPosition = _playerTransform.position + direction * _distance * multiplier;
            newPosition = Vector3.Lerp(_image.position, newPosition, _interpolationSpeed * Time.deltaTime);
            newPosition.y = _playerTransform.position.y + 0.5f;
            _image.position = newPosition;
        }

        public Vector3 GetPositionAtDistance(float distance)
        {
            float multiplier = _playerComponents.Movement.CurrentSpeed;
            Vector3 direction = _movement.LastMovementInput;
            Vector3 newPosition = _playerTransform.position + direction * distance * multiplier;
            newPosition.y = _playerTransform.position.y + 0.5f;
            return newPosition;
        }

        public Transform GetImage()
        {
            return _image;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            _image.parent = null;
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
