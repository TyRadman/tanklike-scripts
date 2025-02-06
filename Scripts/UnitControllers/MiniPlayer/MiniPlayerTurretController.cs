using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class MiniPlayerTurretController : MonoBehaviour, IController
    {
        public bool IsActive { get; protected set; }

        [Tooltip("The transforms that will rotate with the turret but aren't children of the turret in the hierarchy.")]
        [SerializeField] private Transform[] _turretChildren;
        private Transform _turret;

        public void SetUp(IController controller)
        {
            if (controller is not MiniPlayerComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            MiniPlayerBodyParts parts = components.BodyParts;

            _turret = parts.GetBodyPartOfType(BodyPartType.Turret).transform;
        }

        public void HandleTurretRotation(Transform target)
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

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {

        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
