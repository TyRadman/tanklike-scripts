using TankLike.UnitControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Elements
{
    [CreateAssetMenu(fileName = "Lightning Effect", menuName = "Elements/ Lightning")]
    public class LightningElementEffect : ElementEffect
    {
        [Header("Special Values")]
        [SerializeField] [Range(0.01f, 0.9f)] private float _speedMultiplier = 0.5f;

        public override void TakeEffect(TankComponents tank)
        {
            base.TakeEffect(tank);
            //tank.Movement.SetSpeedMultiplier(_speedMultiplier);
        }

        public override void StopEffect(TankComponents tank)
        {
            base.StopEffect(tank);
            //tank.Movement.SetSpeedMultiplier(1f);
        }
    }
}
