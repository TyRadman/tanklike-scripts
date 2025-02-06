using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using TankLike.Combat.SkillTree;
    using UnitControllers;

    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "BulletDeflection", menuName = MENU_ROOT + "Bullet Deflection")]
    public class BulletDeflectionSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private FiniteDeflections _deflectionInfo;

        public override void ApplyStats(TankComponents components)
        {
            //components.Shooter.SetDeflection(_deflectionInfo);
        }

        public override void PopulateStatProperties()
        {
            SkillProperties deflectionsCount = new SkillProperties()
            {
                Name = "Number of deflections",
                Value = _deflectionInfo.MaxDeflections.ToString(),
                DisplayColor = Colors.Gray
            };

            StatProperties.Add(deflectionsCount);
        }
    }
}
