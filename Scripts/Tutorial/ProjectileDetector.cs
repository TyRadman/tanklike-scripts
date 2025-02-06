using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class ProjectileDetector : MonoBehaviour
    {
        public System.Action OnProjectileDetected;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Ball"))
            {
                OnProjectileDetected?.Invoke();
            }
        }
    }
}
