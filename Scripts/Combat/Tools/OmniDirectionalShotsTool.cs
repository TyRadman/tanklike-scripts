using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using UnitControllers;
    using Cam;

    [CreateAssetMenu(fileName = NAME_PREFIX + "OmniDirectionalShot", menuName = ASSET_MENU_ROOT + "Omni-directional Shot")]
    public class OmniDirectionalShotsTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] protected Weapon _normalShot;
        [SerializeField] private int _bulletsNumber = 3;
        [SerializeField] private float _angleBetweenShots = 15f;
        [SerializeField] private float _startingAngle;
        [SerializeField] private float _bulletSpeed;

        private TankComponents _components;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            _components = components;
            _normalShot.SetSpeed(_bulletSpeed);
            _normalShot.SetUp(components);
        }

        public override void UseTool()
        {
            base.UseTool();

            for (int i = 0; i < _bulletsNumber; i++)
            {
                _normalShot.OnShot(_components.transform, _startingAngle + _angleBetweenShots * i);
                GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);
            }
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }
    }
}
