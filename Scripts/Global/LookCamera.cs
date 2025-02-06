using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class LookCamera : MonoBehaviour
    {
        private Transform _camera;

        private void Start()
        {
            _camera = GameManager.Instance.CameraManager.GetMainCamera();
        }

        private void Update()
        {
            print($"Looking at camera from {gameObject.name}");
            transform.LookAt(_camera);
        }
    }
}
