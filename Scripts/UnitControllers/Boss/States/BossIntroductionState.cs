using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    public class BossIntroductionState : BossState
    {
        protected bool _isActive;
        protected BossComponents _components;
        protected BossAnimations _animations;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);

            _components = bossComponents;
            _animations = _components.Animations;
        }
    }
}
