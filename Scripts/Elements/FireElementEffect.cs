using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;
using TankLike.Combat;

namespace TankLike.Elements
{
    [CreateAssetMenu(fileName = "Fire Effect", menuName = "Elements/ Fire or Poison")]
    public class FireElementEffect : ElementEffect
    {
        [Header("Special Values")]
        [SerializeField] private int _damagePerSecond = 5;

        public override void TakeEffect(TankComponents tank)
        {
            DamageInfo damageInfo = DamageInfo.Create()
                .SetDamage(_damagePerSecond)
                .SetInstigator(tank)
                .Build();

            tank.Health.TakeDamage(damageInfo);
            // play any effects

        }
    }
}
