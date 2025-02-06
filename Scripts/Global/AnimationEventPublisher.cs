using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class AnimationEventPublisher : MonoBehaviour
    {
        public System.Action OnEventFired;
        public void FireEvent()
        {
            OnEventFired?.Invoke();
        }
    }
}
