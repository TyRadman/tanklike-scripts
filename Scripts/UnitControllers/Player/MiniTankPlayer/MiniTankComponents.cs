using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using Attributes;
    using Minimap;
    using UnitControllers;
    using Utils;

    public class MiniTankComponents : MonoBehaviour, IController
    {
        [field: SerializeField, InSelf] public TankBodyParts TankBodyParts { private set; get; }
        [SerializeField] protected UnitMinimapIcon _minimapIcon;
        public System.Action OnPlayerRevived { get; set; }

        public int PlayerIndex { get; private set; } = 0;

        private bool _isDisposed = false;

        /// <summary>
        /// For events that need to take place after the player is activated.
        /// </summary>
        public System.Action OnPlayerActivated { get; set; }
        /// <summary>
        /// For events that need to take place only once after the player is activated.
        /// </summary>
        public System.Action OnPlayerActivatedOnce { get; set; }
        public bool IsActive { get; set; }
        protected List<IController> _components = new List<IController>();

        protected void SetUpController(IController controller, System.Type fallBackType = null)
        {
            // Check for null and set fall back type for logging
            if (controller == null)
            {
                Helper.LogSetUpNullReferences(fallBackType);
                return;
            }

            controller.SetUp(this);
        }

        protected void AddComponentToList<T>(T controller) where T : IController
        {
            if (controller == null)
            {
                Helper.LogSetUpNullReferences(typeof(T));
                return;
            }

            _components.Add(controller);
        }

        public void SetUp(IController controller = null)
        {
            _isDisposed = false;
            AddComponentToList(TankBodyParts);

            // set up all the components
            _components.ForEach(c => c.SetUp(this));
        }

        public void SetIndex(int index)
        {
            PlayerIndex = index;
        }

        public void SpawnMinimapIcon()
        {
            _minimapIcon.SpawnIcon();
        }

        #region IController
        public void Activate()
        {
            _components.ForEach(c => c.Activate());
            OnPlayerActivated?.Invoke();

            if (OnPlayerActivatedOnce != null)
            {
                OnPlayerActivatedOnce.Invoke();
                OnPlayerActivatedOnce = null;
            }
        }

        public void Deactivate()
        {
            _components.ForEach(c => c.Deactivate());
        }

        public void Restart()
        {
            _components.ForEach(c => c.Restart());
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _components.ForEach(c => c.Dispose());
        }
        #endregion
    }
}
