using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using System;
    using UnitControllers;

    [CreateAssetMenu(fileName = PREFIX + "Special_SpeedBoostOnFullFuel", menuName = Directories.SPECIAL_UPGRADES + "Player01/ Speed Boost On Full Fuel")]
    public class SpeedBoostOnMaxFuelUpgrade : SkillUpgrade
    {
        [Header(SPECIAL_VALUES_HEADER)]
        [SerializeField] private float _speedIncrement = 0.4f;

        private SpeedStatModifier _speedModifier;

        public override void SetUp(PlayerComponents player)
        {
            base.SetUp(player);

            _speedModifier = new SpeedStatModifier(_speedIncrement);
        }

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            SkillProperties speedIncrement = new SkillProperties()
            {
                IsComparisonValue = false,
                Name = "Speed Increment",
                Value = _speedIncrement.ToString(),
                DisplayColor = Colors.DarkGreen,
                UnitString = PropertyUnits.PERCENTAGE
            };

            _properties.Add(speedIncrement);

            SaveProperties();
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();

            _player.Fuel.AddOnFuelFullListener(OnFuelFull);
            _player.Fuel.AddOnFuelNotFullListener(OnFuelNotFull);
        }

        public override void CancelUpgrade()
        {
            base.CancelUpgrade();

            _player.Fuel.RemoveOnFuelFullListener(OnFuelFull);
            _player.Fuel.RemoveOnFuelNotFullListener(OnFuelNotFull);

            _speedModifier.RemoveStatModifier(_player.StatsController);
        }

        private void OnFuelFull()
        {
            _speedModifier.AddStatModifier(_player.StatsController);
        }

        private void OnFuelNotFull()
        {
            _speedModifier.RemoveStatModifier(_player.StatsController);
        }
    }
}
