using TankLike.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = "No Deflection", menuName = "Shot Configurations/Deflection/No Deflections")]
    public partial class NoneDeflections : Deflection
    {
        public override void Deflect(Transform ball, SphereCollider collider, ref Vector3 movementDir, float detectionDistance, LayerMask layers)
        {
            base.Deflect(ball, collider, ref movementDir, detectionDistance, layers);
        }

        public override void SetUp(Bullet bullet)
        {
            base.SetUp(bullet);
        }

        // it returns null, because no deflection is just no deflection
        public override DeflectionData GetData()
        {
            return null;
        }
    }
}
