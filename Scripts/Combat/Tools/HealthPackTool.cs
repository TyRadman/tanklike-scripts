using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = NAME_PREFIX + "HealthPack", menuName = ASSET_MENU_ROOT + "Health Pack")]
    public class HealthPackTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private int _healthAmountPerPack = 30;
        private TankHealth _tankHealth;

        public override void SetUp(TankComponents tankTransform)
        {
            base.SetUp(tankTransform);

            _tankHealth = tankTransform.GetComponent<TankHealth>();
        }

        public override void UseTool()
        {
            base.UseTool();

            DamageInfo damageInfo = DamageInfo.Create()
                .SetDamage(-_healthAmountPerPack)
                .Build();

            // we pass null for now, so we don't need to know who did the healing for a given tank
            _tankHealth.TakeDamage(damageInfo);
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }

    }
}
