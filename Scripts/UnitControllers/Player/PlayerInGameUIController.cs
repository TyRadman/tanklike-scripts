using UnityEngine;
using TMPro;

namespace TankLike.UnitControllers
{
    using Utils;

    public class PlayerInGameUIController : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }

        [Header("References")]
        [SerializeField] private TextMeshProUGUI _superActivationText;
        [SerializeField] private Animator _animator;

        private PlayerComponents _playerComponents;
        private string _lastInputName;
        private int _playerIndex;
        private bool _isDisplayed = false;

        private readonly int ShowKey = Animator.StringToHash("Show");
        private readonly int HideKey = Animator.StringToHash("Hide");

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
            _playerIndex = _playerComponents.PlayerIndex;
            _isDisplayed = false;
        }

        public void DisplayInput(string inputName)
        {
            if (_isDisplayed)
            {
                return;
            }

            if (inputName != _lastInputName)
            {
                _lastInputName = inputName;

                int activationIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(inputName, _playerIndex);

                _superActivationText.text = Helper.GetInputIcon(activationIndex);
            }

            _isDisplayed = true;

            _animator.SetTrigger(ShowKey);
        }

        public void HideInput()
        {
            if (!_isDisplayed)
            {
                return;
            }

            _isDisplayed = false;

            _animator.SetTrigger(HideKey);
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;

            HideInput();
        }

        public void Dispose()
        {

        }

        public void Restart()
        {

        }
        #endregion
    }
}
