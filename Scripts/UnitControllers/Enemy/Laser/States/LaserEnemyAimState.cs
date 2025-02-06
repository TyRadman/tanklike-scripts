using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_Laser_Aim", menuName = MENU_PATH + "Laser/Aim State")]
    public class LaserEnemyAimState : AimState
    {
        [SerializeField] private float _aimAtTargetChance = 0.5f;
        private bool _aimAtTarget;

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);

            ((LaserShooter)_shooter).OnLaserFacedTarget += OnLaserFacedTargetHandler;
        }
        public override void OnEnter()
        {
            //Debug.Log("AIM STATE");
            _isActive = true;

            //set random values for state parameters
            _aimDuration = Random.Range(_aimDurationRange.x, _aimDurationRange.y);
            _aimTimer = 0f;

            _target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);

            var aimChance = Random.Range(0f, 1f);
            _aimAtTarget = aimChance <= _aimAtTargetChance ? true : false;
        }

        public override void OnUpdate()
        {
            //a delay to make sure the enemy stopped rotating (dirty)
            if (_aimTimer < _aimDuration)
            {
                _aimTimer += Time.deltaTime;
            }
            else
            {
                if(_aimAtTarget)
                {
                    ((LaserShooter)_shooter).AimLaserAtTarget(_target);
                }
                else
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

        private void OnLaserFacedTargetHandler()
        {
            if (!_isActive)
                return;

            _aimAtTarget = false;
        }
    }
}
