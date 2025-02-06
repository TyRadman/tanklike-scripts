using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public interface IState
    {
        void OnEnter();
        void OnUpdate();
        void OnExit();
        void OnDispose();
    }
}
