using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class SceneLoadingManager : MonoBehaviour, IManager
    {
        private Coroutine _switchSceneCoroutine;
        private Coroutine _unloadSceneCoroutine;

        public bool IsActive { get; private set; }
        public string NextSceneToLoad { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        public void SwitchScene(string currentScene, string targetScene, bool withLoading = false)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            NextSceneToLoad = targetScene;

            if (_switchSceneCoroutine != null)
            {
                StopCoroutine(_switchSceneCoroutine);
            }

            if (withLoading)
            {
                _switchSceneCoroutine = StartCoroutine(SwitchToLoadingSceneRoutine(currentScene));
            }
            else
            {
                _switchSceneCoroutine = StartCoroutine(SwitchSceneRoutine(currentScene, targetScene)); 
            }
        }

        private IEnumerator SwitchSceneRoutine(string currentScene, string targetScene)
        {
            GameManager.Instance.FadeUIController.StartFadeIn();

            yield return new WaitForSecondsRealtime(GameManager.Instance.FadeUIController.FadeInDuration);

            // Reset timeScale
            Time.timeScale = 1;

            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);

            while (!unloadOperation.isDone)
            {
                yield return null;
            }

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;

            GameManager.Instance.FadeUIController.StartFadeOut();
        }

        private IEnumerator SwitchToLoadingSceneRoutine(string currentScene)
        {
            GameManager.Instance.FadeUIController.StartFadeIn();

            yield return new WaitForSecondsRealtime(GameManager.Instance.FadeUIController.FadeInDuration);

            // Reset timeScale
            Time.timeScale = 1;

            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);

            while (!unloadOperation.isDone)
            {
                yield return null;
            }

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(Scenes.LOADING, LoadSceneMode.Additive);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;

            GameManager.Instance.FadeUIController.StartFadeOut();
        }

        public void UnloadScene(string targetScene)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            if (_unloadSceneCoroutine != null)
            {
                StopCoroutine(_unloadSceneCoroutine);
            }

            _unloadSceneCoroutine = StartCoroutine(UnloadSceneRoutine(targetScene));
        }

        private IEnumerator UnloadSceneRoutine(string targetScene)
        {          
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(targetScene);

            while (!unloadOperation.isDone)
            {
                yield return null;
            }

            yield return null;

            GameManager.Instance.FadeUIController.StartFadeOut();
        }
    }
}
