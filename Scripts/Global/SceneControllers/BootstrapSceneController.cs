using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class BootstrapSceneController : SceneController
    {
        private const string BOOTSTRAP_SCENE = "S_Bootstrap";
        private const string MAIN_MENU_SCENE = "S_MainMenu";

        public override void SetUp()
        {
            // Get the currently active scene
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;

            // If the currently active scene is the bootstraper, load the main menu
            if(sceneName == BOOTSTRAP_SCENE)
            {
                Debug.Log("SETUP BOOTSTRAP SCENE");

                StartCoroutine(LoadingSceneProcess());
            }
        }

        private IEnumerator LoadingSceneProcess()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(MAIN_MENU_SCENE, LoadSceneMode.Additive);

            while (!operation.isDone)
            {
                yield return null;
            }
        }

        protected override void SetUpManagers()
        {
        }

        public override void Dispose()
        {
            Debug.Log("DISPOSE BOOTSTRAP SCENE");
        }
    }
}
