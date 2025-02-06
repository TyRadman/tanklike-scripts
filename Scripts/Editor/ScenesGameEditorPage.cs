using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TankLike.EditorTools
{
    using UnityEditor.SceneManagement;
    using Utils;

    public class ScenesGameEditorPage : BaseGameEditorPage
    {
        private List<SceneAsset> _scenes = new List<SceneAsset>();
        private string[] _scenesNames = new string[0];
        private string[] _scenesPaths = new string[0];
        private int _buildIndex = 0;

        public override EGameEditorPageTag PageTag()
        {
            return EGameEditorPageTag.Scenes;
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnGUI()
        {
            RenderHeader("Game Editor - Scenes Data", 30, 20, 20, true);

            RenderSection("Scenes", RenderSelectingScenes);
        }

        private void RenderSelectingScenes()
        {
            GUILayout.Label("Select Scene");

            if (_scenes == null || _scenes.Count == 0)
            {
                _scenes = Helper.FindAllAssetFromProjectFiles<SceneAsset>("Assets/Scenes/");
            }

            int scenesCount = _scenes.Count;

            if (_scenesNames.Length == scenesCount && _scenesPaths.Length == scenesCount)
            {

            }
            else
            {
                _scenesNames = new string[scenesCount];
                _scenesPaths = new string[scenesCount];

                for (int i = 0; i < _scenes.Count; i++)
                {
                    string name = _scenes[i].name;
                    name = name.Replace("S_", "");
                    _scenesNames[i] = name;
                    _scenesPaths[i] = AssetDatabase.GetAssetPath(_scenes[i]);
                }
            }

            _buildIndex = EditorGUILayout.Popup(_buildIndex, _scenesNames);

            if (GUILayout.Button("Load Scene"))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(_scenesPaths[_buildIndex]);
                }
            }

            // play the scene without leaving the original scene
            if (GUILayout.Button("Play Scene"))
            {
                EditorSceneManager.OpenScene(_scenesPaths[_buildIndex]);
                EditorApplication.isPlaying = true;
            }
        }
    }
}
