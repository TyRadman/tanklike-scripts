using TankLike.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = "Finite Deflection", menuName = "Shot Configurations/Deflection/Finite Deflections")]
    public class FiniteDeflections : Deflection
    {
        public int MaxDeflections = 1;

        public override void SetUp(Bullet bullet)
        {
            base.SetUp(bullet);
        }

        public override void Reset(DeflectionData data, Bullet bullet)
        {
            base.Reset(data, bullet);
            FiniteDeflectionData newData = (FiniteDeflectionData)data;
            // reset the deflection number
            newData.CurrentDeflections = 0;
            newData.CanDeflect = true;
        }

        public override DeflectionData GetData()
        {
            FiniteDeflectionData data = new FiniteDeflectionData();
            data.MaxDeflectionNumber = MaxDeflections;
            return data;
        }

        public override void Deflect(SphereCollider collider, ref Vector3 movementDir, float detectionDistance, LayerMask layers, Collider other, DeflectionData deflectionData, Bullet bullet)
        {
            FiniteDeflectionData data = (FiniteDeflectionData)deflectionData;

            if (!data.CanDeflect)
            {
                return;
            }

            Vector3 startPoint = collider.transform.position - movementDir.normalized * detectionDistance;
            Vector3 endPoint = collider.transform.position + movementDir.normalized * detectionDistance;
            Debug.DrawLine(startPoint, endPoint, Color.red, 10f);

            if (Physics.SphereCast(startPoint, collider.radius, movementDir.normalized, out RaycastHit hit, detectionDistance, layers))
            {
                Debug.DrawRay((startPoint + endPoint) / 2f, hit.normal, Color.green, 10f);
                movementDir = Vector3.Reflect(movementDir, hit.normal);
                movementDir.y = 0;
                movementDir.Normalize();
                data.CurrentDeflections++;

                if (data.CurrentDeflections >= data.MaxDeflectionNumber)
                {
                    data.CanDeflect = false;
                }
            }
            else
            {
                Debug.LogError($"Why {Time.time}");
            }

            //if (Physics.SphereCast(ball.position + Vector3.up * 0.3f, collider.radius, movementDir, out RaycastHit hit, detectionDistance, layers))
            //if (Physics.Raycast(ball.position, movementDir, out RaycastHit hit))
            //{
            //    movementDir = Vector3.Reflect(movementDir, hit.normal);
            //    movementDir.y = 0;

            //    movementDir.Normalize();
            //    _currentDeflections++;

            //    if (_currentDeflections >= _maxDeflectionNumber)
            //    {
            //        _canDeflect = false;
            //        _currentDeflections = 0;
            //        // once there are no deflections left to perform, the wall will count as a hittable
            //        AddWallTagToBulletHittables();
            //    }
            //}
        }
    }

    [System.Serializable]
    public class FiniteDeflectionData : DeflectionData
    {

    }
}
