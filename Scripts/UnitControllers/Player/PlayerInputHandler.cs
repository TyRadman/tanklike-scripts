using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public PlayerInputActions Controls { set; get; }
        [field: SerializeField] public PlayerInput Playerinputs { set; get; }
        private static int ConfirmationNumber = 0;
        private static int MaxConfirmationNumber = 0;
        private bool _confirmed = false;
        [SerializeField] private PlayerTempInfoSaver _playersInfoSaver;
        private InputActionMap _lobbyActionMap;
        private bool _isSetUp = false;
        public int PlayerIndex;

        //public void SetUp()
        //{
        //    _isSetUp = true;
        //    Controls = new PlayerInputActions();

        //    _lobbyActionMap = Playerinputs.actions.FindActionMap(Controls.Lobby.Get().name);
        //    _lobbyActionMap.FindAction(Controls.Lobby.Submit.name).performed += ConfirmSelection;
        //    _lobbyActionMap.FindAction(Controls.Lobby.Cancel.name).performed += ReturnButton;
        //    MaxConfirmationNumber++;

        //    if (MaxConfirmationNumber == 2)
        //    {
        //        FindObjectOfType<PlayerInputManager>().DisableJoining();
        //    }

        //    _playersInfoSaver.AddPlayerInputHandler(this);
        //}

        //public void ReturnButton(InputAction.CallbackContext _)
        //{
        //    if (_confirmed)
        //    {
        //        Deconfirm();
        //    }
        //    else
        //    {
        //        Disconnect();
        //    }
        //}

        //public void Disconnect()
        //{
        //    if (MaxConfirmationNumber < 2)
        //    {
        //        FindObjectOfType<PlayerInputManager>().EnableJoining();
        //    }

        //    _playersInfoSaver.RemovePlayerInputHandler(this);
        //    MaxConfirmationNumber--;
        //    Destroy(gameObject);
        //}

        //private void Deconfirm()
        //{
        //    ConfirmationNumber--;
        //    _confirmed = false;
        //}

        //public void ConfirmSelection(InputAction.CallbackContext _)
        //{
        //    if (_confirmed)
        //    {
        //        return;
        //    }

        //    ConfirmationNumber++;
        //    _confirmed = true;
        //    //print($"Confirmation: {ConfirmationNumber}. Max: {MaxConfirmationNumber}");

        //    if (ConfirmationNumber == MaxConfirmationNumber)
        //    {
        //        _playersInfoSaver.DisableAllInputs();
        //    }
        //}

        //private void OnDestroy()
        //{
        //    if(_lobbyActionMap == null || !_isSetUp)
        //    {
        //        return;
        //    }

        //    _lobbyActionMap.FindAction(Controls.Lobby.Submit.name).performed -= ConfirmSelection;
        //    _lobbyActionMap.FindAction(Controls.Lobby.Cancel.name).performed -= ReturnButton;
        //}
    }
}

// Reference: https://forum.unity.com/threads/manual-local-multiplayer-using-inputdevices.1295664/
// Alternative approach
// private InputActionMap _lobbyActionMap;
// guaranteed solution, but harder
//_lobbyActionMap = _input.actions.FindActionMap("Lobby");
//_lobbyActionMap.FindAction("Return").performed += Disconnect;