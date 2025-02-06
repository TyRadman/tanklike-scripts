using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Laser_Attack", menuName = MENU_PATH + "Laser/Attack State")]
    public class LaserEnemyAttackState : AttackState
    {
        [SerializeField] private Vector2 _attackCooldownRange;
        private EnemyMovement _movement;
        private Transform _target;
        private bool _isTelegraphing;
        private float _cooldownDuration;
        private float _cooldownTimer;
        private bool _isCooldown;
        private bool _startAttack;

        private bool _aimAtTarget;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            _movement = (EnemyMovement)enemyComponents.Movement;
            _shooter.OnAttackFinished += OnAttackFinishedHandler;
            _shooter.OnTelegraphFinished += OnTelegraphFinishedHandler;
        }

        public override void OnEnter()
        {
            //Debug.Log("ATTACK STATE");
            _isActive = true;
            _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);

            _isTelegraphing = true;

            _isCooldown = false;
            _cooldownDuration = Random.Range(_attackCooldownRange.x, _attackCooldownRange.y);
            _cooldownTimer = 0f;

            _startAttack = true;
        }

        public override void OnUpdate()
        {
            if (_movement.GetCurrentSpeed() >= 0.1f)
            {
                return;
            }

            if (_startAttack)
            {
                _shooter.StartTelegraph();
                _startAttack = false;
            }

            if (_isCooldown)
            {
                _cooldownTimer += Time.deltaTime;

                if (_cooldownTimer >= _cooldownDuration)
                {
                    _stateMachine.ChangeState(EnemyStateType.MOVE);
                }
            }
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
            _shooter.OnShootFinished -= OnAttackFinishedHandler;
        }

        private void OnTelegraphFinishedHandler()
        {
            if (!_isActive)
            {
                return;
            }

            _isTelegraphing = false;
            _shooter.StartAttackRoutine(((LaserWeapon)_shooter.GetWeapon()).GetLaserDuration());
            _aimAtTarget = false;
        }

        private void OnAttackFinishedHandler()
        {
            if (!_isActive)
                return;

            _isCooldown = true;
        }
    }
}
