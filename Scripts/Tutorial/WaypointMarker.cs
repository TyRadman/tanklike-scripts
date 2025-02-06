using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TankLike.Tutorial
{
    using UI.HUD;
    using UnitControllers;
    using Sound;

    public class WaypointMarker : MonoBehaviour
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _activeClip;
        [SerializeField] private AnimationClip _completedClip;
        [SerializeField] private OffScreenIndicatorProfile _offScreenIndicatorProfile;
        [Header("Events")]
        [SerializeField] private UnityEvent _onTriggered;
        [SerializeField] private AbilityConstraint _constraints;
        public TutorialManager Manager;
        [SerializeField] private List<TutorialAchievement> _achievements = new List<TutorialAchievement>();
        public List<EnemySpawn> EnemiesToSpawn = new List<EnemySpawn>();

        [SerializeField] private UnityEvent _onAchievementsCompleted;
        [SerializeField] private Audio _onTriggerAudio;
        [SerializeField] private Audio _onAchievementCompletedAudio;


        private IEnumerator Start()
        {
            _offScreenIndicatorProfile.SetUp(transform, false);
            yield return new WaitForSeconds(1f);
            GameManager.Instance.HUDController.OffScreenIndicator.AddTarget(_offScreenIndicatorProfile);
        }

        private void CheckAchievements()
        {
            if (_achievements.TrueForAll(a => a.IsAchieved()))
            {
                _achievements.ForEach(a => a.OnAchievementCompleted = null);
                _onAchievementsCompleted?.Invoke();
                RemoveConstraints();
                GameManager.Instance.AudioManager.Play(_onAchievementCompletedAudio);
            }
        }

        public void RemoveConstraints()
        {
            GameManager.Instance.PlayersManager.Constraints.RemoveConstraints(_constraints);
        }

        #region Trigger Enter
        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                OnPlayerEnteredTrigger();
            }
        }

        private void OnPlayerEnteredTrigger()
        {
            if(EnemiesToSpawn.Count > 0)
            {
                Manager.SpawnEnemies(this);
            }

            PlayCompletionAnimation();
            GameManager.Instance.AudioManager.Play(_onTriggerAudio);

            Manager.SetRespawnPoint(transform);

            _onTriggered?.Invoke();

            GameManager.Instance.HUDController.OffScreenIndicator.RemoveTarget(_offScreenIndicatorProfile);

            GameManager.Instance.PlayersManager.Constraints.ApplyConstraints(_constraints);
            Manager.SetCurrentConstraints(_constraints);

            SetUpAchievements();
        }

        private void SetUpAchievements()
        {
            _achievements.ForEach(a => a.SetUp(this));
            _achievements.ForEach(a => a.OnAchievementCompleted += CheckAchievements);
        }

        private void PlayCompletionAnimation()
        {
            _animation.clip = _completedClip;
            _animation.Play();
        }
        #endregion
    }

    [System.Serializable]
    public class EnemySpawn
    {
        public EnemyType Enemy;
        public Transform SpawnPoint;
        public List<DifficultyModifier> Modifiers = new List<DifficultyModifier>();
    }
}
