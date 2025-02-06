using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class AbilitySelectionSceneController : SceneController
    {
        private const string MAIN_MENU_SCENE = "S_MainMenu";
        private const string ABILITY_SELECTION_SCENE = "S_AbilitySelection";

        public override void SetUp()
        {
            Debug.Log("SETUP ABILITY SELECTION SCENE");
            StartCoroutine(SetupRoutine(ABILITY_SELECTION_SCENE));
        }

        protected override void SetUpManagers()
        {
            // Set current scene controller
            GameManager.Instance.SetCurrentSceneController(this);

            GameManager.Instance.ResultsUIController.gameObject.SetActive(false);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(false);
            GameManager.Instance.HUDController.gameObject.SetActive(false);
            GameManager.Instance.WorkshopController.WorkshopUI.gameObject.SetActive(false);
        }

        public override void Dispose()
        {
            Debug.Log("DISPOSE ABILITY SELECTION SCENE");
        }
    }
}
