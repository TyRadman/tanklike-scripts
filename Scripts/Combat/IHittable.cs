using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public interface IHittable
    {
        // the point in this very case is the bullet's position rather than the contact point
        //void OnHit(Vector3 point);
    }
}
