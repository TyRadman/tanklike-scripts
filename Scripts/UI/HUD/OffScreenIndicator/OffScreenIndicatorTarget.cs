using System;
using UnityEngine;

namespace TankLike.UI.HUD
{
    public class OffScreenIndicatorTarget : MonoBehaviour, IDisposable
    {
        public OffScreenIcon IconPrefab;

        public bool IsShown = false;

        /// <summary>
        /// The transform of the target that the off-screen indicator will follow.
        /// </summary>
        [HideInInspector] public Transform TargetTransform;

        /// <summary>
        /// The icon of the off screen indicator. Instantiated from the prefab in the PlaerOffScreenIndicator.
        /// </summary>
        [HideInInspector] public OffScreenIcon Icon;

        [SerializeField] private Color _iconColor = Colors.DarkGray;
        [SerializeField] private Sprite _iconSprite;
        [SerializeField] private bool _isActive = true;

        private void Awake()
        {
            TargetTransform = transform;
        }

        internal void Initialize()
        {
            if (Icon == null)
            {
                Icon = Instantiate(IconPrefab, GameManager.Instance.HUDController.OffScreenIndicator.transform);
                Icon.SetData(_iconColor, _iconSprite);
            }
        }

        private void OnEnable()
        {
            if (!_isActive)
            {
                return;
            }

            GameManager.Instance.HUDController.OffScreenIndicator.AddTarget(this);
        }

        private void OnDisable()
        {
            if (!_isActive)
            {
                return;
            }

            try
            {
                GameManager.Instance.HUDController.OffScreenIndicator.RemoveTarget(this);
            }
            catch (Exception e)
            {
                //Debug.LogError(e);
            }
        }

        public void SetColor(Color color)
        {
            _iconColor = color;

            if (Icon != null)
            {
                Icon.SetData(_iconColor, _iconSprite);
            }
        }

        internal void Disable()
        {
            _isActive = false;

            GameManager.Instance.HUDController.OffScreenIndicator.RemoveTarget(this);
        }

        internal void Enable()
        {
            _isActive = true;

            //GameManager.Instance.HUDController.OffScreenIndicator.AddTarget(this);
        }

        internal void Hide()
        {
            IsShown = false;
         
            if (Icon == null)
            {
                return;
            }

            Icon.HideIcon();
        }

        internal void Show()
        {
            IsShown = true;

            if (Icon == null)
            {
                return;
            }
            
            Icon.ShowIcon();
        }

        public void Dispose()
        {
            Destroy(Icon.gameObject);
        }
    }
}
