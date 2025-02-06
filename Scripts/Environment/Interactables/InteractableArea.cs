using System;
using UnityEngine;
using System.Collections.Generic;

namespace TankLike.Environment
{
    using UnitControllers;
    using Attributes;

    [RequireComponent(typeof(Collider))]
    public class InteractableArea : MonoBehaviour
    {
        [field: SerializeField] public Action OnInteractorExit { get; set; }
        
        [SerializeField] protected Transform _displayInteractionTextParent;
        [SerializeField] protected string _interactionText;
        [SerializeField] private AbilityConstraint _onEnterConstraints;
        [SerializeField] protected bool _isActive;
        [SerializeField, InSelf] private Collider _collider;
        
        protected PlayerInteractions _currentInteractor;
        protected bool _hasInteractor = false;
        protected int _currentPlayerIndex;

        public virtual void SetUp()
        {

        }

        #region On Enter
        protected virtual void OnTriggerEnter(Collider other)
        {
            PlayerComponents player = PlayersManager.GetPlayerComponentByCollider(other);

            if (player == null)
            {
                Debug.Log("No player");
                return;
            }

            bool isValidPlayerInteractor = _currentInteractor == null && !_hasInteractor;

            // only add the player as an interactor if the interaction area doesn't have an interactor
            if (isValidPlayerInteractor)
            {
                OnAreaEntered(player);
            }
        }

        protected virtual void OnAreaEntered(PlayerComponents player)
        {
            _hasInteractor = true;

            _currentInteractor = player.PlayerInteractions;
            _currentInteractor.OnInteractionAreaEnter(this, _displayInteractionTextParent, _interactionText, _onEnterConstraints);

            // display input
            string inputName = InputManager.Controls.Player.Jump.name;
            player.InGameUIController.DisplayInput(inputName);
        }
        #endregion

        #region On Exit
        protected virtual void OnTriggerExit(Collider other)
        {
            PlayerComponents player = PlayersManager.GetPlayerComponentByCollider(other);

            if (player == null)
            {
                return;
            }

            bool isValidPlayerInteractor = _currentInteractor == player.PlayerInteractions;

            if(isValidPlayerInteractor)
            {
                OnAreaLeft(player);
            }
        }

        protected virtual void OnAreaLeft(PlayerComponents player)
        {
            _hasInteractor = false;
            OnInteractorExit?.Invoke();

            StopInteraction();

            RefreshCollider();

            player.InGameUIController.HideInput();
        }
        #endregion

        private void RefreshCollider()
        {
            _collider.enabled = false;
            Invoke(nameof(EnableCollider), Time.deltaTime);
        }

        private void EnableCollider()
        {
            _collider.enabled = true;
        }

        public virtual void StopInteraction()
        {
            if (_currentInteractor != null)
            {
                _currentInteractor.OnInteractionAreaExit(_onEnterConstraints);
                _currentInteractor.GetComponent<PlayerComponents>().InGameUIController.HideInput();
                _currentInteractor = null;
            }
        }

        public virtual void EnableInteraction(bool value)
        {
            _collider.enabled = value;
        }

        public virtual void Deactivate()
        {
            _collider.enabled = false;
        }

        public virtual void Interact(int playerIndex)
        {
            _currentPlayerIndex = playerIndex;
        }

        public AbilityConstraint GetConstraints()
        {
            return _onEnterConstraints;
        }
    }
}
