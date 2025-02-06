using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public interface IEquippableWeapon
    {
        bool IsWeaponEquipped { get; set; }
        void Equip();
        void Unequip();
        bool CanEquip();
        bool CanUnequip();
    }
}
