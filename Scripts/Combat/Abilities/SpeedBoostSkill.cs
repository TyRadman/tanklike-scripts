using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using UnitControllers;
    using Combat.SkillTree;

    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "SpeedBoost", menuName = MENU_ROOT + "Speed Boost")]
    public class SpeedBoostSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private float _speedMultiplier = 0.3f;

        public override void ApplyStats(TankComponents components)
        {
            //components.Movement.MultiplySpeed(_speedMultiplier);
        }

        public override void PopulateStatProperties()
        {
            SkillProperties speedIncrement = new SkillProperties()
            {
                Name = "Speed increase",
                Value = ((1 + _speedMultiplier) * 100f).ToString(),
                DisplayColor = Colors.Green,
                UnitString = PropertyUnits.PERCENTAGE};

            StatProperties.Add(speedIncrement);
        }
    }
}
