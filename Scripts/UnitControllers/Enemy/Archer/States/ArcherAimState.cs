using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Archer_Aim", menuName = MENU_PATH + "Archer/Aim State")]
    public class ArcherAimState : AimState
    {
        [Header("Archer values")]
        [SerializeField] private Vector2 _standbyDurationRange;

        private float _standbyDuration;
        private float _standbyTimer;
        private float _switchTargetDuration;
        private float _switchTargetTimer;
 
        public override void OnEnter()
        {
            //Debug.Log("AIM STATE");
            _isActive = true;

            SetTarget();

            //set random values for state parameters
            _aimDuration = Random.Range(_aimDurationRange.x, _aimDurationRange.y);
            _aimTimer = 0f;
            _standbyDuration = Random.Range(_standbyDurationRange.x, _standbyDurationRange.y);
            _standbyTimer = 0f;
            _switchTargetDuration = Random.Range(_switchTargetDurationRange.x, _switchTargetDurationRange.y);
            _switchTargetTimer = 0f;
        }

        public override void OnUpdate()
        {
            // switch to MOVE state when the timer is up
            if (_aimTimer >= _aimDuration)
            {
                _stateMachine.ChangeState(EnemyStateType.MOVE);
            }

            _aimTimer += Time.deltaTime;

            _turretController.HandleTurretRotation(_target);

            // check if the target can be shot
            if (_shooter.IsWayToTargetBlocked(_target))
            {
                _switchTargetTimer += Time.deltaTime;

                if (_switchTargetTimer >= _switchTargetDuration)
                {
                    //Debug.Log("Switch Target");
                    SetTarget();
                    _switchTargetTimer = 0f;
                }

                return;
            }

            //if not, stand by, and shoot if there is no cooldown
            _standbyTimer += Time.deltaTime;

            if (_standbyTimer >= _standbyDuration)
            {
                if (_shooter.IsCanShoot())
                {
                    _stateMachine.ChangeState(EnemyStateType.ATTACK);
                }
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
