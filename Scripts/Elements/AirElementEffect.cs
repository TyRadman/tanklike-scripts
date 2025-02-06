using TankLike.UnitControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Elements
{
    [CreateAssetMenu(fileName = "Air Effect", menuName = "Elements/ Air")]
    public class AirElementEffect : ElementEffect
    {
        [Header("Special Values")]
        [SerializeField] private float _damageMultiplier = 0.5f;

        public override void TakeEffect(TankComponents tank)
        {
            base.TakeEffect(tank);
            tank.Health.SetDamageIntakeMultiplier(_damageMultiplier);
        }

        public override void StopEffect(TankComponents tank)
        {
            base.StopEffect(tank);
            tank.Health.SetDamageIntakeMultiplier(0f);
        }
    }
}
