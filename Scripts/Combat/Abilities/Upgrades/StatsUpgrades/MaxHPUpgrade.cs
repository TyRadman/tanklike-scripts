using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using UnitControllers;

    [CreateAssetMenu(fileName = PREFIX + "MaxHP_NUMBER", menuName = Directories.STATS_UPGRADES + "Max HP")]
    public class MaxHPUpgrade : SkillUpgrade
    {
        [SerializeField] private int _healthPointsToIncrease = 10;

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            _properties.Clear();

            int previousMaxHP = player.Health.GetMaxHP();

            SkillProperties health = new SkillProperties()
            {
                IsComparisonValue = true,
                PreviousValue = previousMaxHP.ToString(),
                Value = (previousMaxHP + _healthPointsToIncrease).ToString(),
                Name = "Max Health",
                UnitString = PropertyUnits.POINTS
            };

            _properties.Add(health);

            SaveProperties();
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();

            _player.Health.AddToMaxHealth(_healthPointsToIncrease);
        }
    }
}
