using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class PoolObject : MonoBehaviour
    {
        public PoolObjectType poolObjectType;
        public float lifetime;
        public bool HasEffects;
        [SerializeField] private PoolObjectType _turnOffEffect;
        private Coroutine turnOffRoutine;

        private void OnEnable()
        {
            if (turnOffRoutine != null)
            {
                StopCoroutine(turnOffRoutine);
            }
            if (lifetime > 0f)
            {
                turnOffRoutine = StartCoroutine(_ScheduleOff());
            }
        }

        public void TurnOff()
        {
            transform.parent = GameManager.Instance.PoolingManager.PooledObjectsParent.transform;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            GameManager.Instance.PoolingManager.AddObject(this);
        }

        IEnumerator _ScheduleOff()
        {
            //yield return new WaitUntil(() => !x.isPlaying);
            yield return new WaitForSeconds(lifetime);

            if (!GameManager.Instance.PoolingManager.PoolDictionary[poolObjectType].Contains(this.gameObject))
            {
                if (HasEffects)
                {
                    GameManager.Instance.PoolingManager.SpawnObj(_turnOffEffect, transform.position, null);
                }

                TurnOff();
            }
        }
    }
}
