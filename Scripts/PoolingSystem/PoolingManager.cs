using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class PoolingManager : MonoBehaviour
    {
        public Dictionary<PoolObjectType, List<GameObject>> PoolDictionary = new Dictionary<PoolObjectType, List<GameObject>>();
        public Transform PooledObjectsParent;

        public void Awake()
        {
            //PooledObjectsParent = GameObject.FindWithTag("PooledObjectParent");
        }

        public void SetUpDictionary()
        {
            PoolDictionary.Clear();

            PoolObjectType[] arr = System.Enum.GetValues(typeof(PoolObjectType)) as PoolObjectType[];

            foreach (PoolObjectType p in arr)
            {
                if (!PoolDictionary.ContainsKey(p))
                {
                    PoolDictionary.Add(p, new List<GameObject>());
                }
            }

            SetupPoolObjects();

        }

        public void Setup(Transform spawnables)
        {
            PooledObjectsParent = spawnables;

            if (PoolDictionary.Count == 0)
            {
                SetUpDictionary();
            }
        }

        public void SetupPoolObjects()
        {
            foreach (KeyValuePair<PoolObjectType, List<GameObject>> pool in PoolDictionary)
            {
                if (pool.Key == PoolObjectType.NONE)
                    continue;

                for (int i = 0; i < 3; i++)
                {
                    //Debug.Log(pool.Key);
                    //Debug.Log(pool.Value);
                    GameObject obj = PoolObjectLoader.InstantiatePrefab(pool.Key).gameObject;
                    obj.transform.parent = PooledObjectsParent.transform;
                    pool.Value.Add(obj);
                    obj.SetActive(false);
                }
            }
        }

        public GameObject GetObject(PoolObjectType objType)
        {
            if (PoolDictionary.Count == 0)
            {
                SetUpDictionary();
            }

            List<GameObject> list = PoolDictionary[objType];
            //Debug.Log(list);
            GameObject obj = null;

            if (list.Count > 0)
            {
                //obj = list.Find(p => p != null);

                //if(obj == null)
                //{
                //    obj = PoolObjectLoader.InstantiatePrefab(objType).gameObject;
                //}
                //else
                //{
                //    list.Remove(obj);
                //}

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        obj = list[i];
                        list.RemoveAt(i);
                        break;
                    }
                    if (i == list.Count - 1)
                    {
                        obj = PoolObjectLoader.InstantiatePrefab(objType).gameObject;
                    }
                }
            }
            else
            {
                obj = PoolObjectLoader.InstantiatePrefab(objType).gameObject;
            }

            return obj;
        }

        public void AddObject(PoolObject obj)
        {
            List<GameObject> list = PoolDictionary[obj.poolObjectType];
            list.Add(obj.gameObject);
            obj.gameObject.SetActive(false);
        }

        public void SpawnObj(PoolObjectType ObjectType, Vector3 position, Transform parent)
        {
            GameObject obj = GetObject(ObjectType);

            if (parent != null)
            {
                obj.transform.parent = parent;
                obj.transform.localPosition = Vector3.zero + position;
                obj.transform.localRotation = Quaternion.identity;
            }
            else
            {
                obj.transform.position = position;
                obj.transform.rotation = Quaternion.identity;
            }

            obj.SetActive(true);
        }
    }
}
