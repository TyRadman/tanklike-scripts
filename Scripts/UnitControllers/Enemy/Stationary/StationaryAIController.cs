using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class StationaryAIController : EnemyAIController
    {
        [Header("Idle State")]
        [SerializeField] private Vector2 _idleDurationRange;

        [Header("Attack State")]
        [SerializeField] private Vector2 _attackCooldownRange;

        protected override void InitStateMachine()
        {
            _stateMachine = new StateMachine<EnemyStateType>();

            var states = new Dictionary<EnemyStateType, IState>();

            var aimState = new StationaryAimState(_stateMachine, _movement, _shooter, _idleDurationRange);
            states.Add(EnemyStateType.AIM, aimState);

            //var attackState = new StationaryAttackState(_stateMachine, _movement, _shooter);
            //states.Add(EnemyStateType.ATTACK, attackState);

            _stateMachine.Init(states);
        }
    }
}
