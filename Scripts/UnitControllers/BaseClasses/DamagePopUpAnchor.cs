using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class DamagePopUpAnchor : MonoBehaviour
    {
        [field: SerializeField] public Transform AnchorTransform { get; private set; }
        [SerializeField] private float _offsetRadius = 0.5f;

        public Vector3 Anchor
        {
            get
            {
                Vector3 circle = Random.insideUnitCircle * _offsetRadius;
                Vector3 offset = new Vector3(circle.x, 0f, circle.y);
                return AnchorTransform.position + offset;
            }
        }
    }
}
