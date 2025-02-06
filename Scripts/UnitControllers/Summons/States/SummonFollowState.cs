using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class SummonFollowState : IState
    {
        private StateMachine<SummonStateType> _stateMachine;
        private SummonMovement _movement;

        public SummonFollowState(StateMachine<SummonStateType> stateMachine, SummonMovement movement)
        {
            _stateMachine = stateMachine;
            _movement = movement;

            _movement.OnReachedSummoner += OnReachedSummonerHandler;
        }

        public void OnEnter()
        {
            //Debug.Log("FOLLOW");
        }

        public void OnUpdate()
        {
            _movement.FollowSummoner();
        }

        public void OnExit()
        {
        }

        public void OnDispose()
        {
        }

        private void OnReachedSummonerHandler()
        {
            _stateMachine.ChangeState(SummonStateType.STANDBY);
        }
    }
}
