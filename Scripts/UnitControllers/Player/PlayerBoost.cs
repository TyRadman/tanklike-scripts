using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Sound;
    using Utils;

    public class PlayerBoost : MonoBehaviour, IController, IInput, IConstraintedComponent, IResumable
    {
        public bool IsActive { get; private set; }
        public bool IsBoosting { get; private set; }
        public float DistanceTravelled { get; private set; } = 0f;
        public bool IsConstrained { get; set; }

        public Action OnBoostStart;
        public Action OnBoostUpdate;
        public Action OnBoostEnd;

        [SerializeField] private Light _boostLight;

        [Header("Settings")]
        [SerializeField] private AnimationCurve _boostCurve;
        [SerializeField] private AnimationCurve _accelerationCurve;
        [SerializeField] private AbilityConstraint _constraints;
        [SerializeField] private float _startBoostSpeed = 0.5f;

        [Header("Wiggles")]
        [SerializeField] protected Wiggle _boostWiggle;

        [Header("References")]
        [SerializeField] private TankBumper _bumper;
        [SerializeField] private ParticleSystem _outOfFuelParticles;

        [Header("Audio")]
        [SerializeField] private Audio _boostAudio;
        [SerializeField] private Audio _failedBoostAudio;

        private PlayerComponents _playerComponents;
        private PlayerMovement _movement;
        private List<ParticleSystem> _boostParticles = new List<ParticleSystem>();
        private PlayerFuel _fuel;
        private Coroutine _boostCoroutine;
        private InputAction _boostInputAction;

        // main modifiers
        private float _startFuelConsumption;
        private float _boostFuelConsumption;
        private float _currentSpeedMultiplier;

        private bool _isBoostingInputOn = false;

        private const float DECELERATION_DURATION = 0.3f;
        private const float LINES_EFFECT_START_TIME = 0.25f;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _movement = (PlayerMovement)_playerComponents.Movement;
            _fuel = _playerComponents.Fuel;

            TankBodyParts parts = _playerComponents.TankBodyParts;

            TankBody body = (TankBody)parts.GetBodyPartOfType(BodyPartType.Body);
            _boostParticles = body.BoostParticles;

            _boostLight.enabled = false;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            _boostInputAction = playerMap.FindAction(c.Player.Boost.name);

            _boostInputAction.started += StartBoost;
            _boostInputAction.canceled += StopBoost;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            int index = _playerComponents.PlayerIndex;

            if(_boostInputAction == null)
            {
                Debug.LogError("Boost input action is null.");
                return;
            }

            _boostInputAction.started -= StartBoost;
            _boostInputAction.canceled -= StopBoost;
        }
        #endregion

        private void StartBoost(InputAction.CallbackContext _)
        {
            if (!IsActive || IsConstrained)
            {
                return;
            }

            if (!_fuel.HasEnoughFuel(_startFuelConsumption))
            {
                OnOutOfFuel();
                return;
            }

            OnBoostStart?.Invoke();

            if (IsBoosting)
            {
                RecoverBoost();
                return;
            }

            GameManager.Instance.AudioManager.Play(_boostAudio);
            _isBoostingInputOn = true;

            // apply constraints
            _playerComponents.Constraints.ApplyConstraints(true, _constraints);

            _boostParticles.ForEach(p => p.Play());

            _playerComponents.TankWiggler.WiggleBody(_boostWiggle);
            GameManager.Instance.CameraManager.PlayerCameraFollow.SetSpeedMultiplier(_currentSpeedMultiplier, _playerComponents.PlayerIndex);

            _movement.StopMovement();
            _movement.SetCurrentSpeed(0.5f);

            this.StopCoroutineSafe(_boostCoroutine);
            _boostCoroutine = StartCoroutine(BoostStartProcess());
        }

        private void OnOutOfFuel()
        {
            GameManager.Instance.AudioManager.Play(_failedBoostAudio);
            _outOfFuelParticles.Play();
        }

        private void RecoverBoost()
        {
            StopAllCoroutines();
            _isBoostingInputOn = true;
            StartCoroutine(BoostUpdateProcess());
        }

        #region Boost Process
        private IEnumerator BoostStartProcess()
        {
            DistanceTravelled = 0f;
            _bumper.EnableBump();
            IsBoosting = true;
            float timer = 0f;
            float accelerationTime = 0.25f;

            if (!_fuel.HasEnoughFuel(_startFuelConsumption))
            {
                yield break;
            }

            _boostLight.enabled = true;

            _fuel.UseFuel(_startFuelConsumption);

            // acceleration process
            while (timer < accelerationTime)
            {
                float dt = Time.deltaTime;
                timer += dt;

                OnBoostUpdate?.Invoke();

                float tankSpeed = _movement.GetMultipliedSpeed();
                DistanceTravelled += tankSpeed * dt;

                float t = _startBoostSpeed + _accelerationCurve.Evaluate(timer / accelerationTime) * (_currentSpeedMultiplier - _startBoostSpeed);
                _movement.SetCurrentSpeed(t);
                yield return null;
            }

            StartCoroutine(BoostUpdateProcess());
        }

        private IEnumerator BoostUpdateProcess()
        {
            if (!_fuel.HasEnoughFuel())
            {
                StartCoroutine(BoostEndProcess());
                yield break;
            }

            float linesEffectTimer = 0f;
            float tankSpeed = _movement.GetMultipliedSpeed();

            while (_isBoostingInputOn && !IsConstrained)
            {
                float dt = Time.deltaTime;

                DistanceTravelled += tankSpeed * dt;

                OnBoostUpdate?.Invoke();

                if (!_fuel.HasEnoughFuel() || !_isBoostingInputOn)
                {
                    break;
                }

                if (linesEffectTimer < LINES_EFFECT_START_TIME)
                {
                    linesEffectTimer += dt;

                    if (linesEffectTimer >= LINES_EFFECT_START_TIME)
                    {
                        GameManager.Instance.EffectsUIController.PlaySpeedLinesEffect();
                    }
                }

                float speed = _currentSpeedMultiplier * _bumper.GetBumperMultiplier();
                _movement.SetCurrentSpeed(speed);
                tankSpeed = _movement.GetMultipliedSpeed();
                _fuel.UseFuel(_boostFuelConsumption * dt);
                yield return null;
            }

            StartCoroutine(BoostEndProcess());
        }

        private IEnumerator BoostEndProcess()
        {
            float timer = 0f;

            float startSpeed = _movement.CurrentSpeed;

            while (timer < DECELERATION_DURATION)
            {
                float dt = Time.deltaTime;
                timer += dt;

                OnBoostUpdate?.Invoke();

                float tankSpeed = _movement.GetMultipliedSpeed();
                DistanceTravelled += tankSpeed * dt;

                float t = timer / DECELERATION_DURATION;
                float speed = Mathf.Lerp(startSpeed, 1f, t);
                _movement.SetCurrentSpeed(speed);
                yield return null;
            }

            _boostLight.enabled = false;
            OnBoostEnd?.Invoke();
            StopBoost();
        }
        #endregion

        private void StopBoost(InputAction.CallbackContext _)
        {
            _isBoostingInputOn = false;
        }

        private void StopBoost()
        {
            IsBoosting = false;

            _bumper.DisableBump();

            _boostParticles.ForEach(p => p.Stop());

            GameManager.Instance.EffectsUIController.StopSpeedLinesEffect();

            _playerComponents.Constraints.ApplyConstraints(false, _constraints);

            GameManager.Instance.CameraManager.PlayerCameraFollow.ResetSpeedMultiplier(_playerComponents.PlayerIndex);
        }

        public void ResetDistanceCalculator()
        {
            DistanceTravelled = 0f;
        }

        #region Fuel Consumption Rate Modifiers

        /// <summary>
        /// Sets the start fuel consumption rate values as the provided value.
        /// </summary>
        /// <param name="multiplier"></param>
        public void SetStartConsumptionRate(float multiplier)
        {
            _startFuelConsumption = multiplier;
        }

        /// <summary>
        /// Sets the update fuel consumption rate values as the provided value.
        /// </summary>
        /// <param name="multiplier"></param>
        public void SetConsumptionRate(float multiplier)
        {
            _boostFuelConsumption = multiplier;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _currentSpeedMultiplier = multiplier;
        }
        #endregion

        #region Constraints
        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canBoost = (constraints & AbilityConstraint.Boost) == 0;

            if (IsConstrained == !canBoost)
            {
                return;
            }

            IsConstrained = !canBoost;

            if (IsConstrained)
            {
                _isBoostingInputOn = false;
                _movement.StartDeceleration();
            }
            else
            {
                ResumeComponent();
            }
        }

        public void ResumeComponent()
        {
            if (_boostInputAction.inProgress)
            {
                StartBoost(new InputAction.CallbackContext());
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
            StopBoost();

            if (_boostCoroutine != null)
            {
                StopCoroutine(_boostCoroutine);
            }

            _bumper.ResetWallsCount();
        }

        public void Restart()
        {
            SetUpInput(_playerComponents.PlayerIndex);

            _boostParticles.ForEach(p => p.Stop());
            _bumper.DisableBump();

            IsConstrained = false;
        }

        public void Dispose()
        {
            _isBoostingInputOn = false;
            StopBoost();
            DisposeInput(_playerComponents.PlayerIndex);
            StopAllCoroutines();
        }
        #endregion
    }
}
