using TankLike.UnitControllers;
using UnityEngine;

namespace BT.Nodes
{
#if UNITY_EDITOR
	public class ExplosiveAttackAction : ActionNode
	{
		private ExplosiveHealth _health;
		private ExplosiveShooter _shooter;

        public override void OnAwake()
        {
            base.OnAwake();

			_health = Agent.Health as ExplosiveHealth;
			_shooter = Agent.Shooter as ExplosiveShooter;
		}

        protected override void OnStart()
		{
			_health.Explode();
			_shooter.Explode();
		}

		protected override NodeState OnUpdate()
		{

			return NodeState.Success;
		}

		protected override void OnStop()
		{
			// stop logic
		}
	}
#endif
}