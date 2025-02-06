using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;

namespace TankLike
{
    public abstract class Deflection : ScriptableObject
    {
        private const string WALL_TAG = "Wall";

        public virtual void Deflect(Transform ball, SphereCollider collider, ref Vector3 movementDir, float detectionDistance, LayerMask layers)
        {

        }

        public virtual void Deflect(SphereCollider collider, ref Vector3 movementDir, float detectionDistance, LayerMask layers, Collider collsion, DeflectionData deflectionData, Bullet bullet)
        {

        }

        public virtual void SetUp(Bullet bullet)
        {

        }

        // for when the bullet is pooled
        public virtual void Reset(DeflectionData data, Bullet bullet)
        {

        }

        public virtual DeflectionData GetData()
        {
            return null;
        }
    }

    [System.Serializable]
    public class DeflectionData
    {
        public int MaxDeflectionNumber;
        public int CurrentDeflections = 0;
        public bool CanDeflect = true;
    }
}
