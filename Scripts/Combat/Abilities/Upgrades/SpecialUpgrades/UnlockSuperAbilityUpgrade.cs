using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    [CreateAssetMenu(fileName = PREFIX + "Special_UnlockSpecialAbility", menuName = Directories.SPECIAL_UPGRADES + "Unlock Special Ability")]
    public class UnlockSuperAbilityUpgrade : SkillUpgrade
    {
        private PlayerSuperAbilities _playerSuperAbility;

        public override void SetUp(PlayerComponents player)
        {
            base.SetUp(player);

            _playerSuperAbility = player.GetUnitComponent<PlayerSuperAbilities>();
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();

            _playerSuperAbility.Unlock();
        }
    }
}
