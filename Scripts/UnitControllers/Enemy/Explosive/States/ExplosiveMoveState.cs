using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Explosive_Move", menuName = MENU_PATH + "Explosive/Move State")]
    public class ExplosiveMoveState : MoveState
    {
        private float _pathUpdateTime = 0.25f;
        private float _pathUpdateTimer;

        private ExplosiveShooter _shooter;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            _shooter = (ExplosiveShooter)enemyComponents.Shooter;
            _shooter.OnExplosionTriggered += OnExplosionTriggeredHandler;
        }

        public override void OnEnter()
        {
            _isActive = true;
            _pathUpdateTimer = 0f;
        }

        public override void OnUpdate()
        {
            _pathUpdateTimer += Time.deltaTime;

            if(_pathUpdateTimer >= _pathUpdateTime)
            {
                _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);
                MoveToTarget(_target.position);
                _pathUpdateTimer = 0f;
            }
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
            _shooter.OnExplosionTriggered -= OnExplosionTriggeredHandler;
        }

        protected override void MoveToTarget(Vector3 target)
        {
            NavMeshHit hit;

            if (NavMesh.SamplePosition(target, out hit, 1.0f, NavMesh.AllAreas))
            {
                _targetPoint = hit.position;
                _movement.SetTargetPosition(_targetPoint);
            }
        }

        private void OnExplosionTriggeredHandler()
        {
            if (!_isActive)
            {
                return;
            }

            _stateMachine.ChangeState(EnemyStateType.ATTACK);
        }
    }
}
