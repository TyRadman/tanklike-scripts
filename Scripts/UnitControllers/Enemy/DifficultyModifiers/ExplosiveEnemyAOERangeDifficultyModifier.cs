using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "DF_ExplosiveAOERange", menuName = MENU_NAME + "Explosive Enemy AOE Range")]
    public class ExplosiveEnemyAOERangeDifficultyModifier : DifficultyModifier
    {
        [SerializeField] private Vector2 _radiusRange;

        public override void ApplyModifier(TankComponents enemy, float difficulty)
        {
            base.ApplyModifier(enemy, difficulty);

            enemy.GetComponent<ExplosiveShooter>().SetRadius(_radiusRange.Lerp(difficulty));
        }
    }
}
