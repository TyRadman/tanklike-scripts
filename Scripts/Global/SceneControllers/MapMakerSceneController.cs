using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class MapMakerSceneController : SceneController
    {
        private const string MAPMAKER_SCENE_NAME = "S_MapMaker";

        public override void SetUp()
        {
            StartCoroutine(SetupRoutine(MAPMAKER_SCENE_NAME));
        }

        protected override void SetUpManagers()
        {
            // Set current scene controller
            GameManager.Instance.SetCurrentSceneController(this);

            // Enable the main menu screen only
            GameManager.Instance.ResultsUIController.gameObject.SetActive(false);
            GameManager.Instance.EffectsUIController.gameObject.SetActive(false);
            GameManager.Instance.HUDController.gameObject.SetActive(false);
            GameManager.Instance.WorkshopController.WorkshopUI.gameObject.SetActive(false);
            GameManager.Instance.ShopsManager.gameObject.SetActive(false);
            GameManager.Instance.Inventory.gameObject.SetActive(false);
            GameManager.Instance.PauseMenuManager.gameObject.SetActive(false);

            GameManager.Instance.CameraManager.CameraObject.SetActive(false);
        }

        public override void Dispose()
        {

        }
    }
}
