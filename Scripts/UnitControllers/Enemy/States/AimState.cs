using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using static TankLike.PlayersManager;

namespace TankLike.UnitControllers.States
{
    public class AimState : State
    {
        [Header("Aim Settings")]
        [SerializeField] protected Vector2 _aimDurationRange;
        [SerializeField] protected Vector2 _switchTargetDurationRange;

        protected bool _isActive;
        protected EnemyMovement _movement;
        protected EnemyShooter _shooter;
        protected EnemyTurretController _turretController;

        // the time after which the tank attacks
        protected float _aimDuration;
        protected float _aimTimer;
        protected bool _targetFound;
        protected Transform _target;

        public void SetAimDurationRange(Vector2 aimDurationRange)
        {
            _aimDurationRange = aimDurationRange;
        }

        public override void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            base.SetUp(stateMachine, enemyComponents);
            _movement = (EnemyMovement)enemyComponents.Movement;
            _shooter = (EnemyShooter)enemyComponents.Shooter;
            _turretController = (EnemyTurretController)enemyComponents.TurretController;
        }

        protected void SetTarget()
        {
            _targetFound = false;

            // find a target to which the way is not blocked
            var alivePlayers = GameManager.Instance.PlayersManager.GetPlayerTransforms();

            // Find aimables targets
            var aimableTargets = new List<PlayerTransforms>();

            for (int i = 0; i < alivePlayers.Count; i++)
            {
                if (!_shooter.IsWayToTargetBlocked(alivePlayers[i].PlayerTransform))
                {
                    aimableTargets.Add(alivePlayers[i]);
                }
            }

            float closestDistance = 1000;
            PlayerTransforms closest = null;

            // Find the closest among aimable targets

            for (int i = 0; i < aimableTargets.Count; i++)
            {
                float distance = Vector3.Distance(aimableTargets[i].ImageTransform.position, _movement.transform.position);

                if(distance < closestDistance)
                {
                    closest = aimableTargets[i];
                    closestDistance = distance;
                }
            }

            if(closest != null)
            {
                _shooter.SetCurrentTarget(closest);
                _target = closest.PlayerTransform;
                _targetFound = true;
            }

            aimableTargets.Clear();

            // if the way is blocked to all alive targets, get the closest one as the target
            if (!_targetFound)
            {
                var player = GameManager.Instance.PlayersManager.GetClosestPlayer(_movement.transform.position);
                _shooter.SetCurrentTarget(player);
                _target = player.PlayerTransform;
            }
        }
    }
}
