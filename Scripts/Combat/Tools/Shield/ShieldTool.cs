using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    using UnitControllers.Shields;

    [CreateAssetMenu(fileName = NAME_PREFIX + "Shield", menuName = ASSET_MENU_ROOT + "Shiled")]
    public class ShieldTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private Shield _shieldPrefab;
        private Shield _createdShield;
        private Transform _tankTransform;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            _tankTransform = components.transform;
            // create the shield
            _createdShield = Instantiate(_shieldPrefab, components.transform);
            _createdShield.SetUp(components);
            _createdShield.transform.localPosition = Vector3.zero;
            _createdShield.SetShieldAlpha(0f);
            // set the alignment of the shield
            _createdShield.SetShieldUser(components.Alignment);
            // set up the shield's size
            _createdShield.SetSize(components.AdditionalInfo.ShieldScale);
        }

        public override void UseTool()
        {
            base.UseTool();
            _createdShield.ActivateShield();
            _tank.Invoke(nameof(DeactivateShield), _duration);
        }

        private void DeactivateShield()
        {
            _createdShield.DeactivateShield();
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }

    }
}
