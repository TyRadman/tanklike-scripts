using System.Collections;
using System.Collections.Generic;
using TankLike.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace TankLike.Combat.Editor
{
    [CustomEditor(typeof(AmmunitionDatabase))]
    public class AmmunitionDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add all ammunitions"))
            {
                AmmunitionDatabase database = (AmmunitionDatabase)target;
                database.ClearAmmunitionList();
                List<AmmunationData> ammunitionData = AssetUtils.GetAllInstances<AmmunationData>(true, new string[] { database.DirectoryToCover });

                foreach (AmmunationData a in ammunitionData)
                {
                    database.AddAmmunition(a);
                }

                database.FillDictionary();
                // Mark the database as dirty and save the changes
                EditorUtility.SetDirty(database);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
