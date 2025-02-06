using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public enum PoolObjectType
    {
        NONE,
        VFX_ELEMENT_FIRE,
    }

    public class PoolObjectLoader : MonoBehaviour
    {
        public static PoolObject InstantiatePrefab(PoolObjectType objType)
        {
            GameObject obj = null;

            switch (objType)
            {             
                case PoolObjectType.VFX_ELEMENT_FIRE:
                    {
                        obj = Instantiate(Resources.Load("VFX_Element_Fire", typeof(GameObject)) as GameObject);
                        break;
                    }
            }

            return obj.GetComponent<PoolObject>();
        }
    }
}
