using UnityEngine;

namespace TankLike.Cheats
{
	[CreateAssetMenu(fileName = NAME + "AddBossKey", menuName = ROOT + "Boss Key")]
	public class Cheat_AddBossKey : Cheat
	{
		public override void Initiate()
		{
			base.Initiate();
		}

		public override void PerformCheat()
		{
			GameManager.Instance.BossKeysManager.OnKeyCollected();
		}
	}
}