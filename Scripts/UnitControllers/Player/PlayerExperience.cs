using System.Collections;
using System.Collections.Generic;
using TankLike.Misc;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Misc;
    using TankLike.Utils;
    using UI.HUD;

    public class PlayerExperience : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }
        public System.Action OnLevelUp { get; set; }

        [SerializeField] private int[] _maxExperiencePerLevel;
        [Header("Level up")] // TODO: fix this later 
        [SerializeField] private ParticleSystem _levelUpEffect;
        [SerializeField] private AudioSource _levelUpAudio;

        [Header("Animations")]
        [SerializeField] private Animation _levelUpAnimation;
        [SerializeField] private AnimationClip _levelUpIdleAnimationClip;
        [SerializeField] private AnimationClip _levelUpPopUpAnimationClip;

        private PlayerComponents _playerComponents;
        private PlayerHUD _HUD;
        private int _currentExperience;
        private int _currentLevel;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _HUD = GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex];

            // TODO: here or in resetValues?
            _currentLevel = 0;
            _currentExperience = 0;

            _HUD.ResetExperienceBar();
            _HUD.UpdateLevelText(_currentLevel + 1);
        }

        public void AddExperience(int xpPoints)
        {
            if (_currentLevel >= _maxExperiencePerLevel.Length)
            {
                return;
            }

            _currentExperience += xpPoints;

            if (_currentExperience >= _maxExperiencePerLevel[_currentLevel])
            {
                LevelUp();
            }

            UpdateUI();
        }

        private void LevelUp()
        {
            _currentExperience -= _maxExperiencePerLevel[_currentLevel];

            _HUD.ResetExperienceBar();
            OnLevelUp?.Invoke(); //add perk point in the player upgrades class?

            _currentLevel++;
            _currentLevel = Mathf.Clamp(_currentLevel, 0, _maxExperiencePerLevel.Length);
            _HUD.UpdateLevelText(_currentLevel + 1);

            AddLevelUpEffect();
        }

        private void UpdateUI()
        {
            _HUD.UpdateExperienceBar(_currentExperience, _maxExperiencePerLevel[_currentLevel]);
        }

        public void AddLevelUpEffect()
        {
            _levelUpEffect.Play();
            _levelUpAnimation.clip = _levelUpPopUpAnimationClip;
            _levelUpAnimation.Play();
            _levelUpAudio.Play();

            //TODO: fix this later
            //ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Buffs.LevelUp;
            //vfx.transform.parent = transform;
            //vfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));
            //vfx.gameObject.SetActive(true);
            //vfx.Play();
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
            _levelUpAnimation.clip = _levelUpIdleAnimationClip;
            _levelUpAnimation.Play();
        }

        public void Dispose()
        {
            _levelUpAnimation.clip = _levelUpIdleAnimationClip;
            _levelUpAnimation.Play();
        }
        #endregion
    }
}
