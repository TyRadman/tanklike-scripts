using UnityEngine;

namespace TankLike.Cheats
{
	[CreateAssetMenu(fileName = NAME + "ReplenishHealth", menuName = ROOT + "Replenish Health")]
	public class Cheat_ReplenishHealth : Cheat
	{
		public override void Initiate()
		{
			base.Initiate();
		}

		public override void PerformCheat()
		{
			GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.Health.ReplenishFullHealth());
		}
	}
}