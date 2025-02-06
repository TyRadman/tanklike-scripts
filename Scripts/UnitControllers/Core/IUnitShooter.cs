using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike
{
    public class IUnitShooter : MonoBehaviour, IController
    {
        public bool IsActive => throw new System.NotImplementedException();

        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        public void Dispose()
        {

        }

        public void Restart()
        {

        }

        public void SetUp(IController controller)
        {

        }
    }
}
