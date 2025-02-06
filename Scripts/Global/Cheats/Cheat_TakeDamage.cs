using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cheats
{
    using Combat;
    using UnitControllers;

    [CreateAssetMenu(fileName = NAME + "DealDamage", menuName = ROOT + "Deal Damage")]
    public class Cheat_TakeDamage : Cheat
    {
        [SerializeField] private int _damageToDeal = 10;

        public override void Initiate()
        {
            base.Initiate();
        }

        public override void PerformCheat()
        {
            List<PlayerComponents> players = GameManager.Instance.PlayersManager.GetPlayers();

            foreach (PlayerComponents player in players)
            {
                DamageInfo damageInfo = DamageInfo.Create()
                    .SetDamage(_damageToDeal)
                    .SetBulletPosition(player.transform.position)
                    .Build();

                player.Health.TakeDamage(damageInfo);
            }
        }
    }
}
