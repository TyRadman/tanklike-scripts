using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using Environment;
    using UI;
    using UI.InGame;

    public class InteractableAreasManager : MonoBehaviour, IManager
    {
        [Header("Areas Screens")]
        [SerializeField] private ToolsNavigator _toolsShop;
        [SerializeField] private InteractableTextBox _textBoxPrefab;
        [SerializeField] private InteractableMenuNavigatable _interactableMenuPrefab;

        public bool IsActive { get; private set; }

        private InteractableTextBox _textBox;
        private InteractableMenuNavigatable _interactableMenu;

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            InteractableTextBox box = Instantiate(_textBoxPrefab, transform);
            box.gameObject.SetActive(false);
            _textBox = box;

            _interactableMenu = Instantiate(_interactableMenuPrefab);
            _interactableMenu.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            IsActive = false;

            if (_textBox != null)
            {
                Destroy(_textBox.gameObject);
                _textBox = null;
            }

            if (_interactableMenu != null)
            {
                Destroy(_interactableMenu.gameObject);
                _interactableMenu = null;
            }
        }
        #endregion

        public void ActivateTextBox(Transform buttonDisplayPosition, string interactionText)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _textBox.gameObject.SetActive(true);
            _textBox.SetInteractionText(interactionText);
            _textBox.SetPosition(buttonDisplayPosition);
            _textBox.PlayOpenAnimation(true);
        }

        public void SetFeedbackText(string text, int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _textBox.SetFeedbackText(text);
        }

        public void DeactivateTextBox(int playerIndex = 0)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _textBox.PlayOpenAnimation(false);
        }

        public void OpenShop(int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            _toolsShop.Open(playerIndex);
        }

        public void OpenWorkshop(int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return;
            }

            GameManager.Instance.WorkshopController.WorkshopUI.Open(playerIndex);
        }

        public InteractableMenuNavigatable GetInteractableMenu()
        {
            if (!IsActive)
            {
                Debug.LogError($"Manager {GetType().Name} is not active, and you're trying to use it!");
                return null;
            }

            return _interactableMenu;
        }
    }
}
