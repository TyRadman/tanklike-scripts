using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers.States;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "DM_AimDuration", menuName = MENU_NAME + "Aim Duration")]
    public class AimDurationDifficultyModifier : DifficultyModifier
    {
        [SerializeField] private Vector2 _aimDurationRange;
        [SerializeField] private float _aimRangeOffset = 1f;
        public override void ApplyModifier(TankComponents enemy, float difficulty)
        {
            base.ApplyModifier(enemy, difficulty);

            Vector2 aimRange = new Vector2(_aimDurationRange.Lerp(difficulty) - _aimRangeOffset, _aimDurationRange.Lerp(difficulty) + _aimRangeOffset);
            ((AimState)((EnemyComponents)enemy).AIController.GetStateByType(EnemyStateType.AIM)).SetAimDurationRange(aimRange);
        }
    }
}
