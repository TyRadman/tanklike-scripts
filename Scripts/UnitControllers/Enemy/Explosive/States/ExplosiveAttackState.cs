using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Explosive_Attack", menuName = MENU_PATH + "Explosive/Attack State")]
    public class ExplosiveAttackState : AttackState
    {
        private ExplosiveHealth _explosiveHealth;
        private ExplosiveShooter _explosiveShooter;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            _stateMachine = stateMachine;

            _explosiveHealth = enemyComponents.Health as ExplosiveHealth;
            _explosiveShooter = enemyComponents.Shooter as ExplosiveShooter;
        }

        public override void OnEnter()
        {
            //Debug.Log("ATTACK STATE");

            _isActive = true;

            _explosiveShooter.StartExplosionTimer();   
        }

        public override void OnUpdate()
        {

        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
        }
    }
}
