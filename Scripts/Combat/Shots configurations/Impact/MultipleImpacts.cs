using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "OnImpact_MultipleImpacts", menuName = MENU_MAIN + "Multiple Impacts")]
    public class MultipleImpacts : OnImpact
    {
        [SerializeField] private OnImpact[] _impacts;

        public override void Impact(Vector3 hitPoint, IDamageable target, int damage, LayerMask mask, Bullet bullet)
        {
            base.Impact(hitPoint, target, damage, mask, bullet);

            for (int i = 0; i < _impacts.Length; i++)
            {
                _impacts[i].Impact(hitPoint, target, damage, mask, bullet);
            }
        }
    }
}
