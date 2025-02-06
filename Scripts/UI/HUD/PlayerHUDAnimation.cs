using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.HUD
{
    using Utils;

    public class PlayerHUDAnimation : MonoBehaviour
    {
        [SerializeField] private Animation _weaponSwapAnimation;
        [SerializeField] private AnimationClip _showSuperAbility;
        [SerializeField] private AnimationClip _showBaseWeapon; 
        
        public void OnBaseWeaponEquipped()
        {
             this.PlayAnimation(_weaponSwapAnimation, _showBaseWeapon);
        }

        public void OnSuperAbilityEquipped()
        {
            this.PlayAnimation(_weaponSwapAnimation, _showSuperAbility);
        }
    }
}
