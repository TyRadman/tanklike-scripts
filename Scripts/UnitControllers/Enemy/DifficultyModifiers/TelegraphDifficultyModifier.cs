using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers.States;
using TankLike.Utils;
using UnityEngine;


namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "DF_TelegraphDuration", menuName = MENU_NAME + "Telegraph Duration")]
    public class TelegraphDifficultyModifier : DifficultyModifier
    {
        [SerializeField] private Vector2 _telegraphDurationRange;

        public override void ApplyModifier(TankComponents enemy, float difficulty)
        {
            base.ApplyModifier(enemy, difficulty);

            ((EnemyShooter)enemy.Shooter).SetTelegraphSpeed(_telegraphDurationRange.Lerp(difficulty));
        }
    }
}
