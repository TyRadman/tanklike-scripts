using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.UnitControllers
{
    public class EnemyMovement : TankMovement
    {
        public System.Action OnTargetReached;
        public System.Action OnPathPointReached;

        [Header("Testing")]
        [SerializeField] private Testing.AIMouseTarget _mouseTarget;
        private bool _debugMode;

        [Header("Navigation")]
        [SerializeField] private bool _obstacleAvoidance = true;
        [SerializeField] private float _stoppingDistance = 0.1f;
        [SerializeField] private float _decelerationDistance = 1f;
        [SerializeField] private float _reverseDistance = 6f;
        [SerializeField] private float _obstacleDetectionDistance = 3f;
        [SerializeField] private LayerMask _obstacleLayers;
        [SerializeField] private float _pathUpdateTime = 1f;
        [SerializeField] private float _forcePathResetTime = 4f;

        [Header("Sensors")]
        [SerializeField] private float _sideSensorOffset = 1f;
        [SerializeField] private float _sensorVerticalOffset = 0.5f;

        public bool TargetIsAvailable { get; private set; }

        protected bool _isMoving = false;
        protected bool _isSlowingDown = false;
        protected Vector3 _targetPosition;
        protected Vector3 _targetDirection;
        protected int _currentPathIndex = -1;
        protected float _pathUpdateTimer;
        protected NavMeshPath _path;
        protected List<Vector3> _points = new List<Vector3>();

        //Obstales detection
        private Vector3 _obstacleNormal;
        private Vector3 _nextPoint;

        private bool _hitFrontObstacle;
        private bool _directionIsDetermined;
        private bool _faceTargetPoint;
        private bool _isfaceTargetDirectionDetermined;
        private bool _criticalObstacleMovement;

        private bool _frontCenterObstacle;
        private bool _frontRightObstacle;
        private bool _frontLeftObstacle;
        private bool _frontDiagnoalRightObstacle;
        private bool _frontDiagnoalLeftObstacle;
        private bool _nextPointObastacle;
        private bool _finalPointObastacle;
        private float _distanceToTarget;

        private Coroutine _forcePathResetCoroutine;

        public override void SetUp(IController controller)
        {
            if (controller is not TankComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            base.SetUp(controller);

            _components = components;

            if (_mouseTarget != null)
            {
                _mouseTarget.OnTargetChanged += OnTargetChangedHandler;
            }
        }

        public void SetDebugMode(bool value)
        {
            _debugMode = value;
         
            if (_mouseTarget != null)
            {
                _mouseTarget.gameObject.SetActive(_debugMode);
            }
        }

        private void OnTargetChangedHandler()
        {
            if (!_debugMode || _isMoving)
            {
                return;
            }

            SetTargetPosition(_mouseTarget.transform.position);
        }

        private void Update()
        {
            if (!IsActive)
            {
                return;
            }

            if (_isMoving)
            {
                DetectObstacles();
                MoveToTarget();
                DrawDebugLines();

                if(_faceTargetPoint)
                {
                    FaceTargetPoint();
                    return;
                }

                UpdatePathToTarget();
            }

            ApplyMovement();
        }

        private void ApplyMovement()
        {
            // Apply movement base on movement type
            bool isMovingWithObstaclesDetected = _isMoving && (_hitFrontObstacle || _frontDiagnoalLeftObstacle || _frontDiagnoalRightObstacle);

            if (isMovingWithObstaclesDetected)
            {
                ObstacleMovement();
            }
            else
            {
                MoveCharacterController(_targetDirection);
            }
        }

        protected void FaceTargetPoint()
        {
            Vector3 point;

            if(_currentPathIndex < _points.Count - 1)
            {
                //Debug.Log("Point is next point " + _currentPathIndex + 1);
                point = _points[_currentPathIndex + 1];
            }
            else
            {
                //Debug.Log("Point is last point " + _points[_points.Count - 1]);
                point = _points[_points.Count - 1];
            }

            Vector3 nextPointDir = point - transform.position;
            nextPointDir.y = 0f;
            nextPointDir.Normalize();

            Vector3 forward = _body.forward;
            forward.y = 0f;
            forward.Normalize();

            // Determine the turn direction based on the angle to the last point direction (right or left)
            if (!_isfaceTargetDirectionDetermined)
            {
                _forwardAmount = 0;
                _turnAmount = GetTurnAmountFromAngle(forward, nextPointDir);

                _isfaceTargetDirectionDetermined = true;

                //Debug.Log("Angle " + angleToTarget);
                //Debug.Log("Turn amount is " + _turnAmount);
            } 
           
            ObstacleMovement();

            float dot = Vector3.Dot(forward, nextPointDir);

            //Debug.Log("dot " + dot);

            if (dot >= 0.99)
            {
                _faceTargetPoint = false;

                if (_forcePathResetCoroutine != null)
                {
                    StopCoroutine(_forcePathResetCoroutine);
                }
            }
        }

        protected void UpdatePathToTarget()
        {
            if(_pathUpdateTimer >= _pathUpdateTime)
            {
                RecalculatePath();
                _pathUpdateTimer = 0f;
            }

            _pathUpdateTimer += Time.deltaTime;
        }

        public void SetTargetPosition(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;

            StartMovement();
        }

        public void StartMovement()
        {
            _pathUpdateTimer = 0f;

            _distanceToTarget = Vector3.Distance(transform.position, _targetPosition);

            RecalculatePath();

            TargetIsAvailable = true;
            _directionIsDetermined = false;
            _isMoving = true;

            OnPathPointReachedHandler();
        }

        protected void RecalculatePath()
        {
            _path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, _targetPosition, NavMesh.AllAreas, _path);

            //Debug.Log("Path length = " + _path.corners.Length);
            if (_path.corners.Length > 0)
            {
                _currentPathIndex = 0;
                _nextPoint = _path.corners[0];
            }

            _points = new List<Vector3>(_path.corners);
        }

        protected bool IsDestinationReachable(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);

            return path.status != NavMeshPathStatus.PathPartial;
        }

        public float GetPathLength(NavMeshPath path)
        {
            float length = 0f;

            if (path.corners.Length < 2)
            {
                return length;
            }

            for (int i = 1; i < path.corners.Length; i++)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            return length;
        }

        public float GetCurrentSpeed()
        {
            return CurrentSpeed;
        }

        protected void MoveToTarget()
        {
            if(_criticalObstacleMovement || _faceTargetPoint)
            {
                return;
            }

            if (!_directionIsDetermined)
            {
                DetermineMovementDirection();
                return;
            }

            float distanceToPoint = Vector3.Distance(transform.position, _nextPoint);

            //Debug.Log("DISTANCE -> " + distanceToPoint);
            if (distanceToPoint > _stoppingDistance)
            {
                _targetDirection = _nextPoint - transform.position;
                _targetDirection.y = 0;
                _targetDirection.Normalize();

                float dotToPoint = Vector3.Dot(transform.forward, _targetDirection);

                // if the last point is on the side of the unit, and is too close, consider point reached
                if(dotToPoint >= -0.1f && dotToPoint <= 0.1f && distanceToPoint <= 2f && _currentPathIndex >= _points.Count - 1)
                {
                    //Debug.Log("YO");
                    OnPathEndReached();
                    return;
                }

                //// if the last point is near the wall, consider point reached if the movement type is obstacle movement and the distance is less than the obstacle detection distance
                //if (distanceToPoint < _obstacleDetectionDistance && _currentPathIndex >= _points.Count - 1)
                //{
                //    Debug.Log("YO2");
                //    OnPathEndReached();
                //    return;
                //}

                // If front obstacles detected
                if (_hitFrontObstacle)
                {
                    // if the last point is near the wall, consider point reached if the distance is less than the obstacle detection distance
                    if (distanceToPoint < _obstacleDetectionDistance && _currentPathIndex >= _points.Count - 1)
                    {
                        //Debug.Log("YO2");
                        OnPathEndReached();
                        return;
                    }

                    // If the 3 front sensors detect an obstacle, start turning right 
                    if (_frontCenterObstacle && _frontRightObstacle && _frontLeftObstacle)
                    {
                        // If the player reached a corner, do a reset 
                        if(_frontDiagnoalLeftObstacle && _frontDiagnoalRightObstacle && distanceToPoint > _obstacleDetectionDistance)
                        {
                            _criticalObstacleMovement = true;
                            StartCoroutine(CriticalObstacleMovementRoutine());
                            return;
                        }
                        else
                        {
                            _turnAmount = GetTurnAmountFromAngle(_body.forward, _targetDirection); // TODO: choose the direction based on the angle to the next point
                            //Debug.Log("_turnAmount " + _turnAmount);

                        }
                    }
                    // If both the center and left sensors detect an obstacle, start turning right
                    else if (_frontCenterObstacle && _frontLeftObstacle)
                    {
                        _forwardAmount = 1;
                        _turnAmount = 1;
                    }
                    // If both the center and right sensors detect an obstacle, start turning left
                    else if (_frontCenterObstacle && _frontRightObstacle)
                    {
                        _forwardAmount = 1;
                        _turnAmount = -1;
                    }
                    // If only the center sensor detects the obstacle, find the angle to determine the turn direction
                    else if (_frontCenterObstacle)
                    {
                        // TODO: use the angle to the next point instead
                        // Calculate the angle between the forward direction and obstacle normal
                        float angle = Vector3.SignedAngle(_body.forward, _obstacleNormal, Vector3.up);

                        //// Determine if the obstacle is to the left or right
                        if (angle > 0)
                        {
                            _turnAmount = 1; // Turn right
                        }
                        else
                        {
                            _turnAmount = -1; // Turn left
                        }        
                    }
                    // If only the left sensor detects the obstacle, stop moving and turn right
                    else if (_frontLeftObstacle && !_frontRightObstacle && !_frontCenterObstacle)
                    {
                        _forwardAmount = 1;
                        _turnAmount = 1;
                    }
                    // If only the left sensor detects the obstacle, stop moving and turn left
                    else if (!_frontLeftObstacle && _frontRightObstacle && !_frontCenterObstacle)
                    {
                        _forwardAmount = 1;
                        _turnAmount = -1;
                    }
                    // If both the left and right sensors detect the obstacle, move forward
                    else if (_frontLeftObstacle && _frontRightObstacle && !_frontCenterObstacle)
                    {
                        _turnAmount = 0;
                        _forwardAmount = 1;
                        _lastForwardAmount = 1;
                    }        
                }
                // If no front obstacles detected
                else
                {
                    // If no obstacles, just move forward
                    _forwardAmount = 1;
                    _lastForwardAmount = 1;

                    // If there are obstacles on the sides, don't turn
                    if (_frontDiagnoalLeftObstacle || _frontDiagnoalRightObstacle)
                    {
                        _turnAmount = 0;
                    }

                    float targetAngle = Vector3.SignedAngle(_body.forward, _targetDirection * _forwardAmount, Vector3.up);

                    // Keep the tank moving in a straight line to the target point
                    if (targetAngle > 1f)
                    {
                        if (!_frontDiagnoalRightObstacle && _frontDiagnoalLeftObstacle)
                        {
                            //_forwardAmount = 0;
                            _turnAmount = 1;
                        }
                    }
                    else if (targetAngle < -1f)
                    {
                        if (!_frontDiagnoalLeftObstacle && _frontDiagnoalRightObstacle)
                        {
                            //_forwardAmount = 0;
                            _turnAmount = -1;
                        }
                    }
                }
            }
            else
            {
                // If reached last point in the path
                if (_currentPathIndex >= _path.corners.Length - 1)
                {
                    OnPathEndReached();
                }
                else
                {
                    // Update next point in the path
                    _currentPathIndex++;
                    _nextPoint = _points[_currentPathIndex];
                    OnPathPointReached?.Invoke();
                }
            }
        }

        private void DetermineMovementDirection()
        {
            if (_points.Count <= 1)
            {
                //Debug.Log("Path has one point");
                return;
            }

            if (_obstacleAvoidance)
            {
                float distanceToPoint = Vector3.Distance(transform.position, _nextPoint);

                //Debug.Log("DISTANCE -> " + distanceToPoint);
                if (distanceToPoint > _stoppingDistance)
                {
                    _faceTargetPoint = true;
                    _isfaceTargetDirectionDetermined = false;
                    //OnPathPointReachedHandler();
                }
            }

            // If the tank was not moving, do the wiggle
            if (_forwardAmount != 1)
            {
                if (_components.TankWiggler != null)
                {
                    _components.TankWiggler.WiggleBody(_backwardWiggle);
                }
            }

            _directionIsDetermined = true;
        }

        private int GetTurnAmountFromAngle(Vector3 forwardDirection, Vector3 TargetDirection)
        {
            forwardDirection.y = 0f;
            forwardDirection.Normalize();

            TargetDirection.y = 0f;
            TargetDirection.Normalize();

            // Calculate angle between forward direction and target direction
            float angleToTarget = Vector3.SignedAngle(forwardDirection, TargetDirection, Vector3.up);

            return angleToTarget > 0 ? 1: -1; // 1 = right, -1 = left         
        }

        public void OnPathEndReached(bool PerformWiggle = true)
        {
            _forwardAmount = 0;
            _turnAmount = 0;
            _isMoving = false;
            _directionIsDetermined = false;
            _currentPathIndex = 0;
            OnTargetReached?.Invoke();
            TargetIsAvailable = false;

            if (_components.TankWiggler != null && PerformWiggle)
            {
                _components.TankWiggler.WiggleBody(_forwardWiggle);
            }

            if (_forcePathResetCoroutine != null)
            {
                StopCoroutine(_forcePathResetCoroutine);
            }
        }

        private IEnumerator CriticalObstacleMovementRoutine()
        {
            //Debug.Log("Critical");

            // Start path reset timer
            //OnPathPointReachedHandler();

            Vector3 point;

            if (_currentPathIndex < _points.Count - 1)
            {
                //Debug.Log("Point is next point " + _currentPathIndex + 1);
                point = _points[_currentPathIndex + 1];
            }
            else
            {
                //Debug.Log("Point is last point " + _points[_points.Count - 1]);
                point = _points[_points.Count - 1];
            }

            Vector3 nextPointDir = point - transform.position;
            nextPointDir.y = 0f;
            nextPointDir.Normalize();

            Vector3 forward = _body.forward;
            forward.y = 0f;
            forward.Normalize();    

            float dot = Vector3.Dot(forward, nextPointDir);

            while (dot <= 0.97)
            {
                nextPointDir = point - transform.position;
                nextPointDir.y = 0f;
                nextPointDir.Normalize();

                forward = _body.forward;
                forward.y = 0f;
                forward.Normalize();

                dot = Vector3.Dot(forward, nextPointDir);

                // Determine the turn direction based on the angle to the last point direction (right or left)  
                float angleToTarget = Vector3.SignedAngle(forward, nextPointDir, Vector3.up);
                _turnAmount = angleToTarget > 0 ? 1 : -1; // 1 = right, -1 = left
                _forwardAmount = 0;

                yield return null;
            }

            _criticalObstacleMovement = false;
        }

        private void DetectObstacles()
        {
            #region Front Sensors
            // Origins
            Vector3 centerSensorOrigin = transform.position + Vector3.up * _sensorVerticalOffset;
            Vector3 rightSensorOrigin = transform.position + _body.right * _sideSensorOffset + Vector3.up * _sensorVerticalOffset;
            Vector3 leftSensorOrigin = transform.position - _body.right * _sideSensorOffset + Vector3.up * _sensorVerticalOffset;

            //front-center
            _obstacleNormal = PerformRaycastCheck(centerSensorOrigin, _body.forward, _obstacleDetectionDistance, ref _frontCenterObstacle, Color.green).normal;

            //front-right
            PerformRaycastCheck(rightSensorOrigin, _body.forward, _obstacleDetectionDistance, ref _frontRightObstacle, Color.green);

            //front-left
            PerformRaycastCheck(leftSensorOrigin, _body.forward, _obstacleDetectionDistance, ref _frontLeftObstacle, Color.green);

            ////DIAGONAL
            //front-diagonal-right
            PerformRaycastCheck(rightSensorOrigin, (_body.forward + _body.right * 0.5f).normalized, _obstacleDetectionDistance, ref _frontDiagnoalRightObstacle, Color.green);

            //front-diagonal-left
            PerformRaycastCheck(leftSensorOrigin, (_body.forward - _body.right * 0.5f).normalized, _obstacleDetectionDistance, ref _frontDiagnoalLeftObstacle, Color.green);

            // Check if there are any obstacles detected
            if (_frontCenterObstacle || _frontLeftObstacle || _frontRightObstacle)
            {
                _hitFrontObstacle = true;
            }
            else
            {
                _hitFrontObstacle = false;
            }
            #endregion

            #region Path Points obstacles
            // Origins
            Vector3 rightPointOrigin = transform.position + _body.right + Vector3.up * _sensorVerticalOffset;
            Vector3 leftPointOrigin = transform.position - _body.right + Vector3.up * _sensorVerticalOffset;

            #region Next Point in Path
            // Right point
            var nextPointRightDir = _nextPoint - (_body.position + _body.right);
            nextPointRightDir.y = 0f;
            nextPointRightDir.Normalize();

            float nextPointRightDistance = Vector3.Distance(_body.position + _body.right, _nextPoint);

            PerformRaycastCheck(rightPointOrigin, nextPointRightDir, nextPointRightDistance, ref _nextPointObastacle, Color.cyan);

            // Left point
            var nextPointLeftDir = (_nextPoint - (_body.position - _body.right));
            nextPointLeftDir.y = 0f;
            nextPointLeftDir.Normalize();

            float nextPointLeftDistance = Vector3.Distance(_body.position - _body.right, _nextPoint);

            PerformRaycastCheck(leftPointOrigin, nextPointLeftDir, nextPointLeftDistance, ref _nextPointObastacle, Color.cyan);
            #endregion

            #region Final Point in Path
            // target_right
            var finalPointRightDir = (_targetPosition - (_body.position + _body.right));
            finalPointRightDir.y = 0f;
            finalPointRightDir.Normalize();

            float lastPointRightDistance = Vector3.Distance(_body.position + _body.right, _targetPosition);

            PerformRaycastCheck(rightPointOrigin, finalPointRightDir, lastPointRightDistance, ref _finalPointObastacle, Color.yellow);

            //target_left
            var finalPointLeftDir = _targetPosition - (_body.position - _body.right);
            finalPointLeftDir.y = 0f;
            finalPointLeftDir.Normalize();

            float lastPointLeftDistance = Vector3.Distance(_body.position - _body.right, _targetPosition);

            PerformRaycastCheck(leftPointOrigin, finalPointLeftDir, lastPointLeftDistance, ref _finalPointObastacle, Color.yellow);
            #endregion
            #endregion
        }

        private RaycastHit PerformRaycastCheck(Vector3 origin, Vector3 direction, float distance, ref bool hitFlag, Color negativeColor)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, _obstacleLayers))
            {
                if (hit.collider.transform.root != transform)
                {
                    hitFlag = true;
                    Debug.DrawRay(origin, direction * distance, Color.red);
                }
            }
            else
            {
                hitFlag = false;
                Debug.DrawRay(origin, direction * distance, negativeColor);
            }

            return hit;
        }

        private void DrawDebugLines()
        {
            #region Path     
            // Path lines
            for (int i = 0; i < _path.corners.Length - 1; i++)
            {
                _path.corners[i].y = _sensorVerticalOffset;
                _path.corners[i + 1].y = _sensorVerticalOffset;

                Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.blue);
            }
            #endregion
        }

        public override void StopMovement()
        {
            base.StopMovement();
            _isMoving = false;

            if (_targetDirection.magnitude > 0)
            {
                _isSlowingDown = true;
            }

            if (_components.TankWiggler != null)
            {
                _components.TankWiggler.WiggleBody(_forwardWiggle);
            }

            _forwardAmount = 0;
        }

        private void ResetObstacleDetection()
        {
            _hitFrontObstacle = false;

            _frontCenterObstacle = false;
            _frontRightObstacle = false;
            _frontLeftObstacle = false;
            _frontDiagnoalRightObstacle = false;
            _frontDiagnoalLeftObstacle = false;
            _nextPointObastacle = false;
            _finalPointObastacle = false;
            _faceTargetPoint = false;
            _isfaceTargetDirectionDetermined = false;
            _criticalObstacleMovement = false;

            _obstacleNormal = Vector3.zero;
        }

        private void OnPathPointReachedHandler()
        {
            if (_forcePathResetCoroutine != null)
            {
                StopCoroutine(_forcePathResetCoroutine);
            }

            _forcePathResetCoroutine = StartCoroutine(ForcePathResetRoutine());
        }

        private IEnumerator ForcePathResetRoutine()
        {
            float timer = 0f;

            //Debug.Log("Start force Path Reset Routine");

            while (timer < _forcePathResetTime)
            {
                //Debug.Log("Timer " + timer + " " + gameObject.name);
                timer += Time.deltaTime;
                yield return null;
            }

            //Debug.Log("Force Path Reset Routine");
            OnPathEndReached();
        }


        #region IController
        public override void Activate()
        {
            base.Activate();
        }

        public override void Dispose()
        {
            base.Dispose();
            //OnPathPointReached -= OnPathPointReachedHandler;
        }

        public override void Restart()
        {
            base.Restart();

            _targetPosition = Vector3.zero;
            _targetDirection = Vector3.zero;
            _currentPathIndex = 0;
            _points.Clear();

            ResetObstacleDetection();

            _directionIsDetermined = false;
            _distanceToTarget = 0;
            _nextPoint = Vector3.zero;

            //OnPathPointReached += OnPathPointReachedHandler;

            StopAllCoroutines();
        }
        #endregion
    }
}
