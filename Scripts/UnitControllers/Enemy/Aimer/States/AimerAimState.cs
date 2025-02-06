using TankLike.UnitControllers;
using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Aimer_Aim", menuName = MENU_PATH + "Aimer/Aim")]
    public class AimerAimState : AimState
    {
        private float _switchTargetDuration;
        private float _switchTargetTimer;

        public AimerAimState(StateMachine<EnemyStateType> stateMachine, EnemyMovement movement, EnemyShooter shooter,
            Vector2 aimDurationRange)
        {
            _stateMachine = stateMachine;
            _movement = movement;
            _shooter = shooter;
            _aimDurationRange = aimDurationRange;
        }

        public override void OnEnter()
        {
            //Debug.Log("AIM STATE");
            _isActive = true;

            SetTarget();

            //set random values for state parameters
            _aimDuration = Random.Range(_aimDurationRange.x, _aimDurationRange.y);
            _aimTimer = 0f;
            _switchTargetDuration = Random.Range(_switchTargetDurationRange.x, _switchTargetDurationRange.y);
            _switchTargetTimer = 0f;
        }

        public override void OnUpdate()
        {
            // switch to ATTACK state when the timer is up
            // TODO: add a bool to control whether the enemy should shoot a blocked target
            if (_aimTimer >= _aimDuration)
            {
                if (!_shooter.IsWayToTargetBlocked(_target))
                {
                    _stateMachine.ChangeState(EnemyStateType.ATTACK);
                }
                else
                {
                    _stateMachine.ChangeState(EnemyStateType.AIM);
                }
            }

            _aimTimer += Time.deltaTime;

            _turretController.HandleTurretRotation(_target);

            // check if the target can be shot
            if (_shooter.IsWayToTargetBlocked(_target))
            {
                _switchTargetTimer += Time.deltaTime;

                if (_switchTargetTimer >= _switchTargetDuration)
                {
                    SetTarget();
                    _switchTargetTimer = 0f;
                }

                return;
            }
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
