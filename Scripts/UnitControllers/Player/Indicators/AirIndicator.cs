using System;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using UnitControllers;
    using Utils;

    public class AirIndicator : MonoBehaviour, IPoolable
    {
        public enum IndicatorLineState
        {
            None = 0, GroundTrajectory = 1, AirTrajectory = 2
        }

        public enum IndicatorState
        {
            Active = 0, Inactive = 1
        }

        private IndicatorLineState _lineState;
        private IndicatorState _indicatorState;

        public Action<IPoolable> OnReleaseToPool { get; private set; }

        [SerializeField] private Transform _indicatorEnd;
        [SerializeField] private Transform _groundLine;
        [SerializeField] private LayerMask _obstructionLayerMask;

        [Header("Animations")]
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _startAnimationClip;
        [SerializeField] private AnimationClip _finishAnimationClip;
        [SerializeField] private Animation _groundLineAnimation;
        [SerializeField] private AnimationClip _groundLineStartClip;
        [SerializeField] private AnimationClip _groundLineEndClip;

        [Header("Curve")]
        [SerializeField] private LineRenderer _line;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _lineHeight = 6f;
        [SerializeField] private int _levelOfDetail = 10;

        [Header("Colors")]
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _inactiveColor;
        [SerializeField] private IndicatorVisualsPreset _groundLineActivePreset;
        [SerializeField] private IndicatorVisualsPreset _groundLineInactivePreset;
        [SerializeField] private SpriteRenderer _endPointSprite;
        [SerializeField] private MeshRenderer _groundLineMeshRenderer;

        [System.Serializable]
        public class IndicatorVisualsPreset
        {
            public Color CenterColor;
            public Color OuterColor;
            public Color EdgeColor;

            public const string CENTER_COLOR = "_CenterColor";
            public const string OUTER_COLOR = "_OuterColor";
            public const string EDGE_COLOR = "_EdgeColor";

            public void ApplyTo(Material material)
            {
                material.SetColor(EDGE_COLOR, EdgeColor);
                material.SetColor(OUTER_COLOR, OuterColor);
                material.SetColor(CENTER_COLOR, CenterColor);
            }
        }


        protected PlayerComponents _player;
        protected PlayerCrosshairController _crosshair;
        protected Transform _playerTransform;
        protected Transform _crosshairTransform;
        private Material _groundLineMaterial;

        private List<float> _pointsDistribution = new List<float>();
        private List<float> _heightPoints = new List<float>();
        private Vector3 _originalGroundLineScale;
        private Vector2 _indicatorAimRadiusRange;
        private float _indicatorSpeedMultiplier = 0.5f;
        private float _lerpSpeed = 0.5f;
        private bool _controlCrosshairVisuals;

        // modifiers
        private bool _avoidWalls;
        private bool _isCrosshairTheParent = true;

        // ground line
        private bool _isGroundTrajectoryDisplayed = true;

        private const float MIN_HEIGHT = 0.5f;

        public void SetUp(PlayerComponents player)
        {
            _player = player;
            _crosshair = player.CrosshairController;
            _playerTransform = player.transform;
            _crosshairTransform = _crosshair.GetCrosshairTransform();

            _line.enabled = false;
            _indicatorEnd.localScale = Vector3.zero;

            // we cache the points at which the line forms curves to avoid calculating these every frame at runtime
            for (int i = 0; i < _levelOfDetail; i++)
            {
                float t = (float)i / ((float)_levelOfDetail - 1f);
                _pointsDistribution.Add(t);
            }

            _originalGroundLineScale = _groundLine.localScale;

            _groundLineMaterial = _groundLineMeshRenderer.material;

            ApplyColorToComponents(_inactiveColor, _groundLineInactivePreset);
        }

        public void SetValues(IndicatorSettings settings)
        {
            _indicatorEnd.transform.localScale = new Vector3(settings.EndSize * 2, 1f, settings.EndSize * 2);
            _indicatorAimRadiusRange = settings.AimRange;
            _indicatorSpeedMultiplier = settings.AimSpeedMultiplier;
            _lineState = settings.LineState;

            if (_lineState == IndicatorLineState.AirTrajectory)
            {
                _lineHeight = settings.LineHeight;
                _line.widthMultiplier = settings.AirLineWidth;

                _heightPoints.Clear();

                // cache the height points to avoid doing it every frame
                for (int i = 0; i < _levelOfDetail; i++)
                {
                    float t = (float)i / ((float)_levelOfDetail - 1f);
                    _heightPoints.Add(MIN_HEIGHT + _curve.Evaluate(t) * _lineHeight);
                }
            }

            _avoidWalls = settings.AvoidWalls;

            _isCrosshairTheParent = settings.IsCrosshairTheParent;

            _controlCrosshairVisuals = settings.ShowCrosshairOnEnd;
        }

        #region Start
        public void StartIndicator()
        {
            CancelInvoke();

            StartImpactPoint();

            PlayAnimation(_startAnimationClip);

            StartCrossHair();

            StartAirTrajectory();

            StartGroundTrajectory();

            _indicatorState = IndicatorState.Active;
        }

        private void StartImpactPoint()
        {
            if (_isCrosshairTheParent)
            {
                _indicatorEnd.parent = _crosshairTransform;
            }
            else
            {
                _indicatorEnd.parent = _playerTransform;
            }

            _indicatorEnd.localPosition = Vector3.zero;
            Vector3 position = _indicatorEnd.position;
            position.y = Constants.GroundHeight;
            _indicatorEnd.position = position;
        }

        private void StartCrossHair()
        {
            if (_controlCrosshairVisuals)
            {
                _crosshair.EnableCrosshair(false);
            }

            _crosshair.SetAimRange(_indicatorAimRadiusRange);
            _crosshair.SetCrosshairSpeedMultiplier(_indicatorSpeedMultiplier);
        }

        private void StartAirTrajectory()
        {
            if (_lineState == IndicatorLineState.AirTrajectory)
            {
                _line.enabled = true;
            }
        }

        private void StartGroundTrajectory()
        {
            if (_lineState == IndicatorLineState.GroundTrajectory)
            {
                _isGroundTrajectoryDisplayed = true;
                _groundLine.parent = _playerTransform;
                _groundLine.localScale = _originalGroundLineScale;
                Vector3 grounLinePosition = new Vector3(_playerTransform.position.x, Constants.GroundHeight, _playerTransform.position.z);
                _groundLine.position = grounLinePosition;

                this.PlayAnimation(_groundLineAnimation, _groundLineStartClip);
            }
        }
        #endregion

        #region Update
        public void UpdateIndicator()
        {
            UpdateGroundTrajectory();

            UpdateAirTrajectory();
        }

        private void UpdateGroundTrajectory()
        {
            if (_lineState != IndicatorLineState.GroundTrajectory)
            {
                return;
            }

            Vector3 direction = _crosshairTransform.position - _playerTransform.position;
            direction.y = 0f;
            _groundLine.forward = direction;

            if (Physics.Raycast(_playerTransform.position, direction, out RaycastHit hit, _indicatorAimRadiusRange.y, _obstructionLayerMask))
            {
                Vector3 scale = _groundLine.localScale;
                float distance = Vector3.Distance(_playerTransform.position, hit.point) - Constants.OffsetToWalls;
                scale.z = Mathf.Lerp(scale.z, distance, _lerpSpeed);
                _groundLine.localScale = scale;
            }
            else
            {
                Vector3 scale = _groundLine.localScale;
                scale.z = Mathf.Lerp(scale.z, _indicatorAimRadiusRange.y, _lerpSpeed);
                _groundLine.localScale = scale;
            }
        }

        private void UpdateAirTrajectory()
        {
            if (_lineState != IndicatorLineState.AirTrajectory)
            {
                return;
            }

            _line.positionCount = _levelOfDetail;
            Vector3 startPoint = _indicatorEnd.position;
            Vector3 endPoint = _player.transform.position;

            for (int i = 0; i < _levelOfDetail; i++)
            {
                float t = _pointsDistribution[i];
                float x = Mathf.Lerp(startPoint.x, endPoint.x, t);
                float z = Mathf.Lerp(startPoint.z, endPoint.z, t);
                Vector3 position = new Vector3(x, _heightPoints[i], z);
                _line.SetPosition(i, position);
            }
        }
        #endregion

        #region End
        public void EndIndicator()
        {
            if(_indicatorState == IndicatorState.Inactive)
            {
                return;
            }

            _indicatorState = IndicatorState.Inactive;

            PlayAnimation(_finishAnimationClip);
            _indicatorEnd.transform.parent = null;
            _crosshair.ResetAimRange();

            if (_controlCrosshairVisuals)
            {
                _crosshair.EnableCrosshair(true);
            }

            _crosshair.ResetCrosshairSpeedMultiplier();

            EndAirTrajectory();

            EndGroundTrajectory();
        }

        private void EndAirTrajectory()
        {
            if (_lineState != IndicatorLineState.AirTrajectory)
            {
                return;
            }

            _line.enabled = false;
        }

        private void EndGroundTrajectory()
        {
            if (_lineState != IndicatorLineState.GroundTrajectory || !_isGroundTrajectoryDisplayed)
            {
                return;
            }

            _isGroundTrajectoryDisplayed = false;
            this.PlayAnimation(_groundLineAnimation, _groundLineEndClip);
            Invoke(nameof(SetGroundLineParentToNull), _groundLineEndClip.length);
        }

        private void SetGroundLineParentToNull()
        {
            _groundLine.parent = null;
        }
        #endregion

        #region Coloring
        public void SetActiveColor()
        {
            ApplyColorToComponents(_activeColor, _groundLineActivePreset);
        }

        public void SetInactiveColor()
        {
            ApplyColorToComponents(_inactiveColor, _groundLineInactivePreset);
        }

        private void ApplyColorToComponents(Color color, IndicatorVisualsPreset groundLinePreset)
        {
            _endPointSprite.color = color;
            Gradient gradient = _line.colorGradient;
            GradientColorKey[] colorKeys = gradient.colorKeys;

            for (int i = 0; i < colorKeys.Length; i++)
            {
                colorKeys[i].color = color;
            }

            gradient.colorKeys = colorKeys;
            _line.colorGradient = gradient;

            if (groundLinePreset != null && _groundLineMaterial != null)
            {
                groundLinePreset.ApplyTo(_groundLineMaterial);
            }
        }
        #endregion

        private void PlayAnimation(AnimationClip clip)
        {
            if (_animation.isPlaying)
            {
                _animation.Stop();
            }

            _animation.clip = clip;
            _animation.Play();
        }

        #region Pool
        public void Init(Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }

        public void OnRequest()
        {

        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
            GameManager.Instance.SetParentToSpawnables(gameObject);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
