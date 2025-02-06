using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using UI.InGame;
    using Sound;
    using Utils;

    public class PlayerAimAssist : MonoBehaviour, IController, IInput, IConstraintedComponent, IResumable
    {
        public bool IsActive { get; private set; }
        public bool IsAiming { get; private set; } = false;
        public bool IsConstrained { get; set; }

        [SerializeField] private Transform _turret;
        [SerializeField] private LayerMask _layersToDetect;
        [SerializeField] private LayerMask _targetLayer;
        [SerializeField] private float _detectionRange = 20f;

        [Header("Angles")]
        [SerializeField] private float _aimAssistAngle = 45f;
        [SerializeField] private float _focusedAimAssistAngle = 90f;
        [SerializeField] private SpriteRenderer _indicatorSprite;

        [Header("Audio")]
        [SerializeField] private Audio _switchTargetsAudio;

        [Header("Debug")]
        [SerializeField] private bool _debug = false;

        private List<Transform> _currentTargets = new List<Transform>();
        private Transform _currentTarget;
        private Transform _tank;
        private PlayerComponents _playerComponents;
        private PlayerCrosshairController _crosshairController;
        private MainCameraFollow _cameraFollowController;
        private Crosshair _crossHair;
        private Coroutine _aimAssistCoroutine;
        private InputAction _aimAssistInputAction;
        private float _normalAngleRad;
        private float _focusedAngleRad;
        private float _currentAngleRad;
        private bool _isInputDown = false;
        private bool _wasInputDown;

        private const float SWITCH_TARGET_THRESHOLD = 0.2f;

        public void SetUp(IController controller)
        {
            _debug = true;
            PlayerComponents playerComponents = controller as PlayerComponents;
            Helper.CheckForComponentValidity(playerComponents != null, GetType());

            _playerComponents = playerComponents;

            _tank = _playerComponents.transform;
            _crossHair = _playerComponents.CrosshairController.GetCrosshair();
            _crosshairController = _playerComponents.CrosshairController;

            // convert the angle to a rad for when checking the dot product to determine if the target is within the range
            _normalAngleRad = Mathf.Cos((_aimAssistAngle / 2) * Mathf.Deg2Rad);
            _focusedAngleRad = Mathf.Cos((_focusedAimAssistAngle / 2) * Mathf.Deg2Rad);

            _cameraFollowController = GameManager.Instance.CameraManager.PlayerCameraFollow;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap gameplayMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            _aimAssistInputAction = gameplayMap.FindAction(c.Player.Aim.name);

            _aimAssistInputAction.performed += OnAimAssistInputDown;
            _aimAssistInputAction.canceled += OnAimAssistInputUp;

            gameplayMap.FindAction(c.Player.TurretRotation.name).performed += UpdateAimTargets;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap gameplayMap = InputManager.GetMap(playerIndex, ActionMap.Player);

            if(_aimAssistInputAction == null)
            {
                Helper.ThrowInputActionError();
                return;
            }

            _aimAssistInputAction.performed -= OnAimAssistInputDown;
            _aimAssistInputAction.canceled -= OnAimAssistInputUp;

            gameplayMap.FindAction(c.Player.TurretRotation.name).performed -= UpdateAimTargets;
        }
        #endregion

        private void OnAimAssistInputDown(InputAction.CallbackContext _)
        {
            if (!IsActive || IsConstrained)
            {
                return;
            }

            _isInputDown = true;
            this.StopCoroutineSafe(_aimAssistCoroutine);
            _aimAssistCoroutine = StartCoroutine(AimAssistProcess());
        }

        private void OnAimAssistInputUp(InputAction.CallbackContext _)
        {
            StopAiming();
            // reset the flag used to continue the aim input after applying constraints
            _wasInputDown = false;
        }

        private void StopAiming()
        {
            _cameraFollowController.SetPlayerTargetToCrosshairMode(_playerComponents.PlayerIndex);
            _isInputDown = false;
            _crossHair.Visuals.StopAiming();
            _crosshairController.DisableIsAiming();
            _currentTarget = null;

            if (_indicatorSprite.enabled)
            {
                _indicatorSprite.enabled = false;
            }
        }

        private void UpdateAimTargets(InputAction.CallbackContext ctx)
        {
            if (!_isInputDown)
            {
                return;
            }

            Vector2 inputValue = ctx.ReadValue<Vector2>();

            HandleTargetSwitching(inputValue);
        }

        private void HandleTargetSwitching(Vector2 input)
        {
            Vector3 turretForward = _turret.forward;
            Vector2 turretForward2D = new Vector2(turretForward.x, turretForward.z);

            // get the cross product of the tank's forward direction and the input direction
            float crossProduct = turretForward2D.x * input.y - turretForward2D.y * input.x;

            // if the cross value is close to zero, then the tank's forward direction and the input direction are almost aligned
            if (Mathf.Abs(crossProduct) > SWITCH_TARGET_THRESHOLD)
            {
                bool isClockwise = crossProduct > 0;

                Transform closestTarget = FindNextTarget(_turret, isClockwise);

                if (closestTarget != null && closestTarget != _currentTarget)
                {
                    PlayAudio();
                    _currentTarget = closestTarget;
                }
            }
        }

        private Transform FindNextTarget(Transform turret, bool isClockwise)
        {
            Transform bestTarget = null;
            float closestAngle = isClockwise ? Mathf.Infinity : -Mathf.Infinity;

            for (int i = 0; i < _currentTargets.Count; i++)
            {
                Transform target = _currentTargets[i];

                if(target == null)
                {
                    _currentTargets.Remove(target);
                }

                Vector3 directionToTarget = (target.position - turret.position).normalized;

                // get the directions in 2D
                Vector2 direction2D = new Vector2(directionToTarget.x, directionToTarget.z);
                Vector2 turretForward = new Vector2(turret.forward.x, turret.forward.z);

                float angle = Vector2.SignedAngle(turretForward, direction2D);

                if (isClockwise && angle > 0 && angle < closestAngle)
                {
                    closestAngle = angle;
                    bestTarget = target;
                }
                else if (!isClockwise && angle < 0 && angle > closestAngle)
                {
                    closestAngle = angle;
                    bestTarget = target;
                }
            }

            return bestTarget;
        }

        private IEnumerator AimAssistProcess()
        {
            while (_isInputDown)
            {
                CacheTargets();
                PerformOnAimingLogic();
                yield return null;
            }
        }

        private void CacheTargets()
        {
            if (!IsActive || IsConstrained)
            {
                return;
            }

            _currentAngleRad = _currentTarget == null ? _normalAngleRad : _focusedAngleRad;
            _currentTargets.Clear();

            List<Transform> hostileTargets = GetHostileTargets();

            if (!hostileTargets.IsEmpty())
            {
                _currentTargets.AddRange(hostileTargets);

                //List<Transform> explosives = GetExplosives();
                
                //if(!explosives.IsEmpty())
                //{
                //    _currentTargets.AddRange(explosives);
                //}
            }
            //else
            //{
            //    List<Transform> nonHostileTargets = AddedNonHostileTargets();

            //    if (!nonHostileTargets.IsEmpty())
            //    {
            //        _currentTargets = nonHostileTargets;
            //    }
            //    else
            //    {
            //    }
            //}

            if (_currentTargets.Count == 0)
            {
                IsAiming = false;
                return;
            }

            // if there is no target or the current target is not within the player's aim range, then cache the closest target
            if (_currentTarget == null || !_currentTargets.Contains(_currentTarget))
            {
                PlayAudio();
                _currentTarget = _currentTargets.OrderBy(t => (t.position - _tank.position).sqrMagnitude).FirstOrDefault();
            }

            IsAiming = true;
        }

        private List<Transform> AddedNonHostileTargets()
        {
            var targets = GameManager.Instance.RoomsManager.CurrentRoom.Spawnables.GetAimAssistTargets();
            List<Transform> droppers = GetTargetsWithinRange(targets);
            return droppers;
        }

        private List<Transform> GetHostileTargets()
        {
            List<Transform> hostileTargets = GetTargetsWithinRange(GameManager.Instance.EnemiesManager.GetSpawnedEnemies());

            return hostileTargets;
        }

        private List<Transform> GetExplosives()
        {
            List<Transform> hostileTargets = GetTargetsWithinRange(GameManager.Instance.RoomsManager.CurrentRoom.Spawnables.GetExplosives());
            return hostileTargets;
        }

        private void PlayAudio()
        {
            GameManager.Instance.AudioManager.Play(_switchTargetsAudio);
        }

        private List<Transform> GetTargetsWithinRange(List<Transform> targets)
        {
            if (targets.IsEmpty())
            {
                return new List<Transform>();
            }

            List<Transform> targetsWithinRange = new List<Transform>();

            // filter out far enemies based on their distance to the player

            targets = targets.FindAll(e => Vector2.Distance(
                new Vector2(e.position.x, e.position.z),
                new Vector2(_tank.position.x, _tank.position.z)) <= _detectionRange);


            if (targets.Count == 0)
            {
                return new List<Transform>();
            }

            for (int i = 0; i < targets.Count; i++)
            {
                float height = Constants.AimAssistRayHeight;
                Vector3 rayOrigin = _tank.position.Where(y: height);
                
                Vector3 direcitonToTarget = (targets[i].position.Where(y: height) - _tank.position.Where(y: height));
                direcitonToTarget.Normalize();

                Vector3 tankForward = _turret.forward;

                if (Physics.Raycast(rayOrigin, direcitonToTarget, out RaycastHit hit, _detectionRange, _layersToDetect))
                {
                    if ((_targetLayer.value & (1 << hit.collider.gameObject.layer)) == 0)
                    {
                        Debug.DrawRay(rayOrigin, direcitonToTarget * _detectionRange, Color.red);
                        continue;
                    }
                    else
                    {
                        Debug.DrawRay(rayOrigin, direcitonToTarget * _detectionRange, Color.yellow);
                    }
                }

                float dotProduct = Vector3.Dot(tankForward, direcitonToTarget);

                if (dotProduct >= _currentAngleRad)
                {
                    if (_debug)
                    {
                        Debug.DrawRay(rayOrigin, direcitonToTarget * _detectionRange, Color.green);
                    }

                    targetsWithinRange.Add(targets[i]);
                }
            }

            if (targetsWithinRange.Count == 0)
            {
                return null;
            }

            if (_debug)
            {
                Vector3 direction = (_crossHair.transform.position - _tank.position).normalized; // TODO: do we need to use the rayOrigin here too?
                direction.y = 0f;

                float halfAngle = _aimAssistAngle / 2;
                Vector3 leftDirection = Quaternion.Euler(0, -halfAngle, 0) * direction;
                Vector3 rightDirection = Quaternion.Euler(0, halfAngle, 0) * direction;
                Debug.DrawRay(_tank.position, leftDirection * _detectionRange, Color.blue);
                Debug.DrawRay(_tank.position, rightDirection * _detectionRange, Color.blue);
            }

            return targetsWithinRange;
        }

        private void PerformOnAimingLogic()
        {
            if (IsAiming)
            {
                if (_indicatorSprite.enabled)
                {
                    _indicatorSprite.enabled = false;
                }

                _cameraFollowController.SetPlayerTargetToAimMode(_playerComponents.PlayerIndex);

                _crossHair.Visuals.PlayActiveAimAnimation();
                _crosshairController.EnableIsAiming();
                _crosshairController.SetAimingPosition(_currentTarget.position);
            }
            else
            {
                if (!_indicatorSprite.enabled)
                {
                    _indicatorSprite.enabled = true;
                }

                _cameraFollowController.SetPlayerTargetToCrosshairMode(_playerComponents.PlayerIndex);
                _crosshairController.DisableIsAiming();
                _crossHair.Visuals.PlayInactiveAimAnimation();
            }
        }

        public void SaveLastInputState()
        {
            _wasInputDown = _isInputDown;
        }

        public void RecoverLastInput()
        {
            if (!_wasInputDown)
            {
                return;
            }

            _wasInputDown = false;
            _isInputDown = true;
            this.StopCoroutineSafe(_aimAssistCoroutine);
            _aimAssistCoroutine = StartCoroutine(AimAssistProcess());
        }

        #region Constraints
        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canAim = (constraints & AbilityConstraint.AimAssist) == 0;

            if (IsConstrained == !canAim)
            {
                return;
            }

            IsConstrained = !canAim;

            if (IsConstrained)
            {
                IsAiming = false;
                StopAiming();
            }
            else
            {
                ResumeComponent();
            }
        }

        public void ResumeComponent()
        {
            if (_aimAssistInputAction.inProgress)
            {
                OnAimAssistInputDown(new InputAction.CallbackContext());
            }
        }
        #endregion

        #region IController
        public void Activate()
        {
            IsActive = true;
            ResumeComponent();
        }

        public void Deactivate()
        {
            IsActive = false;
            _wasInputDown = false;
        }

        public void Restart()
        {
            SetUpInput(_playerComponents.PlayerIndex);
        }

        public void Dispose()
        {
            StopAiming();
            DisposeInput(_playerComponents.PlayerIndex);
            StopAllCoroutines();
        }
        #endregion
    }
}
