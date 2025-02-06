using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using UI.SkillTree;
    using Combat.SkillTree;
    using Combat.SkillTree.Upgrades;
    using Utils;
    using System;

    public class PlayerUpgrades : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }

        [field: SerializeField, Header("Skills")] public List<SkillProfile> BaseWeaponSkills { get; private set; }
        [field: SerializeField] public List<SkillProfile> SuperAbilitySkills { get; private set; }
        [field: SerializeField] public List<SkillProfile> ChargeAbilitySkills { get; private set; }
        [field: SerializeField] public List<SkillProfile> BoostAbilitySkills { get; private set; }

        private SkillProfile _baseShotSkillProfile;
        private SkillProfile _chargeAttackSkillProfile;
        private SkillProfile _superAbilitySkillProfile;
        private SkillProfile _boostAbilitySkillProfile;

        [Header("Others")]
        [SerializeField] private int _skillPointsCount;
        [SerializeField] private SkillTreeHolder _skillTreePrefab;
        [SerializeField] private List<SkillUpgrade> _statUpgrades = new List<SkillUpgrade>();
        [SerializeField] private List<SkillUpgrade> _specialUpgrades = new List<SkillUpgrade>();

        private PlayerComponents _playerComponents;
        private PlayerExperience _experience;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _experience = _playerComponents.Experience;

            _statUpgrades.ForEach(u => u.SetUp(_playerComponents));
            _specialUpgrades.ForEach(u => u.SetUp(_playerComponents));
        }

        public List<SkillUpgrade> GetStatUpgrades()
        {
            return _statUpgrades;
        }

        public List<SkillUpgrade> GetSpecialUpgrades()
        {
            return _specialUpgrades;
        }

        public void Upgrade(SkillUpgrade upgrade)
        {
            if (upgrade is BaseWeaponUpgrade weaponUpgrade)
            {
                _playerComponents.Shooter.UpgradeSkill(weaponUpgrade);
            }
        }

        #region Utilities

        #region Getters
        public SkillProfile GetBaseWeaponSkillProfile()
        {
            return _baseShotSkillProfile;
        }

        public SkillProfile GetChargeAttackSkillProfile()
        {
            return _chargeAttackSkillProfile;
        }

        public SkillProfile GetSuperAbilitySkillProfile()
        {
            return _superAbilitySkillProfile;
        }

        public SkillProfile GetBoostAbilitySkillProfile()
        {
            return _boostAbilitySkillProfile;
        }
        #endregion

        #region Setters
        public void SetSuperAbility(SkillProfile skillProfile)
        {
            _superAbilitySkillProfile = skillProfile;
        }

        public void SetBoostAbility(SkillProfile skillProfile)
        {
            _boostAbilitySkillProfile = skillProfile;
            _statUpgrades.AddRange(_boostAbilitySkillProfile.Upgrades);
        }

        public void SetChargeAttack(SkillProfile skillProfile)
        {
            _chargeAttackSkillProfile = skillProfile;
        }

        public void SetBaseWeapon(SkillProfile skillProfile)
        {
            _baseShotSkillProfile = skillProfile;
        }
        #endregion

        public void AddSkillPoint()
        {
            _skillPointsCount++;
        }

        public void AddSkillPoints(int amount)
        {
            _skillPointsCount += amount;
        }

        public int GetSkillPoints()
        {
            return _skillPointsCount;
        }

        public SkillTreeHolder GetSkillTree()
        {
            return _skillTreePrefab;
        }
        #endregion

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            _experience.OnLevelUp += AddSkillPoint;
        }

        public void Dispose()
        {
            _experience.OnLevelUp -= AddSkillPoint;
        }
        #endregion
    }

    public enum UpgradeTypes
    {
        BaseWeapon = 0, ChargeAttack = 1, SuperAbility = 2, BoostAbility = 3, SpecialUpgrade = 4, StatsUpgrade = 5, None = 10
    }
}
