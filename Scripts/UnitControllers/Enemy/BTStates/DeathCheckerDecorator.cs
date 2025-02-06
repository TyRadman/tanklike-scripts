using TankLike.UnitControllers;
using UnityEngine;

namespace BT.Nodes
{
#if UNITY_EDITOR
	public class DeathCheckerDecorator : ConditionalCheckNode
	{
		private EnemyHealth _health;

        public override void OnAwake()
        {
            base.OnAwake();

			_health = Agent.Health as EnemyHealth;

		}

        protected override bool IsTrue()
		{
			bool isDead = _health.IsDead;
			return !isDead;
		}
	}
#endif
}