using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.EditorTools
{
    public class MainGameEditorPage : BaseGameEditorPage
    {
        public override EGameEditorPageTag PageTag()
        {
            return EGameEditorPageTag.Main;
        }

        public override void OnEnable()
        {

        }

        public override void OnGUI()
        {
            RenderHeader("Game Editor", 30);

            RenderSection("Main Menu", RenderMainMenu);
        }

        private void RenderMainMenu()
        {
            if (GUILayout.Button("Players Data"))
            {
                GameEditor.OpenPage(EGameEditorPageTag.Players);
            }

            if (GUILayout.Button("Enemies Data"))
            {
                GameEditor.OpenPage(EGameEditorPageTag.Enemies);
            }

            if(GUILayout.Button("Scenes Data"))
            {
                GameEditor.OpenPage(EGameEditorPageTag.Scenes);
            }

            if (GUILayout.Button("Level Data"))
            {
                GameEditor.OpenPage(EGameEditorPageTag.Level);
            }
        }
    }
}
