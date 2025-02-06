using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public abstract class UnitShooter : MonoBehaviour
    {
        public abstract List<Transform> GetShootingPoints();
        public System.Action OnTargetHit { get; protected set; }


        #region Registering and Unregistering callbacks for the OnTargetHit event
        public void RegisterOnTargetHitCallback(System.Action action)
        {
            OnTargetHit += action;
        }

        public void UnregisterOnTargetHitCallback(System.Action action)
        {
            OnTargetHit -= action;
        }
        #endregion
    }
}
