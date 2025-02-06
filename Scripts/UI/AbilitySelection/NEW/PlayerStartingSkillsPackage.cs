using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree
{
    using UnitControllers;

    public class PlayerStartingSkillsPackage : ScriptableObject
    {
        [field: SerializeField] public SkillsPackage[] StartingSkills { get; private set; }

        [field: SerializeField] public List<SkillProfile> WeaponSkills { get; private set; }
        [field: SerializeField] public List<SkillProfile> ChargeAttackSkills { get; private set; }
        [field: SerializeField] public List<SkillProfile> SuperAbilitySkills { get; private set; }

        #region Setters
        public void SetStartingWeapon(SkillProfile profile, int playerIndex)
        {
            StartingSkills[playerIndex].Weapon = profile;
        }

        public void SetStartingChargeAttack(SkillProfile profile, int playerIndex)
        {
            StartingSkills[playerIndex].ChargeAttack = profile;
        }

        public void SetStartingSuperAbility(SkillProfile profile, int playerIndex)
        {
            StartingSkills[playerIndex].SuperAbility = profile;
        }
        #endregion

        #region Getters
        public SkillProfile GetStartingWeapon(int playerIndex)
        {
            return StartingSkills[playerIndex].Weapon;
        }

        public SkillProfile GetStartingChargeAttack(int playerIndex)
        {
            return StartingSkills[playerIndex].ChargeAttack;
        }

        public SkillProfile GetStartingSuperAbility(int playerIndex)
        {
            return StartingSkills[playerIndex].SuperAbility;
        }

        internal void SetProfile(SkillProfile skillProfile, int playerIndex, UpgradeTypes upgradeTypes)
        {
            if(skillProfile == null)
            {
                Debug.LogError("SkillProfile is null. No skill passed");
                return;
            }

            switch (upgradeTypes)
            {
                case UpgradeTypes.BaseWeapon:
                    SetStartingWeapon(skillProfile, playerIndex);
                    break;
                case UpgradeTypes.ChargeAttack:
                    SetStartingChargeAttack(skillProfile, playerIndex);
                    break;
                case UpgradeTypes.SuperAbility:
                    SetStartingSuperAbility(skillProfile, playerIndex);
                    break;
                default:
                    break;
            }
        }
        #endregion

        [System.Serializable]
        public class SkillsPackage
        {
            public SkillProfile Weapon;
            public SkillProfile ChargeAttack;
            public SkillProfile SuperAbility;
        }
    }
}
