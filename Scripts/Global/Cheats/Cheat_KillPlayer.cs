using UnityEngine;

namespace TankLike.Cheats
{
	[CreateAssetMenu(fileName = NAME + "KillPlayer", menuName = ROOT + "Kill Player")]
	public class Cheat_KillPlayer : Cheat
	{
		[SerializeField] private int _playerIndex = 0;

		public override void Initiate()
		{
			base.Initiate();
		}

		public override void PerformCheat()
		{
			GameManager.Instance.PlayersManager.GetPlayer(_playerIndex).Health.Die();
		}
	}
}