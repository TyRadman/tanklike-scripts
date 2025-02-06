using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using TankLike.Loading;

    public class LoadingSceneController : SceneController
    {
        [SerializeField] private LoadingScreenManager _loadingScreenManager;

        public override void SetUp()
        {
            //Debug.Log("SETUP LOADING SCENE");
            StartCoroutine(SetupRoutine(Scenes.LOADING));
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
            GameManager.Instance.Inventory.gameObject.SetActive(false);
            GameManager.Instance.PauseMenuManager.gameObject.SetActive(false);

            _loadingScreenManager.SetUp();
        }

        public override void Dispose()
        {
            //Debug.Log("DISPOSE LOADING SCENE");
        }
    }
}
