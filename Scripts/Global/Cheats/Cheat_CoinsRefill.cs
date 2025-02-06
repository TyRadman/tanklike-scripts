using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cheats
{
    [CreateAssetMenu(fileName = NAME + "CoinsRefill", menuName = ROOT + "Coins Refill")]
    public class Cheat_CoinsRefill : Cheat
    {
        private const int COINS_TO_ADD = 500;

        public override void Initiate()
        {
            base.Initiate();
        }

        public override void PerformCheat()
        {
            GameManager.Instance.PlayersManager.Coins.AddCoins(COINS_TO_ADD);
        }
    }
}
