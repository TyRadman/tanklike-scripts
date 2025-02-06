using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.Cheats
{
    [CreateAssetMenu(fileName = NAME + "SuperCharge", menuName = ROOT + "Super Charge")]
    public class Cheat_ChargeSuper : Cheat
    {
        public override void Initiate()
        {
            base.Initiate();
        }

        public override void PerformCheat()
        {
            GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.SuperRecharger.FullyChargeSuperAbility());
        }
    }
}
