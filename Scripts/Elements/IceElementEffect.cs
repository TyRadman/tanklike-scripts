using TankLike.UnitControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Elements
{
    [CreateAssetMenu(fileName = "Ice Effect", menuName = "Elements/ Ice")]
    public class IceElementEffect : ElementEffect
    {
        public override void TakeEffect(TankComponents tank)
        {
            base.TakeEffect(tank);

            // TODO: replace it with constraints
            //tank.Movement.EnableMovement(false);
            tank.Movement.StopMovement();
            tank.Movement?.EnableRotation(false);
            //tank.Shooter?.EnableShooting(false);
            tank.Animation?.StopAnimations(true);
            //tank.SuperAbility?.EnableAbility(false);
        }

        public override void StopEffect(TankComponents tank)
        {
            base.StopEffect(tank);

            // TODO: replace it with constraints
            //tank.Movement.EnableMovement(true);
            tank.Movement?.EnableRotation(true);
            //tank.Shooter?.EnableShooting(true);
            tank.Animation?.StopAnimations(false);
            //tank.SuperAbility?.EnableAbility(true);
        }
    }
}
