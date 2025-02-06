using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class LobbySceneController : SceneController
    {
        private const string ABILITY_SELECTION_SCENE = "S_AbilitySelection";
        private const string LOBBY_SCENE = "S_Lobby";

        public override void SetUp()
        {
            //Debug.Log("SETUP LOBBY SCENE");
            StartCoroutine(SetupRoutine(LOBBY_SCENE));
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
            //Debug.Log("DISPOSE LOBBY SCENE");
        }
    }
}
