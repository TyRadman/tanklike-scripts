using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Elements
{
    [CreateAssetMenu(fileName = "Additive Stack", menuName = "Elements/ Stacks/ Additive", order = 0)]
    public class AdditiveStack : ElementStack
    {
        public override void StackEffects(TankElementsEffects.Effect oldElement, TankElementsEffects.Effect newElement, TankElementsEffects tankEffects)
        {
            base.StackEffects(oldElement, newElement, tankEffects);

            // add the duration of the new effect to the old one
            oldElement.Duration += newElement.Duration;
            // disable the new effect as the older one will still perform it
            newElement.Active = false;
        }
    }
}
