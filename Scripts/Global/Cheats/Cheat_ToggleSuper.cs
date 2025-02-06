using UnityEngine;

namespace TankLike.Cheats
{
	[CreateAssetMenu(fileName = NAME + "SuperToggle", menuName = ROOT + "Super Toggle")]
	public class Cheat_ToggleSuper : Cheat
    {
        public override void Initiate()
        {
            base.Initiate();
            Button.SetButtonColor(false);
        }

		public override void PerformCheat()
		{
            _enabled = !_enabled;

            if (_enabled)
            {
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperRecharger.FullyChargeSuperAbility());
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperAbilities.EnableChargeConsumption(false));
            }
            else
            {
                GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperAbilities.EnableChargeConsumption(true));
            }

            Button.SetButtonColor(_enabled);
        }
	}
}