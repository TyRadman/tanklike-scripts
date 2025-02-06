using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    public class AttackState : State
    {
        [Header("Attack Settings")]
        [Min(1), SerializeField] protected Vector2Int _attacksAmountRange;
        [SerializeField] protected float _breakBetweenAttacks;

        protected bool _isActive;
        protected EnemyShooter _shooter;
        protected EnemyTurretController _turretController;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            _shooter = (EnemyShooter)enemyComponents.Shooter;
            _turretController = (EnemyTurretController)enemyComponents.TurretController;
        }

        public void SetAttackRate(float shootingRate)
        {
            _breakBetweenAttacks = shootingRate;
        }

        protected bool CanAttack()
        {
            return GameManager.Instance.EnemiesCombatManager.RequestAttack(_enemyComponents);
        }
    }
}
