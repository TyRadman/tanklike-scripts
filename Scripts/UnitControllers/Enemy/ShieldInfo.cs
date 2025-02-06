using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class ShieldInfo : MonoBehaviour
    {
        [SerializeField] private Transform _catchingSpot;
        [SerializeField] private Transform _shootingPoint;

        public Transform GetCatchingSpot()
        {
            return _catchingSpot;
        }

        public Transform GetShootingSpot()
        {
            return _shootingPoint;
        }
    }
}
