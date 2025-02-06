using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Minimap
{
    public class MinimapIcon : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected SpriteRenderer _spriteRendererOutline;
        [SerializeField] protected MeshRenderer _meshRenderer;
        [SerializeField] protected MinimapIconType _iconType;
        [SerializeField] private bool _getIconInfo = true;
        [SerializeField] private bool _clampIcon = false;
        [SerializeField] private float _minimapSize;
        private const float OUTLINE_RATIO = 1.8f;

        private Vector3 _initialPosition;
        private Transform _minimapTransform;

        protected virtual void Start()
        {
            if (!CanInitialize())
            {
                return;
            }

            _minimapTransform = GameManager.Instance.CameraManager.MinimapCameraFollow.transform;

            if (!_getIconInfo)
            {
                return;
            }

            MinimapManager.MinimapIcon info = GameManager.Instance.MinimapManager.GetMinimapIconInfo(_iconType);

            if(info == null)
            {
                Debug.LogError($"No info found for minimap icon at {gameObject.name}");
                return;
            }

            if(_spriteRenderer == null)
            {
                Debug.LogError($"No sprite renderer at {gameObject.name}");
                return;
            }

            if (_spriteRenderer.sprite != null)
            {
                _spriteRenderer.sprite = info.Icon;
            }

            if (_spriteRendererOutline != null)
            {
                _spriteRendererOutline.sprite = info.Icon;
                _spriteRendererOutline.transform.localScale = Vector3.one * info.Size * OUTLINE_RATIO;
            }
            
            _spriteRenderer.color = info.Color;
            _spriteRenderer.transform.localScale = Vector3.one * info.Size;
        }

        private void Update()
        {
            if (!_clampIcon)
            {
                return;
            }

            _initialPosition = transform.parent.position;

            float x = Mathf.Clamp(_initialPosition.x, _minimapTransform.position.x - _minimapSize, _minimapTransform.position.x + _minimapSize);
            float z = Mathf.Clamp(_initialPosition.z, _minimapTransform.position.z - _minimapSize, _minimapTransform.position.z + _minimapSize);
            transform.position = new Vector3(x, transform.position.y, z);
        }

        public void SwitchMeshMaterial(Material material)
        {
            _meshRenderer.material = material;
        }

        protected bool CanInitialize()
        {
            return GameManager.Instance != null && GameManager.Instance.MinimapManager.IsActive;
        }
    }
}
