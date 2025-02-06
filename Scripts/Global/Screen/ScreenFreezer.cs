using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ScreenFreeze
{
    public class ScreenFreezer : MonoBehaviour, IManager
    {
        [SerializeField] private float _freezingScale = 0.1f;
        private bool _isFreezing = false;
        private ScreenFreezeData _currentData;
        private float _timer = 0f;

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;

            _isFreezing = false;
            _currentData = null;

            StopAllCoroutines();
        }
        #endregion

        public void FreezeScreen(ScreenFreezeData data)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            if (data == null)
            {
                //Debug.Log("No freezing data");
                return;
            }

            _isFreezing = true;
            _currentData = data;
            StartCoroutine(ReleaseScreen());
        }

        private IEnumerator ReleaseScreen(float time = 0)
        {
            _timer = time;
            AnimationCurve curve = _currentData.Curve;

            while(_timer < _currentData.Duration)
            {
                _timer += Time.unscaledDeltaTime;

                Time.timeScale = Mathf.Lerp(_freezingScale, 1f, 1 - curve.Evaluate(_timer / _currentData.Duration));
                yield return null;
            }

            _isFreezing = false;
        }

        public void PauseFreeze()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            if (!_isFreezing)
            {
                return;
            }

            StopAllCoroutines();
        }

        public void ResumeFreeze()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            if (!_isFreezing)
            {
                return;
            }

            StartCoroutine(ReleaseScreen(_timer));
        }
    }
}
