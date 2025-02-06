using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;
using TankLike.Combat.AirDrone;
using TankLike.UnitControllers;

namespace TankLike
{
    [CreateAssetMenu(fileName = NAME_PREFIX + "AirDrone", menuName = ASSET_MENU_ROOT + "Air Drone")]
    public class AirDrone_Tool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private AirDroneController _dronePrefab;
        private AirDroneController _drone;
        [SerializeField] private int _shotsPerSummon = 5;
        [SerializeField] private LayerMask _enemiesMask;
        private Transform _playerTransform;
        [SerializeField] private float _shootingCoolDown = 0.5f;

        public override void SetUp(TankComponents tank)
        {
            base.SetUp(tank);

            _playerTransform = tank.transform;
            // create the drone
            _drone = Instantiate(_dronePrefab);
            _drone.gameObject.SetActive(false);
            // set up the values for the drone
            _drone.SetUp(tank, _enemiesMask, _shootingCoolDown);
        }

        public override void UseTool()
        {
            if (_drone.IsActive) return;
            
            base.UseTool();
            _drone.transform.position = _playerTransform.position;
            _drone.StartDrone();
            _drone.SetShots(_shotsPerSummon);
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }
    }
}
