using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using Combat.SkillTree;
    using UnitControllers;

    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "MaxHealthBoost", menuName = MENU_ROOT + "Max Health Boost")]
    public class MaxHealthBoostSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private int _healthPoints = 100;

        public override void ApplyStats(TankComponents components)
        {
            components.Health.AddToMaxHealth(_healthPoints);
        }

        public override void PopulateStatProperties()
        {
            SkillProperties speedIncrement = new SkillProperties()
            {
                Name = "Health to add",
                Value = _healthPoints.ToString(),
                DisplayColor = Colors.Green,
                UnitString = PropertyUnits.POINTS
            };

            StatProperties.Add(speedIncrement);
        }
    }
}
