using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    [CreateAssetMenu(fileName = PREFIX + "Empty", menuName = Directories.ABILITIES + "Empty Ability")]
    public class EmptyAbility : Ability
    {
        public override void Dispose()
        {
        }

        public override void OnAbilityFinished()
        {
        }

        public override void OnAbilityHoldStart()
        {
        }

        public override void OnAbilityHoldUpdate()
        {
        }

        public override void OnAbilityInterrupted()
        {
        }

        public override void SetUpIndicatorSpecialValues(AirIndicator indicator)
        {
        }
    }
}
