using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TankLike.Minimap;
using TankLike.UnitControllers;
using TankLike.Cam;

namespace TankLike.Cam
{
    public class CameraManager : MonoBehaviour, IManager
    {
        [Header("References")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private CinemachineVirtualCamera _mainVirtualCamera;

        [field: SerializeField] public GameObject CameraObject { get; private set; }
        [field: SerializeField] public CameraShake Shake { get; private set; }
        [field: SerializeField] public CameraZoom Zoom { get; private set; }
        [field: SerializeField] public MainCameraFollow PlayerCameraFollow { get; private set; }
        
        [SerializeField] private CameraFollow _minimapCameraFollow;

        public CameraFollow MinimapCameraFollow => _minimapCameraFollow;
        public bool IsActive { get; private set; }

        public void SetReferences()
        {
            Shake.SetReferences(_mainVirtualCamera);
            Zoom.SetReferences(_mainVirtualCamera, this);
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            Shake.SetUp();
            Zoom.SetUp();

            PlayerCameraFollow.SetUp();
            _minimapCameraFollow.SetUp();

            SetCamerasLimits(GameManager.Instance.RoomsManager.CurrentRoom.CameraLimits);
        }

        public void Dispose()
        {
            IsActive = false;

            Shake.Dispose();
            Zoom.Dispose();

            PlayerCameraFollow.Dispose();
        }
        #endregion

        public void SetCamerasLimits(CameraLimits limits)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            PlayerCameraFollow.SetLimits(limits);
            _minimapCameraFollow.SetLimits(limits);
        }

        // Used for test scenes
        public void ResetCameraLimit()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            PlayerCameraFollow.ResetLimits();
            //_minimapCameraFollow.SetCurrentLimits(limits);
        }

        public void EnableCamerasInterpolation(bool enable)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            PlayerCameraFollow.EnableInterpolation(enable);
            _minimapCameraFollow.EnableInterpolation(enable);
        }

        public Transform GetMainCamera()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return null;
            }

            return _mainCamera.transform;
        }
    }
}