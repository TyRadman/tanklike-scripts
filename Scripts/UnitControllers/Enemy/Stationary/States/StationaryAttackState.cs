using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TankLike.PlayersManager;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Aimer_Attack", menuName = MENU_PATH + "Aimer/Attack")]
    public class StationaryAttackState : AttackState
    {
        private EnemyMovement _movement;
        private Transform _target;
        [SerializeField] private bool _isImageTarget;
        [SerializeField] private float _predictionDistance = 0;
        private PlayerTransforms _targetTransform;

        private Coroutine _attackCoroutine;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);
            _stateMachine = stateMachine;
            _movement = (EnemyMovement)enemyComponents.Movement;
            _shooter = (EnemyShooter)enemyComponents.Shooter;

            _shooter.OnAttackFinished += OnAttackFinishedHandler;
            _shooter.OnTelegraphFinished += OnTelegraphFinishedHandler;
        }

        public override void OnEnter()
        {
            _isActive = true;
            _target = !_isImageTarget ? _shooter.GetCurrentTarget().PlayerTransform : _shooter.GetCurrentTarget().ImageTransform;
            _targetTransform = _shooter.GetCurrentTarget();

            _shooter.StartTelegraph();

            //if (CanAttack())
            //{
            //    _shooter.StartTelegraph();
            //}
            //else
            //{
            //    _stateMachine.ChangeState(EnemyStateType.AIM);
            //}
        }

        public override void OnUpdate()
        {
            //_turretController.HandleTurretRotation(_target);
            _turretController.HandleTurretRotation(_targetTransform.GetImageAtDistance(_predictionDistance));
        }

        public override void OnExit()
        {
            _isActive = false;
            _shooter.UnsetCurrentTarget();
        }

        public override void OnDispose()
        {
            _shooter.OnShootFinished -= OnAttackFinishedHandler;
            _shooter.OnTelegraphFinished -= OnTelegraphFinishedHandler;
        }

        private void OnTelegraphFinishedHandler()
        {
            if (!_isActive)
            {
                return;
            }
         
            _shooter.StartAttackRoutine(_attacksAmountRange.RandomValue(), _breakBetweenAttacks);
        }

        private void OnAttackFinishedHandler()
        {
            if (!_isActive)
            {
                return;
            }

            _stateMachine.ChangeState(EnemyStateType.AIM);
        }
    }
}
