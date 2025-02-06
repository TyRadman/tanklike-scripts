using UnityEngine;

namespace TankLike.Cheats
{
	[CreateAssetMenu(fileName = NAME + "DestroyEnemies", menuName = ROOT + "Destroy Enemies")]
	public class Cheat_DestroyEnemies : Cheat
	{
		public override void Initiate()
		{
			base.Initiate();
		}

		public override void PerformCheat()
		{
			GameManager.Instance.EnemiesManager.DestroyAllEnemies();
		}
	}
}