using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cam
{
    using UnitControllers;

    [System.Serializable]
    public class CameraTarget
    {
        public enum FollowMode
        {
            CrossHair = 0,
            Aim = 1,
            Player = 2,
        }

        private FollowMode _mode = FollowMode.CrossHair;
        private FollowMode _lastMode = FollowMode.CrossHair;
        public float SpeedMultiplier = 1f;
        public Transform CrossHair;
        public Transform Target;
        public int Index = -1;
        public bool IsActive { get; private set; } = false;
        private Transform _player;
        private const float AIM_MODE_TARGET_POSITION = 0.5f;

        public CameraTarget(PlayerComponents player)
        {
            Index = player.PlayerIndex;
            Target = new GameObject($"Player{Index}CameraTarget").transform;
            IsActive = true;
            CrossHair = player.CrosshairController.GetCrosshairTransform();
            Target.position = player.transform.position;
            _player = player.transform;
        }

        public void UpdatePositionWithinLimits(CameraLimits limits)
        {
            Vector3 position; 
            
            switch (_mode)
            {
                case FollowMode.CrossHair:
                    {
                        position = CrossHair.position;
                        break;
                    }
                case FollowMode.Player:
                    {
                        position = _player.position;
                        break;
                    }
                case FollowMode.Aim:
                    {
                        position = Vector3.Lerp(_player.position, CrossHair.position, AIM_MODE_TARGET_POSITION);
                        break;
                    }
                default:
                    {
                        position = Vector3.zero;
                        break;
                    }
            }
            
            Target.position = GetPositionWithinLimits(position, limits);
        }

        public void UpdatePositionWithinInterpolatedLimits(CameraLimits limits, float interpolationSpeed)
        {
            Vector3 position;

            switch (_mode)
            {
                case FollowMode.CrossHair:
                    {
                        position = Vector3.Lerp(Target.position, CrossHair.position, interpolationSpeed);
                        break;
                    }
                case FollowMode.Player:
                    {
                        position = Vector3.Lerp(Target.position, _player.position, interpolationSpeed);
                        break;
                    }
                case FollowMode.Aim:
                    {
                        Vector3 desiredPosition = Vector3.Lerp(Target.position, CrossHair.position, interpolationSpeed);
                        position = Vector3.Lerp(_player.position, CrossHair.position, AIM_MODE_TARGET_POSITION);
                        break;
                    }
                default:
                    {
                        position = Vector3.zero;
                        break;
                    }
            }

            Target.position = GetPositionWithinLimits(position, limits);
        }

        private Vector3 GetPositionWithinLimits(Vector3 position, CameraLimits limits)
        {
            position.x = Mathf.Clamp(position.x, limits.HorizontalLimits.x, limits.HorizontalLimits.y);
            position.z = Mathf.Clamp(position.z, limits.VerticalLimits.x, limits.VerticalLimits.y);
            position.y = 0f;
            return position;
        }

        public void SetToMode(FollowMode mode)
        {
            if(_mode == mode)
            {
                return;
            }

            _lastMode = _mode;
            _mode = mode;
        }

        public void SetToLastMode()
        {
            _mode = _lastMode;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
