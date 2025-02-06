using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Attributes;

    public abstract class UnitComponents : MonoBehaviour, IController
    {
        public bool IsActive { get; protected set; }
        [field: SerializeField] public TankAlignment Alignment { get; protected set; }


        /// <summary>
        /// Returns the shooter component of the unit.
        /// </summary>
        /// <returns></returns>
        public abstract UnitShooter GetShooter();

        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Dispose()
        {

        }

        public virtual void Restart()
        {

        }

        public virtual void SetUp(IController controller)
        {

        }

        public abstract T GetUnitComponent<T>() where T : IController;

        public abstract void PositionUnit(Transform position);
    }
}
