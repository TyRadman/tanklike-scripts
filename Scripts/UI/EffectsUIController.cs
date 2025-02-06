using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI
{
    public class EffectsUIController : MonoBehaviour, IManager
    {
        [SerializeField] private ParticleSystem  _speedLinesParticles;

        [Header("Level Name")]

        [SerializeField] private GameObject _levelNameParent;
        [SerializeField] private float _levelNameDisplayduration = 1f;
        [SerializeField] private Animation _levelNameAnimation;
        [SerializeField] private AnimationClip _showAnimationClip;
        [SerializeField] private AnimationClip _hideAnimationClip;

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _levelNameParent.SetActive(false);
        }

        public void Dispose()
        {
            IsActive = false;

            StopAllCoroutines();
            _levelNameParent.SetActive(false);
        }
        #endregion

        public void PlaySpeedLinesEffect()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _speedLinesParticles.Play();
        }

        public void StopSpeedLinesEffect()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _speedLinesParticles.Stop();
        }

        public void ShowLevelName()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            StartCoroutine(ShowLevelNameProcess());
        }

        private IEnumerator ShowLevelNameProcess()
        {
            _levelNameParent.SetActive(true);

            _levelNameAnimation.clip = _showAnimationClip;
            _levelNameAnimation.Play();

            yield return new WaitForSeconds(_levelNameDisplayduration);

            _levelNameAnimation.clip = _hideAnimationClip;
            _levelNameAnimation.Play();

            yield return new WaitForSeconds(1f);

            _levelNameParent.SetActive(false);
        }
    }
}
