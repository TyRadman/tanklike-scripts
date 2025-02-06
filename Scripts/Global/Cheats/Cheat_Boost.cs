
using UnityEngine;

namespace TankLike.Cheats
{
	[CreateAssetMenu(fileName = NAME + "Boost", menuName = ROOT + "Boost")]
	public class Cheat_Boost : Cheat
	{
		public override void Initiate()
		{
			base.Initiate();
			Button.SetButtonColor(false);
		}

		public override void PerformCheat()
		{
			_enabled = !_enabled;
			GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.Fuel.EnableFuelConsumption(!_enabled));

			Button.SetButtonColor(_enabled);
		}
	}
}