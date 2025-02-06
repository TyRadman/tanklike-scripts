using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace TankLike.UnitControllers
{
    using Utils;
    using Sound;

    public class PlayerJump : MonoBehaviour, IController, IConstraintedComponent
    {
        public bool IsActive { get; private set; }
        public bool IsJumping { get; private set; }
        public Action OnJumped { get; internal set; }
        public bool IsConstrained { get; set; }

        [Header("Jump Settings")]
        [SerializeField] private float _jumpMaxHeight;
        [SerializeField] private float _failedJumpMaxHeight = 0.3f;
        [SerializeField] private AnimationCurve _JumpCurve;
        [SerializeField] private AnimationCurve _fallCurve;
        [SerializeField] private float _jumpDuration;
        [SerializeField] private float _flyDuration = 0.5f;
        [SerializeField] private float _fallDuration;
        [Tooltip("The point during the fall process at which the player becames vulnerable."), Range(0f, 1f)]
        [SerializeField] private float _invincibilityEndTime = 0f;
        [SerializeField] private Light _jumpLight;

        [Header("Fuel Settings")]
        [SerializeField] private float _jumpFuelInitialConsumption = 10f;

        [Header("Other Settings")]
        [SerializeField] private AbilityConstraint _constraints;
        [SerializeField] private Audio _thrusterAudio;
        [SerializeField] private Wiggle _onJumpWiggle;
        [SerializeField] private Wiggle _onBoostingWiggle;
        [SerializeField] private Wiggle _onFailedJumpWiggle;

        [Header("References")]
        [SerializeField] private Transform _playerRenderer;
        [SerializeField] private JumpBar _UIBar;

        private PlayerComponents _playerComponents;
        private PlayerFuel _fuel;
        private List<ParticleSystem> _thrusters;
        private List<ParticleSystem> _tracks;
        private ParticleSystem _landParticles;

        private Coroutine _jumpCoroutine;
        private Coroutine _flyCoroutine;
        private Coroutine _fallCoroutine;
        private Coroutine _cooldownCoroutine;
        private Vector3 _rendererGroundedPosition;
        private bool _isJumpPressed;
        private bool _isflying;
        private bool _isUsingFuel;
        private bool _isCooldown;
        private bool _isFailedJumpBeingPerformed = false;

        private const int JUMP_LAYER = 22;
        private const int PLAYER_DAMAGEABLE_LAYER = 23;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _fuel = _playerComponents.Fuel;

            TankBodyParts parts = _playerComponents.TankBodyParts;

            TankCarrier carrier = (TankCarrier)parts.GetBodyPartOfType(BodyPartType.Carrier);
            TankBody body = (TankBody)parts.GetBodyPartOfType(BodyPartType.Body);

            _thrusters = carrier.ThrustersParticles;
            _tracks = carrier.TracksParticles;
            _landParticles = body.LandParticles;
            _rendererGroundedPosition = _playerRenderer.localPosition;

            _jumpLight.enabled = false;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Jump.name).performed += OnJumpInputDown;
            playerMap.FindAction(c.Player.Jump.name).canceled += OnJumpUp;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Jump.name).performed -= OnJumpInputDown;
            playerMap.FindAction(c.Player.Jump.name).canceled -= OnJumpUp;
        }
        #endregion

        public void OnJumpInputDown(InputAction.CallbackContext context)
        {
            bool unableToJump = !IsActive || _isCooldown || IsConstrained;

            if (unableToJump)
            {
                return;
            }

            PerformJump();
        }

        private void PerformJump()
        {
            if (_fuel.HasEnoughFuel(_jumpFuelInitialConsumption) && !_isFailedJumpBeingPerformed)
            {
                OnJumpStarted();
                OnJumped?.Invoke();
                _jumpCoroutine = StartCoroutine(JumpRoutine());
                StartCoroutine(FuelConsumptionRoutine());
                _jumpLight.enabled = true;
            }
            else
            {
                PerformFailedJump();
            }
        }

        public void OnJumpUp(InputAction.CallbackContext context)
        {
            if (!IsActive)
            {
                return;
            }

            _isJumpPressed = false;
        }

        private void OnJumpStarted()
        {
            _isJumpPressed = true;

            PerformForwardWiggle();

            IsJumping = true;
            _playerComponents.AimAssist.SaveLastInputState();
            _playerComponents.Constraints.ApplyConstraints(true, _constraints);
            _thrusters.ForEach(e => e.Play());
            _tracks.ForEach(e => e.Stop());
            GameManager.Instance.AudioManager.Play(_thrusterAudio);
            _playerComponents.Health.SetDamageDetectorsLayer(JUMP_LAYER); //dirty

            _UIBar.PlayShowAnimation();
            _UIBar.SetAmount(1f);

        }

        private void PerformForwardWiggle()
        {
            if (!_playerComponents.PlayerBoost.IsBoosting)
            {
                _playerComponents.TankWiggler.WiggleBody(_onJumpWiggle);
            }
            else
            {
                _playerComponents.TankWiggler.WiggleBody(_onBoostingWiggle);
            }
        }

        private IEnumerator JumpRoutine()
        {
            float timer = 0f;

            while (timer < _jumpDuration)
            {
                timer += Time.deltaTime;
                // Move upward until you reach the max jump height
                float progress = timer / _jumpDuration;
                _playerRenderer.localPosition = _rendererGroundedPosition + new Vector3(0f, _JumpCurve.Evaluate(progress) * _jumpMaxHeight, 0f);

                _playerComponents.Animation.AnimateMovement(true, 1, 0, 0); // dirty

                yield return null;
            }

            _flyCoroutine = StartCoroutine(FlyRoutine());
        }

        private IEnumerator FlyRoutine()
        {
            float timer = 0f;
            _isflying = true;

            while (timer <= _flyDuration)
            {
                if (!_isJumpPressed)
                {
                    break;
                }

                _playerComponents.Animation.AnimateMovement(true, 1, 0, 0); // dirty
                timer += Time.deltaTime;
                _UIBar.SetAmount(1 - timer / _flyDuration);
                yield return null;
            }

            _isUsingFuel = false;
            _isflying = false;
            StopThrusters();
            _fallCoroutine = StartCoroutine(FallRoutine());
        }

        private IEnumerator FallRoutine()
        {
            float timer = 0f;
            float progress = 0f;
            bool isVulnerable = false;

            _UIBar.PlayHideAnimation();

            while (timer < _fallDuration)
            {
                // Move downward
                progress = timer / _fallDuration;
                _playerRenderer.localPosition = _rendererGroundedPosition + new Vector3(0f, _fallCurve.Evaluate(progress) * _jumpMaxHeight, 0f);

                _playerComponents.Animation.AnimateMovement(true, 1, 0, 0); // dirty

                if(progress >= _invincibilityEndTime && !isVulnerable)
                {
                    _playerComponents.Health.SetDamageDetectorsLayer(PLAYER_DAMAGEABLE_LAYER);
                    isVulnerable = true;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            _playerRenderer.localPosition = _rendererGroundedPosition;

            _jumpLight.enabled = false;
            IsJumping = false;
            _playerComponents.Constraints.ApplyConstraints(false, _constraints);
            _playerComponents.AimAssist.RecoverLastInput();
            _landParticles.Play();
            _tracks.ForEach(e => e.Play());
            _isCooldown = false;

        }

        private void StopJump()
        {
            this.StopCoroutineSafe(_jumpCoroutine);
            this.StopCoroutineSafe(_flyCoroutine);

            _isUsingFuel = false;
            _isflying = false;
            _thrusters.ForEach(e => e.Stop());
            _fallCoroutine = StartCoroutine(FallRoutine());
        }

        private void ResetJump()
        {
            StopAllJumpCoroutines();

            _playerRenderer.localPosition = _rendererGroundedPosition;

            IsJumping = false;
            _isUsingFuel = false;
            _isflying = false;

            _playerComponents.Constraints.ApplyConstraints(false, _constraints);
            _playerComponents.Health.SetDamageDetectorsLayer(PLAYER_DAMAGEABLE_LAYER);

            StopThrusters();

            //_UIBar.PlayHideAnimation();
        }

        private void StopAllJumpCoroutines()
        {
            this.StopCoroutineSafe(_jumpCoroutine);
            this.StopCoroutineSafe(_flyCoroutine);
            this.StopCoroutineSafe(_fallCoroutine);
            this.StopCoroutineSafe(_cooldownCoroutine);
        }

        private IEnumerator FuelConsumptionRoutine()
        {
            _isUsingFuel = true;

            _fuel.UseFuel(_jumpFuelInitialConsumption);

            yield return new WaitForSeconds(_jumpDuration);

            while (_isUsingFuel)
            {
                // If we run out of fuel, we break out of the loop
                if (!_fuel.HasEnoughFuel() && _isflying)
                {
                    StopJump();
                    break;
                }
              
                _fuel.UseFuel(_jumpFuelInitialConsumption * Time.deltaTime);

                yield return null;
            }
        }

        #region Thrusters
        private void PlayThrusters()
        {
            _thrusters.ForEach(t => t.Play());

            _jumpLight.enabled = true;
        }

        private void StopThrusters()
        {
            _thrusters.ForEach(t => t.Stop());

            _jumpLight.enabled = false;
        }
        #endregion

        #region Failed Jump
        private void PerformFailedJump()
        {
            if(_isFailedJumpBeingPerformed)
            {
                return;
            }

            _playerComponents.TankWiggler.WiggleBody(_onFailedJumpWiggle);
            StartCoroutine(FailedJumpRoutine());
        }

        private IEnumerator FailedJumpRoutine()
        {
            float timer = 0f;
            float duration = _onFailedJumpWiggle.Duration * 0.7f;

            _isFailedJumpBeingPerformed = true;

            PlayThrusters();

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float progress = timer / duration;
                _playerRenderer.localPosition = _rendererGroundedPosition + new Vector3(0f, _JumpCurve.Evaluate(progress) * _failedJumpMaxHeight, 0f);

                yield return null;
            }

            timer = 0f;
            float fallDuration = _onFailedJumpWiggle.Duration - duration;

            while (timer < fallDuration)
            {
                timer += Time.deltaTime;
                float progress = 1f - timer / fallDuration;
                _playerRenderer.localPosition = _rendererGroundedPosition + new Vector3(0f, _JumpCurve.Evaluate(progress) * _failedJumpMaxHeight, 0f);

                yield return null;
            }

            _isFailedJumpBeingPerformed = false;
            StopThrusters();
        }
        #endregion

        #region Constraints
        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canJump = (constraints & AbilityConstraint.Jump) == 0;
            IsConstrained = !canJump;
        }
        #endregion

        #region IController
        public void Activate()
        {
            IsActive = true;
            // Hide it so it won't be visible after switching rooms
            _UIBar.ForceHide();
        }

        public void Deactivate()
        {
            IsActive = false;
            ResetJump();
        }

        public void Restart()
        {
            SetUpInput(_playerComponents.PlayerIndex);
        }

        public void Dispose()
        {
            DisposeInput(_playerComponents.PlayerIndex);
            StopAllJumpCoroutines();
        }
        #endregion
    }
}
