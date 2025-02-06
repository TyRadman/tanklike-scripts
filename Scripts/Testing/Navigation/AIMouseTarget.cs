using TankLike.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Testing
{
    public class AIMouseTarget : MonoBehaviour
    {
        public System.Action OnTargetChanged;

        [SerializeField] private Transform _targetTransform;
        [SerializeField] private LayerMask _groundLayers;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _targetTransform.position = Helper.GetMouseWorldPosition(_groundLayers);
                OnTargetChanged?.Invoke();
            }
        }
    }
}
