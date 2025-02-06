using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = "Infinite Deflection", menuName = "Shot Configurations/Deflection/Infinite Deflections")]
    public class InfiniteDeflections : Deflection
    {
        public override void Deflect(Transform ball, SphereCollider collider, ref Vector3 movementDir, float detectionDistance, LayerMask layers)
        {
            if (Physics.SphereCast(ball.position + Vector3.up * 0.3f, collider.radius, movementDir, out RaycastHit hit, detectionDistance, layers))
            {
                movementDir = Vector3.Reflect(movementDir, hit.normal);
                movementDir.y = 0;
                movementDir.Normalize();
            }
        }
    }
}
