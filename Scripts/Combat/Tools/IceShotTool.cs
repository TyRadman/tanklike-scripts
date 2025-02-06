using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = NAME_PREFIX + "IceShot", menuName = ASSET_MENU_ROOT + "Ice Shot")]
    public class IceShotTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private Weapon _shot;
        private PlayerShooter _playerShooter;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            _playerShooter = components.Shooter as PlayerShooter;
            _shot.SetUp(components);
        }

        public override void UseTool()
        {
            base.UseTool();

            //_playerShooter.SetCustomShot(_shot);
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }
    }
}
