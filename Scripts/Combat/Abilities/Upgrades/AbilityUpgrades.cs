using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using UnitControllers;

    /// <summary>
    /// The base class that holds upgrades for abilities. These can be super, charge or boost abilities.
    /// </summary>
    public abstract class AbilityUpgrades : SkillUpgrade
    {
        protected const string ROOT_PATH = Directories.ABILITIES;

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();

            switch(UpgradeType)
            { 
                case UpgradeTypes.ChargeAttack:
                    _player.ChargeAttack.GetAbilityHolder().UpgradeSkill(this, _player);
                    _player.ChargeAttack.ReEquipSkill();
                    _player.Upgrades.GetChargeAttackSkillProfile().Upgrades.ForEach(u => u.SetUpgradeProperties(_player));
                    break;
                case UpgradeTypes.SuperAbility:
                    _player.SuperAbilities.GetAbilityHolder().UpgradeSkill(this, _player);
                    _player.SuperAbilities.ReEquipSkill();
                    _player.Upgrades.GetSuperAbilitySkillProfile().Upgrades.ForEach(u => u.SetUpgradeProperties(_player));
                    break;
                case UpgradeTypes.BoostAbility:
                    _player.BoostAbility.GetAbilityHolder().UpgradeSkill(this, _player);
                    _player.BoostAbility.ReEquipSkill();
                    _player.Upgrades.GetBoostAbilitySkillProfile().Upgrades.ForEach(u => u.SetUpgradeProperties(_player));
                    break;
            }
        }
    }
}
