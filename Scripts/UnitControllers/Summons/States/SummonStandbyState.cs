using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class SummonStandbyState : IState
    {
        private StateMachine<SummonStateType> _stateMachine;
        private SummonMovement _movement;
        private SummonShooter _shooter;

        private float _standbyDuration;
        private float _retreatDistance;
        private float _timer;

        public SummonStandbyState(StateMachine<SummonStateType> stateMachine, SummonMovement movement, SummonShooter shooter, 
            float standbyDuration, float retreatDistance)
        {
            _stateMachine = stateMachine;
            _movement = movement;
            _shooter = shooter;

            _standbyDuration = standbyDuration;
            _retreatDistance = retreatDistance;
        }

        public void OnEnter()
        {
            _timer = 0f;
        }

        public void OnUpdate()
        {
            _timer += Time.deltaTime;

            if(_timer >= _standbyDuration)
            {
                if (_shooter.ShouldAttackEnemies())
                {
                    _stateMachine.ChangeState(SummonStateType.ATTACK);

                    //if (_movement.GetDistanceToSummoner() >= _retreatDistance)
                    //{
                    //    _stateMachine.ChangeState(SummonStateType.FOLLOW);
                    //}
                    //else
                    //{
                    //    _stateMachine.ChangeState(SummonStateType.ATTACK);
                    //}
                }
                else
                {
                    _stateMachine.ChangeState(SummonStateType.FOLLOW);
                }
            }
        }

        public void OnExit()
        {
        }

        public void OnDispose()
        {
        }
    }
}
