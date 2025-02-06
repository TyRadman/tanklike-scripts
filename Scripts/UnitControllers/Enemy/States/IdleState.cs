using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Idle", menuName = MENU_PATH + "Idle")]
    public class IdleState : State
    {
        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }
    }
}
