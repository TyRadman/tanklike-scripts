using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Utils
{
    public class Pool<T> where T : IPoolable
    {
        private List<IPoolable> _pool;

        private System.Func<T> _onCreateNewInstance;
        private System.Action<T> _onRequest;
        private System.Action<T> _onRelease;
        private System.Action<T> _onClear;

        public Pool(System.Func<T> createNewInstance, System.Action<T> onRequest, System.Action<T> OnRelease, System.Action<T> onClear, int preFillCount = 0)
        {
            _onCreateNewInstance = createNewInstance;
            _onRequest = onRequest;
            _onRelease = OnRelease;
            _onClear = onClear;
            _pool = new List<IPoolable>();

            for (int i = 0; i < preFillCount; i++)
            {
                T obj = _onCreateNewInstance();
                obj.Init(ReleaseObject);
                ReleaseObject(obj);
            }
        }

        public string GetPoolList()
        {
            string list = "";

            for (int i = 0; i < _pool.Count; i++)
            {
                list += _pool[i].ToString() + " ";
            }

            return list;
        }

        public T RequestObject(Vector3? position = null, Quaternion? rotation = null)
        {
            IPoolable obj;

            if (_pool.Count != 0)
            {
                obj = _pool[0];
                _pool.RemoveAt(0);
            }
            else
            {
                obj = _onCreateNewInstance();
                obj.Init(ReleaseObject);
            }

            if (position != null)
            {
                obj.transform.position = position.Value;
            }

            if (rotation != null)
            {
                obj.transform.rotation = rotation.Value;
            }

            _onRequest((T)obj);

            return (T)obj;
        }

        public void ReleaseObject(IPoolable obj)
        {
            _onRelease((T)obj);
            if (!_pool.Contains(obj))
            {
                _pool.Add(obj);
            }
        }

        public void Clear()
        {
            foreach (var obj in _pool)
            {
                if (_onClear != null && !obj.Equals(null))
                {
                    _onClear((T)obj);
                }
            }

            _pool.Clear();
        }
    }
}
