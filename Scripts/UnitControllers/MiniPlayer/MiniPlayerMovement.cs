using System.Collections;
using System.Collections.Generic;
using TankLike.Sound;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class MiniPlayerMovement : MonoBehaviour, IController, IInput
    {
        public bool IsActive { get; private set; }
        [field: SerializeField] public bool IsMoving { get; private set; } = false;
        public System.Action<float, Transform> OnMovement;
        public Vector3 LastMovementInput { get; set; }

        [HideInInspector] public float CurrentSpeed { get; private set; } = 0f;

        [Header("Movement")]
        [SerializeField] protected float _maxSpeed = 6f;
        [SerializeField] protected float _accelerationDefault = 2.25f;
        [SerializeField] protected float _decelerationDefault = 2.25f;

        [Header("Rotation")]
        [SerializeField] protected float _rotationSpeed = 0.2f;
        [Tooltip("Used for enemies when obstacles are detected (with transform.Rotate)")]
        [SerializeField] protected float _turnSpeed = 180f;

        [Header("Wiggles")]
        [SerializeField] protected Wiggle _forwardWiggle;
        [SerializeField] protected Wiggle _backwardWiggle;

        private CollisionEventPublisher _bumper;
        private MiniPlayerComponents _playerComponents;
        private InputAction _movementInputAction;
        protected CharacterController _characterController;
        protected Transform _body;
        protected Transform _turret;
        private Vector3 _moveDir;
        private Vector3 _lastMovementInputVector;
        private Vector2 _inputVector;
        protected Vector3 _currentMovement;
        protected Vector3 _movementInput;
        protected float _tempAcceleration;
        protected float _tempDeceleration;
        protected int _forwardAmount;
        protected int _turnAmount;
        protected int _lastForwardAmount;
        protected float _turnSsmoothVelocity;
        private bool _decelerate;
        private bool _isTouchingWall;
        private bool _canMove = true;

        [Header("Gravity")]
        [SerializeField] protected bool _useGravity;
        [SerializeField] protected float GroundGravity;
        [SerializeField] protected float Gravity;
        [SerializeField] protected bool _canRotate = true;
        public bool IsConstrained { get; set; }

        public void SetUp(IController controller)
        {
            if (controller is not UnitComponents unitComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = unitComponents as MiniPlayerComponents;

            MiniPlayerBodyParts parts = _playerComponents.BodyParts;

            TankBody tankBody = (TankBody)parts.GetBodyPartOfType(BodyPartType.Body);
            TankCarrier carrier = (TankCarrier)parts.GetBodyPartOfType(BodyPartType.Carrier);

            _body = tankBody.transform;
            _turret = parts.GetBodyPartOfType(BodyPartType.Turret).transform;

            _characterController = _playerComponents.CharacterController;

            SetBumper(tankBody.Bumper);
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

            // REVIEW: Do we need to have stat modifiers for the miniplayer?
            //_speedMultiplier = _statsController.GetSpeedModifierValue();
            IsConstrained = false;
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
                ApplyWiggle();
                ApplyMomentum();
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
            _currentMovement = _maxSpeed * CurrentSpeed * Time.deltaTime * _body.forward;
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
            //_animation.AnimateMovement(LastMovementInput.magnitude != 0, 1, 0, CurrentSpeed * _speedMultiplier);
        }

        private void ApplyWiggle()
        {
            if (_movementInput.magnitude > 0)
            {
                if (CurrentSpeed == 0f && _forwardAmount == 0)
                {
                    _playerComponents.Wiggler.WiggleBody(_backwardWiggle);
                    _forwardAmount = 1;
                }
            }
            else
            {
                if (CurrentSpeed >= 1f && _forwardAmount == 1)
                {
                    _playerComponents.Wiggler.WiggleBody(_forwardWiggle);
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

        public void EnableMovement(bool canMove)
        {
            if (!canMove)
            {
                _lastMovementInputVector = _movementInput;
            }
        }

        #region Constraints
        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canMove = (constraints & AbilityConstraint.Movement) == 0;

            if (IsConstrained == !canMove)
            {
                return;
            }

            IsConstrained = !canMove;
        }
        #endregion

        #region Utilities
        protected void HandleGravity()
        {
            if (!_useGravity)
            {
                return;
            }

            if (_characterController.isGrounded)
            {
                _currentMovement.y = GroundGravity;
            }
            else
            {
                _currentMovement.y = Gravity;
            }
        }
        #endregion

        #region IController
        public void Activate()
        {
            IsActive = true;
            //_characterController.enabled = true;
            EnableGravity(true);

        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Dispose()
        {
            _bumper.OnTriggerEnterEvent -= OnBumpHandler;
            DisposeInput(_playerComponents.PlayerIndex);
            _inputVector = Vector2.zero;
        }

        public void Restart()
        {
            _bumper.gameObject.SetActive(false);
            _bumper.OnTriggerEnterEvent += OnBumpHandler;
            SetUpInput(_playerComponents.PlayerIndex);
        }
        #endregion
    }
}