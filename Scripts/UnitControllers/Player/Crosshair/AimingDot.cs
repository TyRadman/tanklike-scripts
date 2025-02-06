using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class AimingDot : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _mesh;

        private Transform _transform;
        private Material _material;

        private const string COLOR_ID = "_BaseColor";
    
        public void SetUp()
        {
            _transform = transform;
            _material = _mesh.material;
        }

        public void SetPosition(Vector3 position)
        {
            _transform.position = position;
        }

        public void SetColor(Color color)
        {
            _material.SetColor(COLOR_ID, color);
        }

        public void Show()
        {
            _mesh.enabled = true;
        }

        public void Hide()
        {
            _mesh.enabled = false;
        }
    }
}
