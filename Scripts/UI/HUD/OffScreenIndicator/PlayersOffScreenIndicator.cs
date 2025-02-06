using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.HUD
{
    /// <summary>
    /// Manages the off screen indicators for the players. It keeps track of the players' screen positions and updates the icons accordingly.
    /// </summary>
    public class PlayersOffScreenIndicator : MonoBehaviour, IManager
    {
        [SerializeField] private List<OffScreenIndicatorProfile> _players;
        [SerializeField] private List<OffScreenIcon> _indicatorIcons;
        [SerializeField] private Transform _indicatorsParent;
        [SerializeField] private float _offset = 10f;
        [SerializeField] private float _lerpSpeed = 5f;

        public bool IsActive { get; private set; }

        private bool _isActive = false; // Not a good name anymore when implementing the IManager
        private Camera _camera;
        //private List<OffScreenIndicatorProfile> _targets = new List<OffScreenIndicatorProfile>();
        private List<OffScreenIndicatorTarget> _targets = new List<OffScreenIndicatorTarget>();
        private HashSet<OffScreenIndicatorTarget> _targetsQueue = new HashSet<OffScreenIndicatorTarget>();

        public void SetReferences()
        {
            _camera = Camera.main;
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            AddQueuedTargets();
        }

        private void AddQueuedTargets()
        {
            foreach (OffScreenIndicatorTarget target in _targetsQueue)
            {
                AddTarget(target);
            }

            _targetsQueue.Clear();
        }

        public void Dispose()
        {
            IsActive = false;

            for (int i = 0; i < _targets.Count; i++)
            {
                _targets[i].Dispose();
            }

            _targets.Clear();
        }
        #endregion

        public void AddTarget(OffScreenIndicatorProfile profile)
        {
            //if (!IsActive)
            //{
            //    Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
            //    return;
            //}

            //_targets.Add(profile);

            //if(profile.Icon == null)
            //{
            //    profile.Icon = Instantiate(profile.IconPrefab, _indicatorsParent);
            //}
        }

        /// <summary>
        /// Adds a target to the off screen indicators manager.
        /// </summary>
        /// <param name="profile"></param>
        public void AddTarget(OffScreenIndicatorTarget target)
        {
            if (!IsActive)
            {
                _targetsQueue.Add(target);
                //Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            target.Initialize();
            _targets.Add(target);

            //Vector3 screenPosition = _camera.WorldToScreenPoint(target.TargetTransform.position);

            //if (screenPosition.z > 0 && !IsInCameraView(screenPosition))
            //{
            //    target.IsShown = true;
            //}
        }
        public void RemoveTarget(OffScreenIndicatorProfile profile)
        {
            //if (!IsActive)
            //{
            //    Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
            //    return;
            //}

            //_targets.Remove(profile);
        }

        /// <summary>
        /// Removes a target from the off screen indicators manager.
        /// </summary>
        /// <param name="profile"></param>
        public void RemoveTarget(OffScreenIndicatorTarget target)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            if(!IsVisible(target.TargetTransform.position))
            {
                target.Hide();
            }
            else
            {
                target.IsShown = false;
            }

            _targets.Remove(target);
        }

        // TODO: run it manually if there are two players. Remove the update function
        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            for (int i = 0; i < _targets.Count; i++)
            {
                DetectTarget(_targets[i]);
            }
        }

        /// <summary>
        /// Detects the target's screen position and updates the icon accordingly.
        /// </summary>
        /// <param name="targetProfile"></param>
        private void DetectTarget(OffScreenIndicatorTarget targetProfile)
        {
            // get the target's screen position
            Vector3 screenPosition = _camera.WorldToScreenPoint(targetProfile.TargetTransform.position);

            if (screenPosition.z > 0 && !IsInCameraView(screenPosition))
            {
                Transform icon = targetProfile.Icon.transform;

                if (!targetProfile.IsShown)
                {
                    targetProfile.Show();

                    icon.position = ClampToScreen(screenPosition);

                    return;
                }

                icon.position = Vector3.Lerp(icon.position, ClampToScreen(screenPosition), _lerpSpeed * Time.deltaTime);

                // rotation
                icon.eulerAngles = Vector3.forward * GetAngle(icon.position);

                // reset the rotation of the icon
                targetProfile.Icon.ResetIconRotation();
            }
            else
            {
                if (targetProfile.IsShown)
                {
                    targetProfile.Hide();
                }
            }
        }

        private bool IsVisible(Vector3 position)
        {
            Vector3 screenPosition = _camera.WorldToScreenPoint(position);

            return IsInCameraView(screenPosition);
        }

        /// <summary>
        /// Checks if a position is within the camera view.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsInCameraView(Vector3 position)
        {
            return position.x > 0 && position.x < Screen.width &&
                position.y > 0 && position.y < Screen.height;
        }

        /// <summary>
        /// Clamps the position to the screen.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector3 ClampToScreen(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, _offset, Screen.width - _offset);
            position.y = Mathf.Clamp(position.y, _offset, Screen.height - _offset);
            return position;
        }

        /// <summary>
        /// Gets the angle of the target's screen position.
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        private float GetAngle(Vector3 screenPosition)
        {
            Vector2 direction = (screenPosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return angle - 90; 
        }

        /// <summary>
        /// Enables or disables the off screen indicator for a player.
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <param name="enableOffScreenIndicator"></param>
        public void EnableOffScreenIndicatorForPlayer(int playerIndex, bool enableOffScreenIndicator)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            if (_players.Count == 0)
            {
                return;
            }

            _players[playerIndex].FollowTarget = enableOffScreenIndicator;

            if(!enableOffScreenIndicator && _players[playerIndex].IsShown)
            {
                _players[playerIndex].Icon.HideIcon();
            }
        }

        public void Enable(bool enable)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _isActive = enable;

            if(!_isActive)
            {
                ForceHideIcons();
            }
        }

        private void ForceHideIcons()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                OffScreenIndicatorProfile player = _players[i];

                if(player.IsShown)
                {
                    player.IsShown = false;
                    player.Icon.HideIcon(2f);
                }
            }
        }
    }
}
