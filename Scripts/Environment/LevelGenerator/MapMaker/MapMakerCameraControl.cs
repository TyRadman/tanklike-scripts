using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    public class MapMakerCameraControl : MonoBehaviour
    {
        private Camera _cam;
        [SerializeField] private float _speed = 10f;
        [SerializeField] private Transform _followObject;
        private PlayerInputActions _playerinput;
        private Vector2 _movement;
        public bool IsActive = true;
        [SerializeField] private float _movementLerp = 0.1f;
        [Header("Zoom values")]
        [SerializeField] private float _zoomAmount = 30f;
        [SerializeField] private float _zoomSpeed = 20f;
        [SerializeField] private Vector2 _zoomLimits;

        private void Start()
        {
            _cam = Camera.main;
            _playerinput = new PlayerInputActions();
            _playerinput.Player.Enable();
        }

        private void Update()
        {
            if (!IsActive) return;

            Move();
            Zoom();
            _followObject.position = Vector3.Lerp(_followObject.position,
                _followObject.position + new Vector3(_movement.x, 0f, _movement.y) * Time.deltaTime * _speed, _movementLerp);
        }

        private void Move()
        {
            _movement = _playerinput.Player.Movement.ReadValue<Vector2>();
        }

        private void Zoom()
        {
            _zoomAmount -= Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed * Time.deltaTime;
            _zoomAmount = Mathf.Clamp(_zoomAmount, _zoomLimits.x, _zoomLimits.y);
            _cam.orthographicSize = _zoomAmount;
        }
    }
}
