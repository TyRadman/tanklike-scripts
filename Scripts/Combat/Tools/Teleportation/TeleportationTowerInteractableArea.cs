using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Tools
{
    using Environment;
    using System;
    using UnitControllers;
    using UI.InGame;
    using UnityEngine.Events;

    public class TeleportationTowerInteractableArea : InteractableArea, IPoolable
    {
        private InteractableMenuNavigatable _interactionMenu;
        [SerializeField] private ParticleSystem _inactiveParticles;
        [SerializeField] private ParticleSystem _activeParticles;
        [SerializeField] private Animator _animator;

        private const string SPAWN_ANIMATION = "Spawn";
        private const string DISAPPEAR_ANIMATION = "Disappear";
        private const float ACTIVE_TO_INACTIVE_TRANSITION_DURATION = 0.1f;

        public Action<IPoolable> OnReleaseToPool { get; private set; }

        private void Start()
        {
            SetUp();
        }

        public override void SetUp()
        {
            base.SetUp();
        }

        public void Activate()
        {
            _inactiveParticles.Stop();
            _activeParticles.Stop();

            _animator.Play(SPAWN_ANIMATION, -1, 0f);
            Invoke(nameof(PlayInactiveParticles), 0.4f);
        }

        public void PlayInactiveParticles()
        {
            _inactiveParticles.Play();
        }

        protected override void OnAreaEntered(PlayerComponents player)
        {
            base.OnAreaEntered(player);
            TurnOnActivePortalEffect();
        }

        protected override void OnAreaLeft(PlayerComponents player)
        {
            base.OnAreaLeft(player);
            TurnOnInactivePortalEffect();
        }

        public override void Interact(int playerIndex)
        {
            base.Interact(playerIndex);

            DisplayInteractionMenu();
        }

        private void DisplayInteractionMenu()
        {
            GameManager.Instance.InteractableAreasManager.DeactivateTextBox();

            Dictionary<TeleportationDestination, Room> visitedDestinations = GameManager.Instance.TeleportationManager.VisitedDestination;

            if (_interactionMenu == null)
            {
                _interactionMenu = GameManager.Instance.InteractableAreasManager.GetInteractableMenu();
            }

            _interactionMenu.transform.parent = _displayInteractionTextParent;
            _interactionMenu.transform.localPosition = Vector3.zero;
            _interactionMenu.gameObject.SetActive(true);

            foreach (KeyValuePair<TeleportationDestination, Room> destination in visitedDestinations)
            {
                UnityEvent buttonEvent = _interactionMenu.SetUpButton(destination.Key.DestinationName);
                buttonEvent.RemoveAllListeners();
                buttonEvent.AddListener(() =>
                {
                    if (Teleport(destination.Value))
                    {
                        GameManager.Instance.TeleportationManager.Teleport(destination.Value);
                        OnTeleported();
                        _interactionMenu.Close(_currentPlayerIndex);
                    }
                });
             
            }

            _interactionMenu.Open(_currentPlayerIndex);
            _interactionMenu.OnClosed += OnMenuClosed;
        }

        private void OnMenuClosed()
        {
            GameManager.Instance.InteractableAreasManager.ActivateTextBox(_displayInteractionTextParent, _interactionText);
        }

        private void OnTeleported()
        {    
            _hasInteractor = false;
            OnInteractorExit?.Invoke();

            StopInteraction();
            Deactivate();
        }

        public bool Teleport(Room roomToTeleportTo)
        {
            Room currentRoom = GameManager.Instance.RoomsManager.CurrentRoom;

            if (roomToTeleportTo.RoomType == currentRoom.RoomType)
            {
                _interactionMenu.DisplayMessage($"You're already in room {roomToTeleportTo.RoomType}");
                return false;
            }

            return true;
        }

        public override void StopInteraction()
        {
            base.StopInteraction();
        }

        public override void Deactivate()
        {
            _animator.Play(DISAPPEAR_ANIMATION, -1, 0f);
            _inactiveParticles.Stop();
            _activeParticles.Stop();
            Invoke(nameof(TurnOff), 1f);
        }

        public override void EnableInteraction(bool value)
        {
            base.EnableInteraction(value);
        }

        private void TurnOnActivePortalEffect()
        {
            CancelInvoke();
            _inactiveParticles.Stop();
            _activeParticles.Play();
        }

        private void TurnOnInactivePortalEffect()
        {
            _inactiveParticles.Play();
            Invoke(nameof(StopActiveEffect), ACTIVE_TO_INACTIVE_TRANSITION_DURATION);
        }

        private void StopActiveEffect()
        {
            _activeParticles.Stop();
        }

        #region Pool
        public void Init(Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }

        public void OnRequest()
        {
            GameManager.Instance.SetParentToRoomSpawnables(gameObject);
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
