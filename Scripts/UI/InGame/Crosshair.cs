using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.InGame
{
    public class Crosshair : MonoBehaviour
    {
        [field: SerializeField] public bool IsActive { set; get; }
        [field: SerializeField] public CrossHairVisuals Visuals { get; private set; }
        [SerializeField] private Transform _cursorChild;
        private Transform _camera;
        private bool _isColliding = false;

        public void SetUp()
        {
            _camera = Camera.main.transform;
        }

        public void ResetValues()
        {
            //gameObject.SetActive(false);
            transform.parent = null;

            Visuals.SetUp();
        }

        public void Enable()
        {
            StartCoroutine(LookAtCameraProcess());
            Visuals.ShowCrossHair();
        }

        public void Disable()
        {
            Visuals.HideCrossHair();
            StopAllCoroutines();
        }

        private IEnumerator LookAtCameraProcess()
        {
            while (true)
            {
                // Get the forward direction of the camera
                Vector3 cameraForward = _camera.transform.forward;

                // Rotate the object to face the camera's forward direction
                _cursorChild.rotation = Quaternion.LookRotation(-cameraForward, Vector3.up);
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            _isColliding = true;
        }

        private void OnTriggerExit(Collider other)
        {
            _isColliding = false;
        }

        public void SetColor(Color color)
        {
            Visuals.SetColor(color, Color.red);
        }
    }
}
