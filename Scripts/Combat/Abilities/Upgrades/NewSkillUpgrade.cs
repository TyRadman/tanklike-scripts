using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using UnitControllers;

    /// <summary>
    /// An upgrade that adds a new ability to the player.
    /// </summary>
    [CreateAssetMenu(fileName = PREFIX + "BrockUpgrade_NAME", menuName = ROOT + "New Ability")]
    public class NewSkillUpgrade : SkillUpgrade
    {
        public override void SetUpgradeProperties(PlayerComponents player)
        {

        }
    }
}
