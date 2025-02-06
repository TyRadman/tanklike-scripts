using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace TankLike.Loading
{
    public class LoadingScreenManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LoadingTankController _loadingPlayerPrefab;
        [SerializeField] private Transform _startingPont;
        [SerializeField] private Transform _finishingPoint;

        [Header("Settings")]
        [SerializeField] private float _minLoadingTime = 3f;

        private List<LoadingTankController> _loadingCharacters = new List<LoadingTankController>();
        private Coroutine _loadingTextFlashCoroutine;

        public void SetUp()
        {
            // TODO: get the prefab from the players database when we have multiple players
            LoadingTankController player = Instantiate(_loadingPlayerPrefab);
            player.transform.position = _startingPont.position;
            player.SetReferences(_startingPont, _finishingPoint);
            player.SetUp();

            _loadingCharacters.Add(player);
                 
            StartCoroutine(LoadingSceneRoutine());
        }

        private IEnumerator LoadingSceneRoutine()
        {
            float loadingStartTime = Time.time; // Track when loading starts.

            // Begin loading the target scene in the background
            string targetScene = GameManager.Instance.SceneLoadingManager.NextSceneToLoad;

            // TESTING: Set the target scene to the Gameplay scene
            if(string.IsNullOrEmpty(targetScene))
            {
                targetScene = Scenes.GAMEPLAY;
            }

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
            loadOperation.allowSceneActivation = false;

            // Wait until the scene is loaded.
            while (!loadOperation.isDone)
            {
                // Check if the scene is fully loaded
                if (loadOperation.progress >= 0.9f)
                {
                    // Scene is loaded but not yet activated.
                    break;
                }

                // Yield the frame to prevent blocking
                yield return null;
            }

            // Ensure the loading screen is displayed for at least the minimum loading time.
            float elapsedTime = Time.time - loadingStartTime;
            if (elapsedTime < _minLoadingTime)
            {
                yield return new WaitForSeconds(_minLoadingTime - elapsedTime); // Wait the remaining time
            }

            GameManager.Instance.FadeUIController.StartFadeIn();

            yield return new WaitForSecondsRealtime(GameManager.Instance.FadeUIController.FadeInDuration);

            // Activate the loaded scene.
            loadOperation.allowSceneActivation = true;

            // Wait for the target scene to activate before unloading the loading screen.
            while (!loadOperation.isDone)
            {
                yield return null;
            }

            GameManager.Instance.DisposeCurrentSceneController();
            GameManager.Instance.SceneLoadingManager.UnloadScene(Scenes.LOADING);
        }
    }
}
