using UnityEngine;

namespace TankLike.Cheats
{
	[CreateAssetMenu(fileName = NAME + "RevivePlayer", menuName = ROOT + "Revive Player")]
	public class Cheat_RevivePlayer : Cheat
	{
		[SerializeField] private int _playerIndex = 0;

		public override void Initiate()
		{
			base.Initiate();
		}

		public override void PerformCheat()
		{
			if(_playerIndex == 1 && PlayersManager.PlayersCount == 1)
            {
				Debug.Log($"There is no P{_playerIndex}");
				return;
            }

			Vector3 position = GameManager.Instance.RoomsManager.CurrentRoom.GetRandomSpawnPoint();
			GameManager.Instance.PlayersManager.PlayerSpawner.RevivePlayer(_playerIndex, position);
		}
	}
}