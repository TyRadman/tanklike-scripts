using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public interface IPausable
    {
        void OnPaused();
        void OnResumed();
    }
}
