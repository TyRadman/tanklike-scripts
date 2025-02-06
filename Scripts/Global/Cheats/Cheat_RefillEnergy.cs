using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cheats
{
    [CreateAssetMenu(fileName = NAME + "RefillEnergy", menuName = ROOT + "Refill Energy")]
    public class Cheat_RefillEnergy : Cheat
    {
		public override void Initiate()
		{
			base.Initiate();
		}

		public override void PerformCheat()
		{
			GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.Energy.MaxFillEnergy());
		}
	}
}
