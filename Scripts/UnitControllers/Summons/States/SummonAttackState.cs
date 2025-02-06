using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class SummonAttackState : IState
    {
        private StateMachine<SummonStateType> _stateMachine;
        private SummonMovement _movement;
        private SummonShooter _shooter;

        private EnemyComponents _target;
        private float _stopDistance;
        private float _timer;

        public SummonAttackState(StateMachine<SummonStateType> stateMachine, SummonMovement movement, SummonShooter shooter, float stopDistance)
        {
            _stateMachine = stateMachine;
            _movement = movement;
            _shooter = shooter;

            _stopDistance = stopDistance;

            _movement.OnReachedTarget += OnReachedTargetHandler;
        }

        public void OnEnter()
        {
            //Debug.Log("ATTACK");
            _target = _shooter.GetClosestTarget();
            if(_target == null)
            {
                _stateMachine.ChangeState(SummonStateType.FOLLOW);
            }
        }

        public void OnUpdate()
        {
            if(_target != null)
            {
                _movement.MoveToTarget(_target.transform, _stopDistance);
            }

        }

        public void OnExit()
        {
        }

        public void OnDispose()
        {

        }

        private void OnReachedTargetHandler()
        {
            if(_target != null)
                _shooter.ShootTarget(_target.transform);

            _stateMachine.ChangeState(SummonStateType.STANDBY);
        }
    }
}
