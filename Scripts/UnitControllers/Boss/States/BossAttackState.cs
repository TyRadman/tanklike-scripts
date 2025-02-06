using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    public class BossAttackState : BossState
    {
        [Header("General Settings")]
        [SerializeField] protected float _randomTargetChance = 0.5f;

        protected bool _isActive;
        protected BossComponents _components;
        protected BossMovementController _movement;
        protected BossAttackController _attackController;
        protected BossAnimations _animations;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);

            _components = bossComponents;
            _movement = (BossMovementController)bossComponents.Movement;
            _attackController = bossComponents.AttackController;
            _animations = _components.Animations;
        }
    }
}
