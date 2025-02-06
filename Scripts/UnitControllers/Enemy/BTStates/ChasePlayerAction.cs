using UnityEngine;
using TankLike.UnitControllers;
using TankLike;
using UnityEngine.AI;

namespace BT.Nodes
{
#if UNITY_EDITOR
	public class ChasePlayerAction : ActionNode
	{
		private EnemyMovement _movement;
		private bool _isTargetReached;
		private float _pathUpdateTime = 0.5f;
		private float _pathUpdateTimer;
		private Transform _target;
		protected Vector3 _targetPoint;

		public override void OnAwake()
        {
            base.OnAwake();

			_movement = Agent.Movement as EnemyMovement;
        }

        protected override void OnStart()
		{
			_isTargetReached = false;
			_movement.OnTargetReached += OnTargetReached;
			_pathUpdateTimer = 0f;
		}

		protected override NodeState OnUpdate()
		{
			UpdateNavMesh();

			if (_isTargetReached)
			{
				return NodeState.Success;
			}

			return NodeState.Running;
		}

		private void UpdateNavMesh()
        {
			_pathUpdateTimer += Time.deltaTime;

			if (_pathUpdateTimer >= _pathUpdateTime)
			{
				_target = GameManager.Instance.PlayersManager.GetClosestPlayerTransform(_movement.transform.position);
				MoveToTarget(_target.position);
				_pathUpdateTimer = 0f;
			}
		}

		private void OnTargetReached()
        {
			_isTargetReached = true;
        }

		private void MoveToTarget(Vector3 target)
		{
			NavMeshHit hit;

			if (NavMesh.SamplePosition(target, out hit, 1.0f, NavMesh.AllAreas))
			{
				_targetPoint = hit.position;
				_movement.SetTargetPosition(_targetPoint);
			}
		}

		protected override void OnStop()
		{
			_movement.OnTargetReached -= OnTargetReached;
		}
	}
#endif
}