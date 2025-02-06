using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Utils
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _camera;
        private Transform _object;

        private void Awake()
        {
            _camera = Camera.main.transform;
            _object = transform;
        }

        void Update()
        {
            //_object.LookAt(_camera);

            // Get the forward direction of the camera
            Vector3 cameraForward = _camera.transform.forward;

            // Rotate the object to face the camera's forward direction
            _object.rotation = Quaternion.LookRotation(-cameraForward, Vector3.up);
        }
    }
}
