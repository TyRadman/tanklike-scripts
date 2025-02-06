using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.UnitControllers.States
{
    public class MoveState : State
    {
        [SerializeField] protected float _maxPathLength = 10f;

        protected bool _isActive;
        protected EnemyMovement _movement;
        protected EnemyTurretController _turretController;

        protected Transform _target;
        protected Vector3 _targetPoint;
        protected bool _targetPointFound;
        protected int _pointFinderCounter;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            _movement = (EnemyMovement)enemyComponents.Movement;
            _turretController = (EnemyTurretController)enemyComponents.TurretController;
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        protected virtual void MoveToTarget(Vector3 target)
        {
            NavMeshHit hit;

            if (NavMesh.SamplePosition(target, out hit, 10.0f, NavMesh.AllAreas))
            {
                _targetPoint = hit.position;

                // Check for path length
                NavMeshPath path = new NavMeshPath();
                if(NavMesh.CalculatePath(_movement.transform.position, _targetPoint, NavMesh.AllAreas, path))
                {
                    float pathLength = _movement.GetPathLength(path);

                    if (pathLength < _maxPathLength)
                    {
                        _targetPointFound = true;
                        _movement.SetTargetPosition(_targetPoint);
                    }
                    else
                    {
                        //Debug.Log($"Path length {pathLength} is too big!");
                    }
                }               
            }
        }

        protected virtual void MoveToTargetWithDistance(Vector3 target, float maxDistance)
        {
            NavMeshHit hit;

            // Find the closest position on the NavMesh to the target
            if (NavMesh.SamplePosition(target, out hit, 10.0f, NavMesh.AllAreas))
            {
                _targetPoint = hit.position;

                // Calculate the path to the target
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(_movement.transform.position, _targetPoint, NavMesh.AllAreas, path))
                {
                    float accumulatedLength = 0f;
                    Vector3[] corners = path.corners;

                    // Traverse the path to find the point that’s within the maxDistance
                    for (int i = 1; i < corners.Length; i++)
                    {
                        float segmentLength = Vector3.Distance(corners[i - 1], corners[i]);
                        accumulatedLength += segmentLength;

                        if (accumulatedLength >= maxDistance)
                        {
                            // Calculate a point on this segment that is exactly maxDistance away
                            float excessDistance = accumulatedLength - maxDistance;
                            Vector3 direction = (corners[i - 1] - corners[i]).normalized;
                            Vector3 pointAlongPath = corners[i] + direction * excessDistance;

                            // Set target point to the calculated position along the path
                            _targetPoint = pointAlongPath;
                            _targetPointFound = true;
                            _movement.SetTargetPosition(_targetPoint);
                            return;
                        }
                    }

                    // If maxPathLength is longer than the full path, move to the end of the path
                    _targetPointFound = true;
                    _movement.SetTargetPosition(_targetPoint);
                }
            }
        }
    }
}
