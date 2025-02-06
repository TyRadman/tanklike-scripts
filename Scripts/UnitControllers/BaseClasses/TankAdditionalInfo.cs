using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds additional values that the tank would need for other components. 
/// I imagined it would be useful because a value like the shield's scale would differ from tank to tank, so we need a value that would accurately do the job, and I couldn't think of a better approach than having a script that holds such values.
/// </summary>
namespace TankLike.UnitControllers
{
    public class TankAdditionalInfo : MonoBehaviour, IController
    {
        [field: SerializeField] public float ShieldScale { get; private set; }

        public bool IsActive { get; private set; }

        public virtual void Activate()
        {

        }

        public virtual void Deactivate()
        {

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
    }
}