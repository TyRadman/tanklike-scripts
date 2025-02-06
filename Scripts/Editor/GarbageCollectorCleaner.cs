using UnityEditor;
using UnityEngine;

namespace TankLike
{
    using Utils;

    [InitializeOnLoad]
    public class GarbageCollectorCleaner : MonoBehaviour
    {

        static GarbageCollectorCleaner()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Trigger clean-up when exiting Play mode
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Cleanup();
            }
        }

        private static void Cleanup()
        {
            // Force garbage collection
            System.GC.Collect();

            // Unload unused assets
            Resources.UnloadUnusedAssets();

            // Clear the Undo stack
            //Undo.ClearAll();

            //Debug.Log("Play mode cleanup completed.".Color(Colors.LightPurple));
        }
    }
}
