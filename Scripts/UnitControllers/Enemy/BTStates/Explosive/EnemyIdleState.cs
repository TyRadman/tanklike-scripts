using UnityEngine;

namespace BT.Nodes
{
#if UNITY_EDITOR
	public class EnemyIdleState : ActionNode
	{
		protected override void OnStart()
		{
			// start logic
		}

		protected override NodeState OnUpdate()
		{
			// update logic
			return NodeState.Success;
		}

		protected override void OnStop()
		{
			// stop logic
		}
	}
#endif
}