using UnityEngine;

namespace TankLike
{
    using Combat.SkillTree.Upgrades;
    using UnitControllers;

    [CreateAssetMenu(fileName = PREFIX + "Special_UnlockChargedAttack", menuName = Directories.SPECIAL_UPGRADES + "Unlock Charged Attack")]
    public class UnlockChargeAttackUpgrade : SkillUpgrade
    {
        private PlayerHoldAction _playerHoldAction;

        public override void SetUp(PlayerComponents player)
        {
            base.SetUp(player);

            _playerHoldAction = player.GetUnitComponent<PlayerHoldAction>();
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();

            _playerHoldAction.Unlock();
        }
    }
}
