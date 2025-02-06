using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers.States;
using TankLike.Utils;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "DF_Name_LaserDuration", menuName = MENU_NAME + "Laser Duration")]
    public class LaserDurationDifficultyModifier : DifficultyModifier
    {
        [SerializeField] private Vector2 _laserDurationRange;

        public override void ApplyModifier(TankComponents enemy, float difficulty)
        {
            base.ApplyModifier(enemy, difficulty);
            enemy.Shooter.SetLaserDuration(_laserDurationRange.Lerp(difficulty));
        }
    }
}
