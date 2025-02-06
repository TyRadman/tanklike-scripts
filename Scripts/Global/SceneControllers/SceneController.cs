using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public abstract class SceneController : MonoBehaviour
    {
        public abstract void SetUp();
        
        public abstract void Dispose();

        protected abstract void SetUpManagers();

        protected IEnumerator SetupRoutine(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);

            while (!scene.isLoaded)
            {
                yield return null;
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            SetUpManagers();
        }

        private void OnEnable()
        {
            SetUp();
        }
    }
}
