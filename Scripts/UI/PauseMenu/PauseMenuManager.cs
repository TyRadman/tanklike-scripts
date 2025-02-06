using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike.UI.PauseMenu
{
    using Sound;
    using UnitControllers;
    using Utils;
    using UI.Signifiers;

    public class PauseMenuManager : MonoBehaviour, IManager, ISignifiersDisplayer
    {
        public System.Action OnPaused;
        public System.Action OnResumed;

        [SerializeField] private GameObject _content;
        [SerializeField] private List<GameObject> _panels;
        [SerializeField] private MenuSelectable _firstSelectedItem;
        [Header("Panels")]
        [SerializeField] private PauseMenuSettings _settingsPanel;
        [SerializeField] private UIActionSignifiersController _menuActionSignifiersController;
        private float _lastTimeScale;
        private MenuSelectable _currentSelectable;
        private bool _isPaused = false;
        private int _currentPlayerIndex = -1;

        public bool IsActive { get; private set; }
        public ISignifierController SignifierController { get; set; }

        private const string RETURN_ACTION_TEXT = "Return";
        private const string SUBMIT_ACTION_TEXT = "Select";

        public void SetReferences()
        {
            _lastTimeScale = 1f;
            EnableFirstPanel();
            _content.SetActive(false);
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            SignifierController = _menuActionSignifiersController;
        }

        public void Dispose()
        {
            IsActive = false;

            _content.SetActive(false);

            _isPaused = false;
            _menuActionSignifiersController.ClearAllSignifiers();
        }
        #endregion

        public void Select()
        {
            if(!_isPaused)
            {
                return;
            }

            _currentSelectable.InvokeAction();
        }

        public void Navigate(Direction direction)
        {
            if (!_isPaused)
            {
                return;
            }

            // Play navigate menu audio
            AudioManager audioManager = GameManager.Instance.AudioManager;
            audioManager.Play(audioManager.UIAudio.NavigateMenuAudio);

            _currentSelectable.InvokeAction(direction);
        }

        public void SetSelectedItem(MenuSelectable selectable)
        {
            _currentSelectable = selectable;
        }

        #region Button Methods
        /// <summary>
        /// Button method: dehighlights the current button and highlight the button passed.
        /// </summary>
        /// <param name="cell"></param>
        public void HighLightSelectable(MenuSelectable cell)
        {
            // dehighlight the previous cell
            _currentSelectable.Highlight(false);
            // set the new cell and highlight it
            _currentSelectable = cell;
            cell.Highlight(true);
        }

        /// <summary>
        /// Hides the HUD, displays the pause menu, and freezes time.
        /// </summary>
        /// <param name="playerIndex"></param>
        public void PauseGame(int playerIndex)
        {
            if (_isPaused)
            {
                return;
            }

            // set the current player index to all the panels
            SetPlayerIndex(playerIndex);

            SetUpActionSignifiers(SignifierController);

            EnableFirstPanel();

            PauseGameplay();

            // show the pause menu
            _content.SetActive(true);

            // hide HUD
            GameManager.Instance.HUDController.HideHUD();

            // disable player input and enable UI input
            GameManager.Instance.InputManager.EnableUIInput();
            _isPaused = true;
            OnPaused?.Invoke();
        }

        /// <summary>
        /// Freezes the gameplay
        /// </summary>
        public void PauseGameplay()
        {
            // disable pausing for the other player if there is another player
            GameManager.Instance.PlayersManager.DisablePauseInputForOtherPlayer(_currentPlayerIndex);

            // stop time
            GameManager.Instance.ScreenFreezer.PauseFreeze();
            _lastTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            if (!_isPaused)
            {
                return;
            }

            UnpauseGameplay();

            // hide the pause menu content
            _content.SetActive(false);
            // show HUD
            //FIX HUD handling
            GameManager.Instance.HUDController.DisplayHUD();
            // hide the pause menu content
            _content.SetActive(false);
            // enable the player input and disable the UI input
            GameManager.Instance.InputManager.EnablePlayerInput();
            _isPaused = false;
            _menuActionSignifiersController.ClearAllSignifiers();
            OnResumed?.Invoke();
        }

        public void UnpauseGameplay()
        {
            GameManager.Instance.PlayersManager.EnablePauseInputForSecondPlayer(_currentPlayerIndex);

            // resume the time scale the way it was before 
            Time.timeScale = _lastTimeScale;
            GameManager.Instance.ScreenFreezer.ResumeFreeze();
        }

        public void GoToMainMenu()
        {
            GameManager.Instance.PlayersManager.GetPlayer(_currentPlayerIndex).UIController.EnablePauseMenuController(false);
            GameManager.Instance.ConfirmPanel.Init(_currentPlayerIndex, LoadMainMenuScene, OnConfirmPanelClose, "Go to main menu?");
        }

        public void OnConfirmPanelClose()
        {
            GameManager.Instance.PlayersManager.GetPlayer(_currentPlayerIndex).UIController.EnablePauseMenuController(true);
        }

        public void LoadMainMenuScene()
        {
            StartCoroutine(LoadMainMenuSceneRoutine());
        }

        private IEnumerator LoadMainMenuSceneRoutine()
        {
            // Wait a frame until the confirm panel is closed
            yield return null;

            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                PlayerComponents player = GameManager.Instance.PlayersManager.GetPlayer(i);
                player.Deactivate();
                player.Dispose();
            }

            GameManager.Instance.DisposeCurrentSceneController();
            GameManager.Instance.SceneLoadingManager.SwitchScene(SceneManager.GetActiveScene().name, Scenes.MAIN_MENU);
        }

        #endregion

            /// <summary>
            /// Displays the first panel by default (resume, settings, exit)
            /// </summary>
        private void EnableFirstPanel()
        {
            _panels.ForEach(p => p.SetActive(false));
            _panels[0].SetActive(true);

            if (_currentSelectable != null)
            {
                _currentSelectable.Highlight(false);
            }

            _currentSelectable = _firstSelectedItem;
            _currentSelectable.Highlight(true);
        }

        private void SetPlayerIndex(int playerIndex)
        {
            _settingsPanel.SetPlayerIndex(playerIndex);
            _currentPlayerIndex = playerIndex;
        }

        public void SetUpActionSignifiers(ISignifierController signifierController)
        {
            PlayerInputActions c = InputManager.Controls;

            // add the actions that are mutual between windows to the signifiers controller
            int submitActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Submit.name, _currentPlayerIndex);
            int returnActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Cancel.name, _currentPlayerIndex);
            string submitActionKey = Helper.GetInputIcon(submitActionIconIndex);
            string returnActionKey = Helper.GetInputIcon(returnActionIconIndex);

            _menuActionSignifiersController.SetUp();

            _menuActionSignifiersController.DisplaySignifier(SUBMIT_ACTION_TEXT, submitActionKey);
            _menuActionSignifiersController.DisplaySignifier(RETURN_ACTION_TEXT, returnActionKey);
        }
    }
}
