using System.Collections;
using System.Collections.Generic;
using TankLike.Environment.MapMaker;
using UnityEditor;
using UnityEngine;

namespace TankLike
{

    public class CustomEditorJobController : EditorWindow
    {
        [MenuItem("Tools/Map Tiles Caching")]
        public static void ShowWindow()
        {
            // Create the Editor windows
            GetWindow<CustomEditorJobController>("Map Tiles Caching");
        }

        private void OnGUI()
        {
            // Display a button for caching surroundings
            if (GUILayout.Button("Cache Surroundings"))
            {
                // Find the first MapTiles_SO asset
                List<MapTiles_SO> maps = FindFirstMapTilesAsset();

                for (int i = 0; i < maps.Count; i++)
                {
                    if (maps[i] != null)
                    {
                        // Start caching
                        maps[i].CacheSurroundingTilesIndices();

                        // Display a confirmation dialog
                        EditorUtility.DisplayDialog("Caching Started", "Caching surroundings has been initiated for the first MapTiles_SO asset found.", "OK");
                    }
                    else
                    {
                        // Display a warning dialog if no asset is found
                        EditorUtility.DisplayDialog("Asset Not Found", "No MapTiles_SO asset found in the project.", "OK");
                    }
                }
            }
        }

        private List<MapTiles_SO> FindFirstMapTilesAsset()
        {
            // Search for all assets of type MapTiles_SO
            string[] guids = AssetDatabase.FindAssets("t:MapTiles_SO");
            List<MapTiles_SO> maps = new List<MapTiles_SO>();

            if (guids.Length > 0)
            {
                for(int i = 0; i < guids.Length; i++)
                {
                    // Load the asset
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    MapTiles_SO map = AssetDatabase.LoadAssetAtPath<MapTiles_SO>(path);
                    maps.Add(map);
                }
            }

            return maps; // Return null if no asset is found
        }
    }
}