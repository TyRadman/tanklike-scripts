using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "DF_MovementSpeed", menuName = MENU_NAME + "Movement Speed")]
    public class MovementSpeedDifficultyModifier : DifficultyModifier
    {
        [SerializeField] private Vector2 _movementSpeedRange;

        public override void ApplyModifier(TankComponents enemy, float difficulty)
        {
            base.ApplyModifier(enemy, difficulty);

            enemy.Movement.SetSpeed(_movementSpeedRange.Lerp(difficulty));
        }
    }
}
