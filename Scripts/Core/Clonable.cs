using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [System.Serializable]
    /// <summary>
    /// A generic class that holds a clone of a scriptable object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Clonable<T> where T : ScriptableObject
    {
        [SerializeField] private T _reference;

        /// <summary>
        /// The clone of the original scriptable object.
        /// </summary>
        [HideInInspector] public T Instance;

        public void Initiate()
        {
            if (_reference != null)
            {
                Instance = Object.Instantiate(_reference);
            }
        }

        /// <summary>
        /// Sets the original scriptable object.
        /// </summary>
        /// <param name="original"></param>
        public void SetOriginal(T original)
        {
            _reference = original;
            Initiate();
        }

        public T GetOriginal()
        {
            return _reference;
        }

        public static implicit operator T(Clonable<T> cloneWrapper)
        {
            return cloneWrapper.Instance;
        }


        public static implicit operator Clonable<T>(T original)
        {
            Clonable<T> cloneWrapper = new Clonable<T>();
            cloneWrapper.SetOriginal(original);
            return cloneWrapper;
        }
    }
}
