using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using Combat.Abilities;
    using UnitControllers;
    using Utils;

    [CreateAssetMenu(fileName = PREFIX + "BaseBoostUpgrade_NAME", menuName = UPGRADE_ROOT_PATH + "Upgrade")]
    public class BaseBoostUpgrade : AbilityUpgrades
    {
        public const string UPGRADE_ROOT_PATH = Directories.ABILITIES + "Base Boost Ability/";

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            EmptyAbility ability = player.BoostAbility.GetAbilityHolder().Ability as EmptyAbility;

            if (ability == null)
            {
                Helper.LogWrongAbilityProperty(typeof(EmptyAbility));
                return;
            }

            if (_properties.Count > 0)
            {
                UpgradeProperties = GetSkillPropertiesString(_properties);
            }
        }
    }
}
