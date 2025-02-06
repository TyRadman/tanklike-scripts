using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.EditorTools
{
    public class GameEditor : EditorWindow
    {
        private static Texture _windowIcon;

        public static List<BaseGameEditorPage> Pages = new List<BaseGameEditorPage>()
        {
            new MainGameEditorPage(), new EnemiesGameEditorPage(), new ScenesGameEditorPage(),
            new PlayerGameEditorPage(), new LevelGameEditorPage()
        };

        public BaseGameEditorPage SelectedPage;

        private void OnEnable()
        {
            Pages.ForEach(p => p.GameEditorInstance = this);
            SelectedPage = Pages.Find(p => p.PageTag() == EGameEditorPageTag.Main);
            SelectedPage.OnEnable();
        }

        public static void OpenPage(EGameEditorPageTag pageTag)
        {
            GetWindow<GameEditor>().SelectedPage = Pages.Find(p => p.PageTag() == pageTag);
            GetWindow<GameEditor>().SelectedPage.OnEnable();
        }

        [MenuItem("Tanklike/Game Editor")]
        public static void ShowWindow()
        {
            GameEditor window = GetWindow<GameEditor>("Game Editor");
            _windowIcon = GetIcon("Assets/UI/EditorIcons/T_Controller.png");
            window.titleContent = new GUIContent("Game Editor", _windowIcon);
        }

        public static Texture GetIcon(string path)
        {
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(path);

            if(texture == null)
            {
                Debug.LogError($"No icon found at path: {path}");
            }

            return texture;
        }

        private void OnGUI()
        {
            SelectedPage.OnGUI();
        }
    }
}
