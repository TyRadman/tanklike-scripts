using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using UnitControllers;
    using Cam;
    using Utils;
    using Environment;

    public class MainCameraFollow : MonoBehaviour, IManager
    {
        public bool IsActive { get; private set; }

        [SerializeField] private Transform _target;
        [SerializeField] private CameraLimits _originalLimits;
        [SerializeField] private CameraLimits _currentLimits;
        [SerializeField] private CameraLimits _offset;
        [SerializeField] private AnimationCurve _moveToTargetCurve;
        [SerializeField][Range(0f, 20f)] private float _mainTargetFollowSpeed = 4f;
        [SerializeField] private float _totalSpeedMultiplier = 1f;

        private List<CameraTarget> _followPoints = new List<CameraTarget>();
        private bool _interpolatePosition = true;
        private int _playersCount;
        private bool _followPlayers = false;

        private const float SPEED_TRANSITION_DURATION = 1f;

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            if (!this.enabled)
            {
                return;
            }

            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                AddCameraFollowTarget(i);
            }

            // set the initial position of the camera
            Vector3 startPosition = Vector3.zero;
            _followPoints.ForEach(f => startPosition += f.Target.position);
            _target.position = startPosition / _followPoints.Count;
            _followPlayers = true;


            CameraLimits limits = new CameraLimits();
            limits.SetValues(GameManager.Instance.RoomsManager.CurrentRoom.CameraLimits);
            limits.AddOffset(GameManager.Instance.RoomsManager.CurrentRoom.transform.position);
            GameManager.Instance.CameraManager.SetCamerasLimits(limits);
        }

        public void Dispose()
        {
            IsActive = false;

            StopAllCoroutines();

            _followPoints.Clear();
            _playersCount = 0;
            _followPlayers = false;
        }
        #endregion

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            _followPoints.ForEach(f => UpdateFollowPointPosition(f));

            if (_playersCount < 1 || !_followPlayers)
            {
                return;
            }

            FollowMainTarget();
        }

        private void AddCameraFollowTarget(int playerIndex)
        {
            if (_followPoints.Exists(f => f.Index == playerIndex))
            {
                return;
            }

            PlayerComponents player = GameManager.Instance.PlayersManager.GetPlayer(playerIndex);
            CameraTarget newFollow = new CameraTarget(player);

            _followPoints.Add(newFollow);
            _playersCount = _followPoints.Count;
        }

        public void SetPlayerTargetToAimMode(int index)
        {
            _followPoints.Find(p => p.Index == index).SetToMode(CameraTarget.FollowMode.Aim);
        }

        public void SetPlayerTargetToCrosshairMode(int index)
        {
            if (_followPoints != null && _followPoints.Count > 0)
            {
                _followPoints.Find(p => p.Index == index).SetToMode(CameraTarget.FollowMode.CrossHair);
            }
        }

        public void SetPlayerTargetToPlayerMode(int index)
        {
            _followPoints.Find(p => p.Index == index).SetToMode(CameraTarget.FollowMode.Player);
        }

        public void SetPlayerTargetToLastMode(int index)
        {
            _followPoints.Find(p => p.Index == index).SetToLastMode();
        }

        /// <summary>
        /// Updates the target points of each camera target. The average of those points will then be the main camera target as cameras can only have one.
        /// </summary>
        /// <param name="target"></param>
        private void UpdateFollowPointPosition(CameraTarget target)
        {
            if (!target.IsActive)
            {
                return;
            }

            if (_interpolatePosition)
            {
                float lerpSpeed = _mainTargetFollowSpeed;
                _followPoints.ForEach(p => lerpSpeed *= p.SpeedMultiplier);
                lerpSpeed *= Time.deltaTime / _playersCount;

                target.UpdatePositionWithinInterpolatedLimits(_currentLimits, lerpSpeed);
            }
            else
            {
                target.UpdatePositionWithinLimits(_currentLimits);
            }
        }

        /// <summary>
        /// Calculates the position of the camera's target using the follow targets.
        /// </summary>
        private void FollowMainTarget()
        {
            List<CameraTarget> activeFollowees = _followPoints.FindAll(f => f.IsActive);
            Vector3 position = Vector3.zero;
            activeFollowees.ForEach(f => position += f.Target.position);
            position /= _playersCount;

            if (_interpolatePosition)
            {
                float lerpSpeed = _mainTargetFollowSpeed;
                activeFollowees.ForEach(p => lerpSpeed *= p.SpeedMultiplier);
                lerpSpeed *= Time.deltaTime;
                position = Vector3.Lerp(_target.position, position, lerpSpeed);
            }

            _target.position = position;
        }

        public void SetSpeedMultiplier(float multiplier, int targetIndex)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            StopAllCoroutines();
            StartCoroutine(ChangeSpeedMultiplierValueProcess(1f + multiplier * _totalSpeedMultiplier, _followPoints[targetIndex]));
        }

        public void ResetSpeedMultiplier(int targetIndex)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            StopAllCoroutines();
            StartCoroutine(ChangeSpeedMultiplierValueProcess(1f, _followPoints[targetIndex]));
        }

        private IEnumerator ChangeSpeedMultiplierValueProcess(float endValue, CameraTarget target)
        {
            float timer = 0f;
            float startValue = target.SpeedMultiplier;

            while (timer < SPEED_TRANSITION_DURATION)
            {
                timer += Time.deltaTime;
                target.SpeedMultiplier = Mathf.Lerp(startValue, endValue, timer / SPEED_TRANSITION_DURATION);
                yield return null;
            }
        }

        public void SetLimits(CameraLimits limits)
        {
            // TODO: Uncomment this after figuring out the setup order
            //if (!IsActive)
            //{
            //    Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
            //    return;
            //}

            _originalLimits.SetValues(limits);
            _currentLimits.SetValuesWithOffset(_originalLimits, _offset, 1f);
        }

        // Used for test scenes
        public void ResetLimits()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _currentLimits.SetValues(_originalLimits);
        }

        public void EnableInterpolation(bool enable)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _interpolatePosition = enable;
        }

        /// <summary>
        /// Removes the player with the provided index from the list of players getting followed by the camera.
        /// </summary>
        /// <param name="playerIndex"></param>
        public void RemoveCameraFollow(int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _followPoints.Find(f => f.Index == playerIndex).SetActive(false);
            _playersCount = _followPoints.FindAll(f => f.IsActive).Count;
        }

        /// <summary>
        /// Adds the player with the provided index from the list of players getting followed by the camera.
        /// </summary>
        /// <param name="playerIndex"></param>
        public void AddCameraFollower(int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            //_followPoints.Add(_backUpFollowPoints.Find(f => f.Index == playerIndex));
            _followPoints.Find(f => f.Index == playerIndex).SetActive(true);
            _playersCount = _followPoints.FindAll(f => f.IsActive).Count;
        }

        public void SetOffsetMultiplier(float value)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _currentLimits.SetValuesWithOffset(_originalLimits, _offset, value);
        }

        public void MoveToPoint(Transform point, float duration)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            StartCoroutine(MoveToPointProcess(point, duration));
        }

        private IEnumerator MoveToPointProcess(Transform point, float duration)
        {
            float timeElapsed = 0f;
            // stop the players following process
            _followPlayers = false;
            // cache the start point
            Vector3 startPoint = _target.position;

            startPoint.x = Mathf.Clamp(startPoint.x, _currentLimits.HorizontalLimits.x, _currentLimits.HorizontalLimits.y);
            startPoint.z = Mathf.Clamp(startPoint.z, _currentLimits.VerticalLimits.x, _currentLimits.VerticalLimits.y);

            while (timeElapsed <= duration)
            {
                timeElapsed += Time.deltaTime;
                float t = _moveToTargetCurve.Evaluate(timeElapsed / duration);

                Vector3 pointPosition = point.position;
                pointPosition.x = Mathf.Clamp(pointPosition.x, _currentLimits.HorizontalLimits.x, _currentLimits.HorizontalLimits.y);
                pointPosition.z = Mathf.Clamp(pointPosition.z, _currentLimits.VerticalLimits.x, _currentLimits.VerticalLimits.y);
                point.position = pointPosition;

                Vector3 newPosition = Vector3.Lerp(startPoint, pointPosition, t);

                _target.position = newPosition;
                yield return null;
            }
        }

        /// <summary>
        /// So far, this is only used for the boss cinematic.
        /// </summary>
        /// <param name="duration"></param>
        public void MoveBackToPlayers(float duration)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            StartCoroutine(MoveToPlayersProcess(duration));
        }

        private IEnumerator MoveToPlayersProcess(float duration)
        {
            float timeElapsed = 0f;
            _followPlayers = false;
            Vector3 startPoint = _target.position;
            List<Transform> players = GameManager.Instance.PlayersManager.GetPlayersTransforms();

            while (timeElapsed <= duration)
            {
                timeElapsed += Time.deltaTime;
                float t = _moveToTargetCurve.Evaluate(timeElapsed / duration);

                Vector3 pointPosition = Vector3.zero;

                players.ForEach(p => pointPosition += p.position);
                pointPosition /= 2;

                pointPosition.x = Mathf.Clamp(pointPosition.x, _currentLimits.HorizontalLimits.x, _currentLimits.HorizontalLimits.y);
                pointPosition.z = Mathf.Clamp(pointPosition.z, _currentLimits.VerticalLimits.x, _currentLimits.VerticalLimits.y);

                Vector3 newPosition = Vector3.Lerp(startPoint, pointPosition, t);

                _target.position = newPosition;
                yield return null;
            }
        }

        /// <summary>
        /// So far, this is only used when the boss cinematic ends.
        /// </summary>
        public void EnableFollowingPlayer()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _followPlayers = true;
        }

        /// <summary>
        /// So far, this is only used when the game is over.
        /// </summary>
        public void StopCameraFollowProcess()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _followPlayers = false;
            StopAllCoroutines();
        }

        #region Utilities
        public void PositionTargetAtActiveRoomCenter()
        {
            Room room = GameManager.Instance.RoomsManager.CurrentRoom;

            if (room == null)
            {
                Debug.LogError("No room found to position the camera target at!");
                return;
            }

            Vector3 position = room.Spawner.GetPlayerSpawnPoint(0);
            position.y = 0f;

            _target.position = position;
            #endregion
        }
    }
}
