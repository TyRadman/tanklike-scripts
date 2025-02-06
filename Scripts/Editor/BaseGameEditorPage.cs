using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace TankLike.EditorTools
{
    using Utils;

    public abstract class BaseGameEditorPage
    {
        protected GameEditorData _data;
        public abstract void OnGUI();
        public abstract EGameEditorPageTag PageTag();
        public GameEditor GameEditorInstance;

        private const string SETTINGS_DATA_NULL_MESSAGE = "No asset of type GameEditorData was found in the project files. Create one in order for the game editor to work";
        private const string OK_MESSAGE = "Yes, sir!";
        private const string TITLE = "Warning";

        public virtual void OnEnable()
        {

        }

        protected void RenderSection(string title, params Action[] renders)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            RenderHeader(title, 20, 10);
            GUILayout.BeginHorizontal();

            for (int i = 0; i < renders.Length; i++)
            {
                renders[i]?.Invoke();
            }

            GUILayout.EndHorizontal();
            GUILayout.Label("", GUILayout.Height(10));
            GUILayout.EndVertical();
        }

        protected void RenderHeader(string title, int fontSize, int beforeSpace = 20, int afterSpace = 20, bool hasBackButton = false)
        {
            GUILayout.Space(beforeSpace);

            // Render the title centered across the entire window
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.fontSize = fontSize;

            // Use a single centered layout for the title
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(title, headerStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (hasBackButton)
            {
                if (GUILayout.Button("Back", GUILayout.Width(100), GUILayout.Height(30)))
                {
                    GameEditor.OpenPage(EGameEditorPageTag.Main);
                }
            }
            //else
            //{
            //    Space(30);
            //}

            GUILayout.Space(afterSpace);
        }


        protected void Space(float spaceHeight)
        {
            GUILayout.Space(spaceHeight);
        }

        protected void Label(string labelText)
        {
            GUILayout.Label(labelText);
        }

        protected void RenderReturnButton()
        {
            if (GUILayout.Button("Return to main menu"))
            {
                GameEditor.OpenPage(EGameEditorPageTag.Main);
            }
        }

        protected GameEditorData GetData()
        {
            if (_data == null)
            {
                _data = Helper.FindAssetFromProjectFiles<GameEditorData>();

                if (_data == null)
                {
                    EditorUtility.DisplayDialog(TITLE, SETTINGS_DATA_NULL_MESSAGE, OK_MESSAGE);
                    return null;
                }
            }

            return _data;
        }
    }
}
