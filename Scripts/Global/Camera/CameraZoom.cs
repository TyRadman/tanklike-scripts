using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TankLike.Cam
{
    using Utils;

    public class CameraZoom : MonoBehaviour, IManager
    {
        public bool IsActive { get; private set; }
        
        [SerializeField] private float _normalZoomValue = 18.7f;
        [SerializeField] private float _fightZoomValue = 25f;
        [SerializeField] private float _focusZoomValue = 15f;
        [SerializeField] private float _zoomDuration = 1f;
        [SerializeField] private AnimationCurve _zoomCurve;

        private MainCameraFollow _cameraFollow;
        private CinemachineFramingTransposer _transposer;
        private Coroutine _zoomRoutine;
        [field: SerializeField] public float ZoomAmount { get; private set; }

        private const float TUTORIAL_BOOST_ZOOM = 35f;

        public void SetReferences(CinemachineVirtualCamera camera, CameraManager cameraManager)
        {
            _transposer = camera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _cameraFollow = cameraManager.PlayerCameraFollow;
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _transposer.m_CameraDistance = _normalZoomValue;

            ZoomAmount = _normalZoomValue;
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        [ContextMenu("Zoom out")]
        public void SetToFightZoom()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _cameraFollow.SetOffsetMultiplier(_fightZoomValue / _normalZoomValue);
            this.StopCoroutineSafe(_zoomRoutine);
            _zoomRoutine = StartCoroutine(ZoomProcess(_fightZoomValue));

        }

        public void SetToTutorialBoostZoom()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _cameraFollow.SetOffsetMultiplier(TUTORIAL_BOOST_ZOOM / _normalZoomValue);
            this.StopCoroutineSafe(_zoomRoutine);
            _zoomRoutine = StartCoroutine(ZoomProcess(TUTORIAL_BOOST_ZOOM));

        }

        public void SetToNormalZoom()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _cameraFollow.SetOffsetMultiplier(1f);
            this.StopCoroutineSafe(_zoomRoutine);
            _zoomRoutine = StartCoroutine(ZoomProcess(_normalZoomValue));
        }

        public void PerformFocusZoom()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _cameraFollow.SetOffsetMultiplier(_focusZoomValue / _normalZoomValue);
            this.StopCoroutineSafe(_zoomRoutine);
            _zoomRoutine = StartCoroutine(ZoomProcess(_focusZoomValue));
        }

        // original: 8
        // bossZoom: 4
        // bossFight:16
        public void SetToZoomValue(float zoomValue)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _cameraFollow.SetOffsetMultiplier(zoomValue / _normalZoomValue);
            this.StopCoroutineSafe(_zoomRoutine);
            _zoomRoutine = StartCoroutine(ZoomProcess(zoomValue));
        }

        private void CacheLastZoom(float value)
        {
            ZoomAmount = value;
        }

        private IEnumerator ZoomProcess(float newZoomValue)
        {
            CacheLastZoom(newZoomValue);
            float time = 0f;
            float startZoom = _transposer.m_CameraDistance;
            Color randomColor = Random.ColorHSV();
            randomColor.a = 1f;
            
            while (time < _zoomDuration)
            {
                time += Time.deltaTime;
                float t = _zoomCurve.Evaluate(time / _zoomDuration);
                _transposer.m_CameraDistance = Mathf.Lerp(startZoom, newZoomValue, t);
                yield return null;
            }
        }
    }
}
