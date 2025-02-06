using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace TankLike.UnitControllers
{
    using UI.InGame;
    using Utils;

    /// <summary>
    /// Controls the cross hair of the player
    /// </summary>
    public class PlayerCrosshairController : MonoBehaviour, IController, IInput, IConstraintedComponent
    {
        public bool IsActive { get; private set; }
        public bool IsConstrained { get; set; }

        [Header("Crosshair")]
        [SerializeField] private Vector2 _crosshairRadiusRange;
        [SerializeField] private float _crosshairSpeed = 100f;
        [Range(0f, 100f)] 
        [SerializeField] private float _crosshairSnapSpeed = 4f;
        [SerializeField] private bool _isAvoidingWalls = true;

        [Header("References")]
        [SerializeField] private Crosshair _crossHair;
        [SerializeField] private Transform _turret;
        [Tooltip("How strongly the aim assist influences the crosshair controls")]
        [SerializeField] private float _crosshairSpeedMultiplier = 1f;
        [SerializeField] private LayerMask _raycastLayerMask;

        [Header("Dotted line")]
        [SerializeField] private int _dotsCount = 7;
        [SerializeField] private AimingDot _dotPrefab;

        private PlayerTurretController _turretController;
        private List<AimingDot> _dots = new List<AimingDot>();
        /// <summary>
        /// We cache that to avoid calculating it every frame.
        /// </summary>
        private List<float> _dotsProgressValues = new List<float>();
        private PlayerComponents _playerComponents;
        private Transform _tank;
        private Vector3 _aimPosition;
        private Vector2 _originalRadiusRange;
        private Vector3 _lastTurretRotation;
        private Vector3 _input;
        private Vector3 _offset;
        private bool _isAiming = false;

        public const float MIN_AIM_SENSITIVITY = 2f;
        public const float MAX_AIM_SENSITIVITY = 40f;
        public const float CROSSHAIR_DOT_OFFSET = 1.5f;
        public const float CROSSHAIR_DOT_END_OFFSET = 0.5f;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _tank = transform;

            _crossHair.SetUp();

            _originalRadiusRange = _crosshairRadiusRange;
            _turretController = _playerComponents.TurretController as PlayerTurretController;

            RegisterForOverHeatRecharge();
            CreateDottedLineDots();
            ShowDots();
        }

        private void RegisterForOverHeatRecharge()
        {
            PlayerOverHeat overHeat = _playerComponents.GetUnitComponent<PlayerOverHeat>();

            if (overHeat != null)
            {
                overHeat.OnShotRecharged += PlayReloadAnimation;
            }
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.TurretRotation.name).performed += UpdateInput;
            playerMap.FindAction(c.Player.TurretRotation.name).canceled += ResetInput;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.TurretRotation.name).performed -= UpdateInput;
            playerMap.FindAction(c.Player.TurretRotation.name).canceled -= ResetInput;
        }
        #endregion

        private void Update()
        {
            if (!IsActive || IsConstrained)
            {
                return;
            }

            MoveCrosshair();
            HandleMovement();
            UpdateDots();
        }

        #region Aim Assist Process
        public void EnableIsAiming()
        {
            _isAiming = true;
        }

        public void DisableIsAiming()
        {
            if (_isAiming)
            {
                _isAiming = false;
                _offset = _crossHair.transform.position - _tank.position;
            }
        }

        public void SetAimingPosition(Vector3 position)
        {
            position.y = Constants.ShootingPointHeight;
            _aimPosition = position;
        }
        #endregion

        private void UpdateInput(InputAction.CallbackContext ctx)
        {
            _input = ctx.ReadValue<Vector2>();
        }

        private void ResetInput(InputAction.CallbackContext ctx)
        {
            _input = Vector3.zero;
        }

        private void MoveCrosshair()
        {
            if (!IsActive || _isAiming || IsConstrained)
            {
                return;
            }

            // convert the input into a vector3
            Vector3 inputDirection = new Vector3(_input.x, 0f, _input.y);

            // the offset to be applied to the cursor's position
            Vector3 lookDirection = _crosshairSpeed * _crosshairSpeedMultiplier * Time.deltaTime * inputDirection;

            // add the offset to the total offset that will apply to the cursor's 
            _offset += lookDirection;

            // cache the turret forward direction
            Vector3 turretForward = _turret.forward;

            // if the offset added to the crosshair is less than the minimum range
            if (_offset.magnitude < _crosshairRadiusRange.x)
            {
                float angle;
                float firstAngle = Vector3.SignedAngle(inputDirection, turretForward, Vector3.up);

                if (firstAngle > 0)
                {
                    angle = Mathf.Atan2(_offset.z, _offset.x) + (lookDirection.magnitude / _crosshairRadiusRange.x);
                }
                else
                {
                    angle = Mathf.Atan2(_offset.z, _offset.x) - (lookDirection.magnitude / _crosshairRadiusRange.x);
                }

                _offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * _crosshairRadiusRange.x;
            }
            // if the offset to be added to the crosshair position is higher than the maximum range
            else if (_offset.magnitude > _crosshairRadiusRange.y)
            {
                // Clamp to the maximum radius
                _offset = _offset.normalized * _crosshairRadiusRange.y;
            }

            if (_isAvoidingWalls)
            {
                // Perform a raycast from the tank's position towards the current crosshair's position
                Vector3 directionToCrosshair = (_tank.position + _offset) - _tank.position;
                Ray ray = new Ray(_tank.position, directionToCrosshair.normalized);

                if (Physics.Raycast(ray, out RaycastHit hit, _crosshairRadiusRange.y, _raycastLayerMask))
                {
                    // If an obstacle is detected, clamp the offset to the hit point
                    _offset = hit.point - _tank.position;

                    // Ensure that the offset doesn't exceed the maximum crosshair range
                    if (_offset.magnitude > _crosshairRadiusRange.y)
                    {
                        _offset = _offset.normalized * _crosshairRadiusRange.y;
                    }
                }
            }
        }

        private void HandleMovement()
        {
            if (_isAiming)
            {
                _crossHair.transform.position = Vector3.Lerp(_crossHair.transform.position, _aimPosition, _crosshairSnapSpeed * Time.deltaTime);
            }
            else
            {
                // move the cursor
                Vector3 position = _tank.position + _offset;

                Vector3 newPosition = Vector3.Lerp(_crossHair.transform.position, position, _crosshairSnapSpeed * Time.deltaTime);
                newPosition.y = Constants.ShootingPointHeight;

                // apply movement to the crosshair
                _crossHair.transform.position = newPosition;
            }

            _turretController.HandleTurretRotation(_crossHair.transform);
        }

        #region Dotted line
        private void CreateDottedLineDots()
        {
            Transform dotsParent = new GameObject($"P{_playerComponents.PlayerIndex} DottedLine").transform;
            Color dotsColor = (_playerComponents.Stats as PlayerData).Skins[_playerComponents.PlayerIndex].Color;

            for (int i = 0; i < _dotsCount - 1; i++)
            {
                AimingDot dot = Instantiate(_dotPrefab, dotsParent);

                dot.SetUp();
                _dots.Add(dot);
                _dotsProgressValues.Add((float)i / ((float)_dotsCount - 1f));

                dot.SetColor(dotsColor);
            }
        }

        public void ShowDots()
        {
            int dotsCount = _dots.Count;

            for (int i = 0; i < dotsCount; i++)
            {
                _dots[i].Show();
            }
        }

        public void HideDots()
        {
            int dotsCount = _dots.Count;

            for (int i = 0; i < dotsCount; i++)
            {
                _dots[i].Hide();
            }
        }

        public void UpdateDots()
        {
            int dotsCount = _dots.Count;
            
            Vector3 crossHairPosition = _crossHair.transform.position;
            Vector3 tankPosition = transform.position;

            float distance = Vector3.Distance(crossHairPosition, tankPosition);

            Vector3 directionToCorsshair = (crossHairPosition - tankPosition).normalized;
            Vector3 directionToTank = (tankPosition - crossHairPosition).normalized;

            float originOffset = CROSSHAIR_DOT_OFFSET;
            float endOffset = CROSSHAIR_DOT_END_OFFSET;

            if (distance * 0.5f < CROSSHAIR_DOT_OFFSET)
            {
                originOffset = Mathf.Min(distance * 0.5f, CROSSHAIR_DOT_OFFSET);
            }

            if (distance * 0.5f < CROSSHAIR_DOT_END_OFFSET)
            {
                endOffset = Mathf.Min(distance * 0.5f, CROSSHAIR_DOT_END_OFFSET);
            }

            Vector3 origin = tankPosition + directionToCorsshair * originOffset;
            Vector3 end = crossHairPosition + directionToTank * endOffset;


            for (int i = 0; i < dotsCount; i++)
            {
                AimingDot dot = _dots[i];

                Vector3 position = Vector3.Lerp(origin, end, _dotsProgressValues[i]).Where(y: Constants.ShootingPointHeight);

                dot.SetPosition(position);
            }
        }
        #endregion

        #region Externals
        public Transform GetCrosshairTransform()
        {
            return _crossHair.transform;
        }

        public Crosshair GetCrosshair()
        {
            return _crossHair;
        }

        public void SetAimRange(Vector2 radiusRange)
        {
            _crosshairRadiusRange = radiusRange;
        }

        public void ResetAimRange()
        {
            _crosshairRadiusRange = _originalRadiusRange;
        }

        public void EnableCrosshair(bool enable)
        {
            if (enable)
            {
                _crossHair.Enable();
            }
            else
            {
                _crossHair.Disable();
            }
        }

        public void SetCrosshairSpeedMultiplier(float crosshairSpeedMultiplier)
        {
            _crosshairSpeedMultiplier = crosshairSpeedMultiplier;
        }

        public void ResetCrosshairSpeedMultiplier()
        {
            _crosshairSpeedMultiplier = 1f;
        }

        public void SetAimSensitivity(float sensitivity)
        {
            _crosshairSpeed = sensitivity;
        }

        public void Enable(bool enable)
        {
            IsActive = enable;
        }

        public void SetColor(Color color)
        {
            _crossHair.SetColor(color);
        }

        public void PlayShootingAnimation()
        {
            _crossHair.Visuals.PlayShootAnimation();
        }

        public void PlayReloadAnimation()
        {
            _crossHair.Visuals.PlayOnShotReloadAnimation();
        }

        public void EnableWallsAvoidance()
        {
            _isAvoidingWalls = true;
        }

        public void DisableWallsAvoidance()
        {
            _isAvoidingWalls = false;
        }
        #endregion

        #region Constraints
        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canAim = (constraints & AbilityConstraint.Rotation) == 0;
            IsConstrained = !canAim;
        }
        #endregion

        #region IController
        public void Activate()
        {
            IsActive = true;

            if(_playerComponents.PlayerWeaponSwapper.GetSwappingStatus() == PlayerWeaponSwapper.WeaponType.BaseWeapon)
            {
                EnableCrosshair(true);
            }

            _offset = _lastTurretRotation * _crosshairRadiusRange.x;
            _crossHair.transform.position = _tank.position + _offset;
        }

        public void Deactivate()
        {
            IsActive = false;

            EnableCrosshair(false);
            _lastTurretRotation = _turret.forward;
        }

        public void Restart()
        {
            IsConstrained = false;
            SetUpInput(_playerComponents.PlayerIndex);
            _crossHair.ResetValues();
        }

        public void Dispose()
        {
            DisposeInput(_playerComponents.PlayerIndex);
            EnableCrosshair(false);
            HideDots();
        }
        #endregion
    }
}
