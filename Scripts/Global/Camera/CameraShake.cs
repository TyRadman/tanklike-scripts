using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Cam
{
    public class CameraShake : MonoBehaviour, IManager
    {
        [SerializeField] private CameraShakeSettings _defaultShakeSettings;
        [SerializeField] private List<CameraShakeSettings> _cameraShakeSettings;

        public bool IsActive { get; private set; }

        private CinemachineBasicMultiChannelPerlin _mainBasicMultiChannelPerlin;

        public void SetReferences(CinemachineVirtualCamera camera)
        {
            _mainBasicMultiChannelPerlin = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _mainBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            _mainBasicMultiChannelPerlin.m_FrequencyGain = _defaultShakeSettings.Frequency;
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        public void ShakeCamera(CameraShakeType type)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }
            
            CameraShakeSettings setting = _cameraShakeSettings.Find(s => s.Type == type);

            if(setting == null) 
            {
                Debug.LogError($"No shake settings of type {type}");
                return;
            }

            _mainBasicMultiChannelPerlin.m_AmplitudeGain = setting.Intensity;
            StartCoroutine(ShakeCameraRoutine(setting.Intensity, setting.Time, setting.Smooth));
        }

        public void ShakeCamera(CameraShakeSettings shake)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            if (shake == null)
            {
                Debug.LogError($"No Shake passed");
                return;
            }

            _mainBasicMultiChannelPerlin.m_AmplitudeGain = shake.Intensity;
            StartCoroutine(ShakeCameraRoutine(shake.Intensity, shake.Time, shake.Smooth));
        }

        private IEnumerator ShakeCameraRoutine(float startingIntensity, float time, bool smooth)
        {
            float timer = 0f;

            while (timer < time)
            {
                if (smooth)
                    _mainBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, timer / time);

                timer += Time.deltaTime;
                yield return null;
            }

            _mainBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        }
    }
}
