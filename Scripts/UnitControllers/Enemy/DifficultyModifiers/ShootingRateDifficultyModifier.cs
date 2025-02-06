using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers.States;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "DM_ShootingRate", menuName = MENU_NAME + "Shooting Rate")]
    public class ShootingRateDifficultyModifier : DifficultyModifier
    {
        [SerializeField] private Vector2 _shootingRateRange;

        public override void ApplyModifier(TankComponents enemy, float difficulty)
        {
            base.ApplyModifier(enemy, difficulty);

            ((AttackState)((EnemyComponents)enemy).AIController.GetStateByType(EnemyStateType.ATTACK)).SetAttackRate(_shootingRateRange.Lerp(difficulty));
        }
    }
}
