using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class Bootstrapper: MonoBehaviour
    {
        const string BOOTSTRAP_SCENE = "S_Bootstrap";
        const string NAVIGATION_SCENE = "S_Navigation";
        const string CHARACTERS_SCENE = "S_Characters";

        // scenes that don't need the bootstrapper
        private static readonly List<string> _excludedScenes = new List<string>()
        {
            "S_Bootstrap", "S_Navigation", "S_Characters", "S_MapMaker"
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Bootstrap()
        {
            // Loop through the currently loaded scenes
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                // If the bootstrap scene is already loaded, or if the current scene doesn't need the bootstrapper return
                if (_excludedScenes.Contains(scene.name))
                {
                    return;
                }

                // Load the bootstrap scene additively
                //Debug.Log("Load bootstrap scene");
                SceneManager.LoadScene(BOOTSTRAP_SCENE, LoadSceneMode.Additive);
            }
        }
    }
}
