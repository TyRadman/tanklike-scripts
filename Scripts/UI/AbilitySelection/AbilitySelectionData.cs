using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.Combat.Abilities;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = "AbilitySelectionData", menuName = Directories.UI + "Ability Selection Data")]
    public class AbilitySelectionData : ScriptableObject
    {
        public List<Weapon> Weapons = new List<Weapon>();
        public List<WeaponHolder> WeaponHolders = new List<WeaponHolder>();
        public List<HoldAbilityHolder> HoldAbilityHolders = new List<HoldAbilityHolder>();
        public List<BoostAbilityHolder> BoostAbilityHolders = new List<BoostAbilityHolder>();
        public List<SuperAbilityHolder> SuperAbilityHolders = new List<SuperAbilityHolder>();

        public WeaponHolder GetNormalShot()
        {
            return WeaponHolders[PlayerPrefs.GetInt(nameof(WeaponHolders))];
        }        
        
        public BoostAbilityHolder GetBoostAbility()
        {
            return BoostAbilityHolders[PlayerPrefs.GetInt(nameof(BoostAbilityHolders))];

        }

        public HoldAbilityHolder GetHoldAbility()
        {
            return HoldAbilityHolders[PlayerPrefs.GetInt(nameof(HoldAbilityHolders))];
        }

        public SuperAbilityHolder GetSuperAbility()
        {
            return SuperAbilityHolders[PlayerPrefs.GetInt(nameof(SuperAbilityHolders))];
        }
    }
}
