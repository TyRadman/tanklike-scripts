using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Elements
{
    [CreateAssetMenu(fileName = "Cancel Stack", menuName = "Elements/ Stacks/ Cancel", order = 0)]
    public class CancelStack : ElementStack
    {
        public enum CancelType
        {
            OldEffectCancel, NewEffectCancel, BothEffectsCancel
        }

        [Tooltip("- Old Effect Cancel: cancels the effect that was already taking place and allows the new one to be performed.\n- New Effect Cancel: cancels the new effect, allowing the other one to continue.\n- Both Effects Cancel: cancel both effects: the new and old.")]
        [SerializeField] private CancelType _cancelType;

        public override void StackEffects(TankElementsEffects.Effect oldElement, TankElementsEffects.Effect newElement, TankElementsEffects tankEffects)
        {
            base.StackEffects(oldElement, newElement, tankEffects);

            // cancel effects depending on the settings
            switch (_cancelType)
            {
                case CancelType.OldEffectCancel:
                    {
                        oldElement.Active = false;
                        break;
                    }
                case CancelType.NewEffectCancel:
                    {
                        newElement.Active = false;
                        break;
                    }
                case CancelType.BothEffectsCancel:
                    {
                        oldElement.Active = false;
                        newElement.Active = false;
                        break;
                    }
            }
        }
    }
}
