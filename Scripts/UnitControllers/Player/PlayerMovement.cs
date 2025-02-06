using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Cam;
    using Sound;
    using Utils;

    public class PlayerMovement : TankMovement, IInput
    {
        [field: SerializeField] public bool IsMoving { get; private set; } = false;
        public System.Action<float, Transform> OnMovement;
        public Vector3 LastMovementInput { get; set; }

        private PlayerJump _playerJump;
        private CollisionEventPublisher _bumper;
        private PlayerComponents _playerComponents;
        private PlayerBoost _boost;
        private InputAction _movementInputAction;
        private Vector3 _moveDir;
        private Vector3 _lastMovementInputVector;
        private Vector2 _inputVector;
        private bool _decelerate;
        private bool _isTouchingWall;
        private float _speedMultiplier;

        public override void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;

            base.SetUp(_playerComponents);

            _boost = _playerComponents.PlayerBoost;
            _playerJump = _playerComponents.Jump;
            TankBodyParts parts = _playerComponents.TankBodyParts;

            TankBody tankBody = (TankBody)parts.GetBodyPartOfType(BodyPartType.Body);
            TankCarrier carrier = (TankCarrier)parts.GetBodyPartOfType(BodyPartType.Carrier);

            if (tankBody == null)
            {
                Debug.LogError("No tank body found");
                return;
            }

            SetBumper(tankBody.Bumper);
        }

        public override void ApplyData(UnitData data)
        {
            if(data == null || data is not PlayerData playerData)
            {
                Debug.LogError("Player data is null or not of type PlayerData");
                return;
            }

            _movementMaxSpeedDefault = playerData.MovementSpeed;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            _movementInputAction = playerMap.FindAction("Movement");
            _movementInputAction.performed += OnPlayerMoveInputPerformed;
            _movementInputAction.canceled += OnPlayerMoveInputCanceled;
        }

        public void DisposeInput(int playerIndex)
        {
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            _movementInputAction.performed -= OnPlayerMoveInputPerformed;
            _movementInputAction.canceled -= OnPlayerMoveInputCanceled;
        }
        #endregion

        private void OnPlayerMoveInputPerformed(InputAction.CallbackContext ctx)
        {
            _inputVector = ctx.ReadValue<Vector2>();
        }

        private void OnPlayerMoveInputCanceled(InputAction.CallbackContext ctx)
        {
            _inputVector = Vector2.zero;
        }

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            _speedMultiplier = _statsController.GetSpeedModifierValue();
            OnMovement?.Invoke(CurrentSpeed, transform);
            HandleInput();
            HandleMovement();
        }

        private void HandleInput()
        {
            // set IsMoving to true if there is any input from the player or if the player is sprinting
            IsMoving = _inputVector.magnitude > 0f;

            // check if there are any constraints on the tank's movement or rotaiton
            if (!_canMove || IsConstrained)
            {
                return;
            }

            _movementInput = new Vector3(_inputVector.x, 0, _inputVector.y);

            _movementInput.Normalize();

            if (_movementInput.magnitude > 0)
            {
                LastMovementInput = _movementInput;
            }
        }

        private void HandleMovement()
        {
            if (!_canMove || _decelerate || IsConstrained)
            {
                ApplyDeceleration();
            }
            else
            {
                if (!_boost.IsBoosting)
                {
                    ApplyWiggle();
                    ApplyMomentum();
                }
            }

            ApplyRotation();
            ApplyMovement();
            ApplyAnimation();
        }

        private void ApplyDeceleration()
        {
            CurrentSpeed -= Time.deltaTime * _decelerationDefault;

            if (CurrentSpeed <= 0)
            {
                CurrentSpeed = 0f;
            }

            if (CurrentSpeed > 0.9f && _forwardAmount == 1)
            {
                _forwardAmount = 0;
            }

            if (CurrentSpeed <= 1)
            {
                _decelerate = false;
            }
        }

        private void ApplyMovement()
        {
            _currentMovement = _tempMaxMovementSpeed * CurrentSpeed * Time.deltaTime * _speedMultiplier * _body.forward;
            _currentMovement.y = 0f;
            HandleGravity();
            _characterController.Move(_currentMovement);
        }

        private void ApplyRotation()
        {
            if (_movementInput.magnitude > 0)
            {
                float targetAngle = Mathf.Atan2(LastMovementInput.x, LastMovementInput.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(_body.eulerAngles.y, targetAngle, ref _turnSsmoothVelocity, _rotationSpeed);

                _moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _moveDir.y = 0;

                _body.localRotation = Quaternion.Euler(0f, angle, 0f);
            }
        }

        private void ApplyAnimation()
        {
            _animation.AnimateMovement(LastMovementInput.magnitude != 0, 1, 0, CurrentSpeed * _speedMultiplier);
        }

        private void ApplyWiggle()
        {
            if (_movementInput.magnitude > 0)
            {
                if (CurrentSpeed == 0f && _forwardAmount == 0)
                {
                    if (!_playerJump.IsJumping)
                    {
                        _playerComponents.TankWiggler.WiggleBody(_backwardWiggle);
                    }

                    _forwardAmount = 1;
                }
            }
            else
            {
                if (CurrentSpeed >= 1f && _forwardAmount == 1)
                {
                    if (!_playerJump.IsJumping)
                    {
                        _playerComponents.TankWiggler.WiggleBody(_forwardWiggle);
                    }

                    _forwardAmount = 0;
                }
            }

            if (CurrentSpeed == 0f && _forwardAmount != 0)
            {
                _forwardAmount = 0;
            }
            else if (CurrentSpeed == 1f && _forwardAmount != 1)
            {
                _forwardAmount = 1;
            }
        }

        public void SetBodyRotation(Quaternion rotation)
        {
            _body.localRotation = rotation;
        }

        public void RotatePlayer(float direction)
        {
            transform.Rotate(_rotationSpeed * direction * Time.deltaTime * Vector3.up);
        }

        private void ApplyMomentum()
        {
            if (_movementInput.magnitude > 0)
            {
                CurrentSpeed += Time.deltaTime * _accelerationDefault;
            }
            else
            {
                CurrentSpeed -= Time.deltaTime * _decelerationDefault;
            }

            CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, 1f);
        }

        public void EnableGravity(bool value)
        {
            _useGravity = value;
        }

        public void SetCurrentSpeed(float currentSpeed)
        {
            CurrentSpeed = currentSpeed;
        }

        public void SetBumper(CollisionEventPublisher bumper)
        {
            _bumper = bumper;
            _bumper.gameObject.SetActive(false); // not necessary? 
        }

        private void OnBumpHandler(Collider other)
        {
            if (_isTouchingWall)
            {
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                _isTouchingWall = true;
            }
        }

        public void StartDeceleration()
        {
            _decelerate = true;
        }

        public override void EnableMovement(bool canMove)
        {
            base.EnableMovement(canMove);

            if (!canMove)
            {
                _lastMovementInputVector = _movementInput;
            }
        }

        #region Constraints
        public override void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canMove = (constraints & AbilityConstraint.Movement) == 0;

            if(IsConstrained == !canMove)
            {
                return;
            }

            IsConstrained = !canMove;
        }
        #endregion

        #region IController
        public override void Activate()
        {
            base.Activate();
            //_characterController.enabled = true;
            EnableGravity(true);

        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Dispose()
        {
            base.Dispose();
            _bumper.OnTriggerEnterEvent -= OnBumpHandler;
            DisposeInput(_playerComponents.PlayerIndex);
            _inputVector = Vector2.zero;
        }

        public override void Restart()
        {
            base.Restart();
            _bumper.gameObject.SetActive(false);
            _bumper.OnTriggerEnterEvent += OnBumpHandler;
            SetUpInput(_playerComponents.PlayerIndex);
        }
        #endregion
    }
}
