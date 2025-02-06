using UnityEngine;

namespace TankLike.Combat
{
    using UnitControllers;

    public interface IDamageable
    {
        bool IsInvincible { get; }
        bool IsDead { get; }
        Transform Transform { get; }
        void TakeDamage(DamageInfo damageInfo);
        void Die();
        DamagePopUpAnchor PopUpAnchor { get; }
    }
}
