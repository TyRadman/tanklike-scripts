using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using Utils;

    public class LaserRenderer : MonoBehaviour
    {
        [SerializeField] private LayerMask _layersToImpact;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _maxDistance = 20f;
        private Material _material;
        private const string LENGTH_ID = "_TilingLength";
        [SerializeField] private float _textureTilingSize = 2;

        public void RenderLaser()
        {
            Ray ray = new Ray(transform.position, transform.forward);

            if(Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _layersToImpact))
            {
                _lineRenderer.SetPosition(0, transform.position);
                _lineRenderer.SetPosition(1, hit.point);

                float laserLength = Vector3.Distance(transform.position, hit.point);

                if(_material == null)
                {
                    _material = _lineRenderer.material;
                }

                Vector2 textureSeizRange = new Vector2(1f, _maxDistance / _textureTilingSize);
                float tilingSize = textureSeizRange.Lerp(Mathf.InverseLerp(0f, _maxDistance, laserLength));
                _material.SetFloat(LENGTH_ID, tilingSize);
            }
        }
    }
}
