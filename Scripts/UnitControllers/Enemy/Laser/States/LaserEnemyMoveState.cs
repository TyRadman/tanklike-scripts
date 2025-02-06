using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Laser_Move", menuName = MENU_PATH + "Laser/Move State")]
    public class LaserEnemyMoveState : MoveState
    {
        [SerializeField] private Vector2 _movementDistanceRange;
        [SerializeField] private float _targetApproachChance;
        [SerializeField] private float _targetApproachDistance = 6f;
        [SerializeField] private float _approachStopDistance = 2f;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);
            _movement.OnTargetReached += OnTargetReachedHandler;
        }

        public override void OnEnter()
        {
            //Debug.Log("MOVE STATE");
            base.OnEnter();

            _isActive = true;

            _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);

            _pointFinderCounter = 0;
            _targetPointFound = false;

            float approachChance = Random.Range(0f, 1f);

            while (!_targetPointFound && _pointFinderCounter < 50)
            {
                Vector3 point;

                float distanceToTarget = Vector3.Distance(_movement.transform.position, _target.position);

                bool moveRandomly = distanceToTarget < _targetApproachDistance || _targetApproachChance > approachChance;

                // If the target is so close, move to a random point
                if (moveRandomly)
                {
                    // Move in a random directions
                    MoveToARandomPoint();
                }
                else
                {
                    // Move towards the player
                    //Debug.Log(_movement.gameObject.name + " is moving towards target");
                    point = _target.position;
                    point.y = 0.5f;

                    // Find the max distance that can be travelled
                    float maxDistance = distanceToTarget < _targetApproachDistance ? distanceToTarget - _approachStopDistance : _targetApproachDistance;
                    //maxDistance = Mathf.Clamp(maxDistance, _movementDistanceRange.x, _movementDistanceRange.y);

                    MoveToTargetWithDistance(point, maxDistance);
                }

                _pointFinderCounter++;
            }

            if (!_targetPointFound)
            {
                _stateMachine.ChangeState(EnemyStateType.AIM);
            }
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
            _movement.OnTargetReached -= OnTargetReachedHandler;
        }

        private void MoveToARandomPoint()
        {
            //Debug.Log(_movement.gameObject.name + " is moving in a random direction");
            float angle = Random.Range(0.0f, Mathf.PI * 2);
            Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            dir *= Random.Range(_movementDistanceRange.x, _movementDistanceRange.y);
            dir.y = 0.5f;

            MoveToTarget(_movement.transform.position + dir);
        }

        private void OnTargetReachedHandler()
        {
            if (!_isActive)
                return;

            _stateMachine.ChangeState(EnemyStateType.AIM);
        }
    }
}
