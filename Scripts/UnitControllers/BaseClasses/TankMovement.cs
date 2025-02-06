using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;

    public abstract class TankMovement : MonoBehaviour, IController, IConstraintedComponent, IUnitDataReciever
    {
        [field: SerializeField] public float CurrentSpeed { get; protected set; }
        public bool IsActive { get; protected set; }
        public Vector3 MovementInput => _movementInput;
        public bool IsConstrained { get; set; }

        [Header("Movement")]
        [SerializeField] protected float _movementMaxSpeedDefault;
        [SerializeField] protected float _accelerationDefault;
        [SerializeField] protected float _decelerationDefault;

        [Header("Wiggles")]
        [SerializeField] protected Wiggle _forwardWiggle;
        [SerializeField] protected Wiggle _backwardWiggle;

        [Header("Rotation")]
        [SerializeField] protected float _rotationSpeed = 0.2f;
        [Tooltip("Used for enemies when obstacles are detected (with transform.Rotate)")]
        [SerializeField] protected float _turnSpeed = 180f;

        [Header("Gravity")]
        [SerializeField] protected bool  _useGravity;
        [SerializeField] protected float GroundGravity;
        [SerializeField] protected float Gravity;
        //[SerializeField] protected float _speedMultiplier = 1;
        [SerializeField] protected bool _canMove { get; set; } = true;
        [SerializeField] protected bool _canRotate = true;

        protected List<ParticleSystem> _tracksParticles = new List<ParticleSystem>();
        protected List<ParticleSystem> _dustParticles = new List<ParticleSystem>();
        protected TankComponents _components;
        protected CharacterController _characterController;
        protected Transform _body;
        protected Transform _turret;
        protected TankAnimation _animation;
        protected PlayerStatsController _statsController;
        protected Vector3 _currentMovement;
        protected Vector3 _movementInput;
        protected float _tempMaxMovementSpeed;
        protected float _tempAcceleration;
        protected float _tempDeceleration;
        protected int _forwardAmount;
        protected int _turnAmount;
        protected int _lastForwardAmount;
        protected float _turnSsmoothVelocity;

        public virtual void SetUp(IController controller)
        {
            if (controller is not TankComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _components = components;
            TankBodyParts parts = _components.TankBodyParts;

            _body = parts.GetBodyPartOfType(BodyPartType.Body).transform;
            _turret = parts.GetBodyPartOfType(BodyPartType.Turret).transform;
            TankCarrier carrier = (TankCarrier)parts.GetBodyPartOfType(BodyPartType.Carrier);

            _statsController = _components.StatsController;

            if (carrier != null)
            {
                if (carrier.TracksParticles != null)
                {
                    _tracksParticles = carrier.TracksParticles;
                }

                if (carrier.DustParticles != null)
                {
                    _dustParticles = carrier.DustParticles;
                }
            }

            _characterController = _components.CharacterController;
            _animation = _components.Animation;
        }

        public virtual void MoveCharacterController(Vector3 movementInput)
        {
            if (!_canMove || IsConstrained)
            {
                return;
            }

            //Momentum
            if ((_forwardAmount > 0 && _lastForwardAmount >= 0) || (_forwardAmount < 0 && _lastForwardAmount <= 0))
            {
                CurrentSpeed += Time.deltaTime * _accelerationDefault * _accelerationDefault;
            }
            else if (_forwardAmount == 0 || (_forwardAmount > 0 && _lastForwardAmount < 0) || (_forwardAmount < 0 && _lastForwardAmount > 0))
            {
                CurrentSpeed -= Time.deltaTime * _decelerationDefault * _decelerationDefault;
            }

            CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, 1f);

            Vector3 lastMovementInput = movementInput;

            //Rotation
            if (movementInput.magnitude > 0)
            {
                float targetAngle = Mathf.Atan2(lastMovementInput.x, lastMovementInput.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(_body.eulerAngles.y, targetAngle, ref _turnSsmoothVelocity, _rotationSpeed);
                _body.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            //Movement
            _currentMovement = _body.forward * (_tempMaxMovementSpeed * CurrentSpeed * Time.deltaTime);
            _currentMovement.y = 0f;
            HandleGravity();
            _characterController.Move(_currentMovement);

            //Animation
            _animation.AnimateMovement(lastMovementInput.magnitude != 0, 1, 0, CurrentSpeed * _statsController.GetSpeedModifierValue());
        }

        public void SetBodyRotation(Quaternion rotation)
        {
            _body.localRotation = rotation;
        }

        protected void ObstacleMovement()
        {
            if (!_canMove || IsConstrained)
            {
                return;
            }

            //Momentum
            if (_forwardAmount > 0 && _lastForwardAmount >= 0)
            {
                CurrentSpeed += Time.deltaTime * _accelerationDefault * _accelerationDefault;
            }
            else if (_forwardAmount == 0)
            {
                CurrentSpeed -= Time.deltaTime * _decelerationDefault * _decelerationDefault;
            }

            CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, 1f);

            //Movement
            _currentMovement = _lastForwardAmount * _body.forward * (_tempMaxMovementSpeed * CurrentSpeed * Time.deltaTime);
            _currentMovement.y = 0f;

            _characterController.Move(_currentMovement);

            //Animation
            if(_animation != null)
            {
                _animation.AnimateMovement(_forwardAmount != 0, _lastForwardAmount, _turnAmount, CurrentSpeed * _statsController.GetSpeedModifierValue());
            }

            //Rotation
            _body.Rotate(Vector3.up * _turnAmount * _turnSpeed * Time.deltaTime);       
        }

        public void MoveCharacterController(float speed)
        {
            //Movement
            _currentMovement += speed * Time.deltaTime * transform.forward;
            _characterController.Move(_currentMovement * Time.deltaTime);

            //Animation
            _animation.AnimateMovement(_forwardAmount != 0, _forwardAmount, _turnAmount, _statsController.GetSpeedModifierValue());
        }

        public void MoveCharacterController(Vector3 direction, float speed)
        {
            //Movement
            _currentMovement += speed * Time.deltaTime * direction;
            _characterController.Move(_currentMovement * Time.deltaTime);

            //Animation
            _animation.AnimateMovement(_forwardAmount != 0, _forwardAmount, _turnAmount, _statsController.GetSpeedModifierValue());
        }

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

        public void ResetMovement()
        {
            _currentMovement = Vector3.zero;
        }

        #region External Methods
        public void SetForwardAmount(int amount)
        {
            _forwardAmount = amount;
        }

        public virtual void EnableMovement(bool canMove)
        {
            _canMove = canMove;
        }

        public virtual void StopMovement()
        {
            _currentMovement = Vector3.zero;
            CurrentSpeed = 0f;
        }

        public void SetSpeed(float speed)
        {
            _movementMaxSpeedDefault = speed;
            _tempMaxMovementSpeed = _movementMaxSpeedDefault;
        }

        public float GetSpeed()
        {
            return _movementMaxSpeedDefault;
        }

        public float GetMultipliedSpeed()
        {
            return _movementMaxSpeedDefault * CurrentSpeed;
        }

        public void EnableRotation(bool enable)
        {
            _canRotate = enable;
        }

        public float GetRotationSpeed()
        {
            return _rotationSpeed;
        }
        #endregion


        #region Constraints
        public virtual void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canMove = (constraints & AbilityConstraint.Movement) == 0;
            IsConstrained = !canMove;
        }
        #endregion

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
            _characterController.enabled = true;
            _tracksParticles.ForEach(t => t.Play());
            _dustParticles.ForEach(t => t.Play());
        }

        public virtual void Deactivate()
        {
            IsActive = false;
            _characterController.enabled = false;
            _tracksParticles.ForEach(t => t.Stop());
            _dustParticles.ForEach(t => t.Stop());
            _currentMovement = Vector3.zero;
            CurrentSpeed = 0f;
            _forwardAmount = 0;
            _turnAmount = 0;
        }

        public virtual void Restart()
        {
            _canMove = true;
            IsConstrained = false;
            _currentMovement = Vector3.zero;
            CurrentSpeed = 0f;
            _forwardAmount = 0;

            // TODO: keep in mind the revive process when we have upgrades
            _tempMaxMovementSpeed = _movementMaxSpeedDefault;
            _tempAcceleration = _accelerationDefault;
            _tempDeceleration = _decelerationDefault;
        }

        public virtual void Dispose()
        {
        }
        #endregion

        #region IUnitDataReciever
        public virtual void ApplyData(UnitData data)
        {

        }
        #endregion
    }
}
