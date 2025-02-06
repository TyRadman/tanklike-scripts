using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class PlayerDetector : MonoBehaviour
    {
        [field: SerializeField] public bool IsActive { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            OnPlayerEntered();
        }

        private void OnPlayerEntered()
        {

        }

        private void OnTriggerExit(Collider other)
        {
            OnPlayerExited();
        }

        private void OnPlayerExited()
        {

        }
    }
}
