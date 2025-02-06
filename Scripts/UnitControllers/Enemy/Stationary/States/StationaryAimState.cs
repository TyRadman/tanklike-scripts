using TankLike.UnitControllers;
using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class StationaryAimState : IState
    {
        private StateMachine<EnemyStateType> _stateMachine;
        private EnemyMovement _movement;
        private EnemyShooter _shooter;

        private bool _isActive;

        private Transform _target;
        private int _rotationDirection;

        private Vector2 _idleDurationRange;
        private float _idleDuration;
        private float _idleTimer;

        public StationaryAimState(StateMachine<EnemyStateType> stateMachine, EnemyMovement movement, EnemyShooter shooter,
            Vector2 aimDurationRange)
        {
            _stateMachine = stateMachine;
            _movement = movement;
            _shooter = shooter;
            _idleDurationRange = aimDurationRange;
        }

        public void OnEnter()
        {
            //Debug.Log("AIM STATE");
            _isActive = true;

            //set random values for state parameters
            _idleDuration = Random.Range(_idleDurationRange.x, _idleDurationRange.y);
            _idleTimer = 0f;
        }

        public void OnUpdate()
        {
            //switch to ATTACK state when the timer is up
            if (_idleTimer >= _idleDuration)
            {
                _stateMachine.ChangeState(EnemyStateType.ATTACK);
            }

            _idleTimer += Time.deltaTime;
        }

        public void OnExit()
        {
            _isActive = false;
        }

        public void OnDispose()
        {
        }
    }
}
