using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    public class BossDeathState : BossState
    {
        [SerializeField] protected float _deathStateDuration = 3f;
        [SerializeField] protected float _deathExplosionInterval = 0.25f;
        [SerializeField] protected float _deathExplosionRadius = 1.5f;
        [SerializeField] protected float _explosionCenterHeight = 0.5f;

        protected bool _isActive;
        protected BossHealth _health;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);

            _health = (BossHealth)bossComponents.Health;
        }
    }
}
