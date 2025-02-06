using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using Utils;

    public class CoroutinesManager : MonoBehaviour, IManager
    {
        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;
            StopAllExternalCoroutines();
        }
        #endregion

        public Coroutine StartExternalCoroutine(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        public void StartExternalCoroutine(IEnumerator coroutine, Coroutine coroutineToStore)
        {
            coroutineToStore = StartCoroutine(coroutine);
        }

        public void StopExternalCoroutine(Coroutine coroutineToStop)
        {
            this.StopCoroutineSafe(coroutineToStop);
        }

        public void StopAllExternalCoroutines()
        {
            StopAllCoroutines();
        }
    }
}
