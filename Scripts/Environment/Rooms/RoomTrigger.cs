using UnityEngine;

namespace TankLike.Environment
{
    using Attributes;

    public class RoomTrigger : MonoBehaviour
    {
        [field: SerializeField] public System.Action OnTriggerEnterEvent { get; set; }
        [field: SerializeField] public System.Action OnTriggerExitEvent { get; set; }

        [SerializeField, InSelf] private Collider _trigger;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitEvent?.Invoke();
        }
        public void CloseRooom()
        {
            _trigger.enabled = false;
        }
    }
}
