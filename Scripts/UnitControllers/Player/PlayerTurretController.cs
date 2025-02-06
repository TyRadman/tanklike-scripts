using UnityEngine;

namespace TankLike.UnitControllers
{
    public class PlayerTurretController : TankTurretController
    {
        [Tooltip("The transforms that will rotate with the turret but aren't children of the turret in the hierarchy.")]
        [SerializeField] private Transform[] _turretChildren;

        public override void HandleTurretRotation(Transform target)
        {
            Vector3 direction = (target.position - _turret.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion currentRotation = _turret.localRotation;
            Quaternion rotation = Quaternion.Euler(currentRotation.eulerAngles.x, targetRotation.eulerAngles.y, currentRotation.eulerAngles.z);
            _turret.localRotation = rotation;

            for (int i = 0; i < _turretChildren.Length; i++)
            {
                Transform child = _turretChildren[i];
                child.localRotation = rotation;
            }
        }
    }
}
