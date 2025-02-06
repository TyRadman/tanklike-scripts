using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Utils;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "DM_BulletSpeed", menuName = MENU_NAME + "Bullet Speed")]
    public class BulletSpeedDifficultyModifier : DifficultyModifier
    {
        [SerializeField] private Vector2Int _bulletSpeedRange;

        public override void ApplyModifier(TankComponents enemy, float difficulty)
        {
            base.ApplyModifier(enemy, difficulty);

            enemy.Shooter.SetWeaponSpeed(_bulletSpeedRange.Lerp(difficulty));
        }
    }
}
