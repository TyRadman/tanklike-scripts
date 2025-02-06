using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using UnitControllers;
    using Elements;
    using Combat.SkillTree;

    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "BulletElement", menuName = MENU_ROOT + "Bullet Element")]
    public class ElementBulletSkill : Skill
    {
        [Header("Special Values")]
        [SerializeField] private ElementEffect _element;

        public override void ApplyStats(TankComponents components)
        {
            //components.Shooter.SetElement(_element);
        }

        public override void PopulateStatProperties()
        {
            
        }
    }
}
