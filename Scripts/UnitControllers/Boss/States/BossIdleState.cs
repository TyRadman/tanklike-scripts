using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    public class BossIdleState : BossState
    {
        protected bool _isActive;
        protected BossMovementController _movement;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);

            _movement = (BossMovementController)bossComponents.Movement;
        }
    }
}
