using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using System;

namespace TankLike
{
    public interface IPoolable
    {
        Transform transform { get; }
        Action<IPoolable> OnReleaseToPool { get; }

        /// <summary>
        /// Initializes the IPoolable object and passes the OnRelease action to be called in the pool itself.
        /// </summary>
        void Init(Action<IPoolable> OnRelease);
        /// <summary>
        /// Used to release the IPoolable object to the pool.
        /// </summary>
        void TurnOff();
        /// <summary>
        /// Requests a new instance of the IPoolable object.
        /// </summary>
        void OnRequest();
        /// <summary>
        /// Disposing the poolable once they're used to be ready to use again. Getting them ready to be used again.
        /// </summary>
        void OnRelease();
        /// <summary>
        /// When we no longer need this pool. Meaning this pool will probably never be used in this scene.
        /// </summary>
        void Clear();
    }
}
