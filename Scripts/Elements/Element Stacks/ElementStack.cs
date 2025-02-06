using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Elements
{
    public abstract class ElementStack : ScriptableObject
    {
        public virtual void StackEffects(TankElementsEffects.Effect oldElement, TankElementsEffects.Effect newElement, TankElementsEffects tankEffects)
        {
            
        }
    }
}
