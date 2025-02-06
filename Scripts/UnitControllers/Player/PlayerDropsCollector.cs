using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using ItemsSystem;
    using Utils;
    using Sound;

    /// <summary>
    /// Handles the collection of drops.
    /// </summary>
    public class PlayerDropsCollector : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }
        private IPlayerController _playerComponents;

        [Header("Audio")]
        [SerializeField] private Audio _onCollectedAudio;

        public void SetUp(IController controller)
        {
            if (controller is not IPlayerController playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Collectable collectable))
            {
                if (collectable.CanBeCollected)
                {
                    collectable.OnCollected(_playerComponents);
                    GameManager.Instance.AudioManager.Play(_onCollectedAudio);
                    GameManager.Instance.ReportManager.ReportCollection(collectable, _playerComponents.PlayerIndex);
                }
            }
        }

        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        public void Dispose()
        {

        }

        public void Restart()
        {

        }
    }
}
