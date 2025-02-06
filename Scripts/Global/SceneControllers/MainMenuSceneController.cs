using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using TankLike.MainMenu;
    using UI.MainMenu;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;

    public class MainMenuSceneController : SceneController
    {
        [SerializeField] private MainMenuUIController _mainMenuUIController;
        [SerializeField] private MainMenuInputManager _inputManager;

        private const string MAIN_MENU_SCENE = "S_MainMenu";

        public override void SetUp()
        {
            StartCoroutine(SetupRoutine(MAIN_MENU_SCENE));
        }

        protected override void SetUpManagers()
        {
            // Set current scene controller
            GameManager.Instance.SetCurrentSceneController(this);

            GameManager.Instance.ResultsUIController.gameObject.SetActive(false);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(false);
            GameManager.Instance.HUDController.gameObject.SetActive(false);
            GameManager.Instance.WorkshopController.WorkshopUI.gameObject.SetActive(false);

            _inputManager.SetUp();
            _mainMenuUIController.SetUp();
        }

        public override void Dispose()
        {

        }
    }
}
