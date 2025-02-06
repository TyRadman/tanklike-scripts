using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class CollisionEventPublisher : MonoBehaviour
    {
        public System.Action<Collider> OnTriggerEnterEvent;
        public System.Action<Collider> OnTriggerStayEvent;
        public System.Action<Collider> OnTriggerExitEvent;

        [SerializeField] private Collider _collider;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnTriggerStayEvent?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitEvent?.Invoke(other);
        }

        public void EnableCollider(bool value)
        {
            _collider.enabled = value;
        }
    }
}
