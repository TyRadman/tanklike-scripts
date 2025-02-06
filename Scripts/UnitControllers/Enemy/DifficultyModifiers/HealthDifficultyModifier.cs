using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "DF_Health", menuName = MENU_NAME + "Health")]
    public class HealthDifficultyModifier : DifficultyModifier
    {
        [SerializeField] private Vector2Int _healthRange;

        public override void ApplyModifier(TankComponents enemy, float difficulty)
        {
            base.ApplyModifier(enemy, difficulty);
            enemy.Health.SetMaxHealth(_healthRange.Lerp(difficulty));
        }

        public void SetHealthRange(Vector2Int healthRange)
        {
            _healthRange = healthRange;
        }
    }
}
