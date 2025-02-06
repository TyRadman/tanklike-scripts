using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.EditorTools
{
    using Combat;
    using System;
    using Utils;

    public class PlayerGameEditorPage : BaseGameEditorPage
    {
        private AmmunitionDatabase _ammunitionDB;
        private PlayerTempInfoSaver _tempInfoSaver;

        public override EGameEditorPageTag PageTag()
        {
            return EGameEditorPageTag.Players;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _ammunitionDB = Helper.FindAssetFromProjectFiles<AmmunitionDatabase>();
        }

        public override void OnGUI()
        {
            RenderHeader("Game Editor - Players Data", 30, 20, 20, true);

            RenderSection("Players", RenderPlayersNumber);
            RenderSection("Databases", RenderRefreshDataBases);

            EditorUtility.SetDirty(_data);
        }

        private void RenderRefreshDataBases()
        {
            if (GUILayout.Button("Add all ammunitions to DB"))
            {
                if (_ammunitionDB == null)
                {
                    _ammunitionDB = Helper.FindAssetFromProjectFiles<AmmunitionDatabase>();
                }

                _ammunitionDB.ClearAmmunitionList();
                List<AmmunationData> ammunitionData = AssetUtils.GetAllInstances<AmmunationData>(true, new string[] { _ammunitionDB.DirectoryToCover });

                foreach (AmmunationData b in ammunitionData)
                {
                    _ammunitionDB.AddAmmunition(b);
                }

                // Mark the database as dirty and save the changes
                EditorUtility.SetDirty(_ammunitionDB);
                AssetDatabase.SaveAssets();
            }
        }

        private void RenderPlayersNumber()
        {
            Space(10f);
            Label("Number of players");
            _data = GetData();
            _data.PlayersCount = GUILayout.Toolbar(_data.PlayersCount - 1, new string[] { "1 Player", "2 Players" }) + 1;
            EditorUtility.SetDirty(_data);
            Space(10f);
        }
    }
}
