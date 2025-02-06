using UnityEditor;
using UnityEngine;
using System.IO;

namespace TankLike
{

    public class CustomScriptTemplate
    {
        private const string ABILITY_TEMPLATE = "using UnityEngine;\n\nnamespace TankLike.Combat\n{\n\tpublic class #SCRIPTNAME# : Ability\n\t{\n\t\tpublic void PerformAbility()\n\t\t{\n\t\t}\n\t}\n}";
        private const string CHEAT_TEMPLATE = "using UnityEngine;\n\nnamespace TankLike.Cheats\n{\n\t[CreateAssetMenu(fileName = NAME + \"\", menuName = ROOT + \"\")]\n\tpublic class #SCRIPTNAME# : Cheat\n\t{\n\t\tpublic override void Initiate()\n\t\t{\n\n\t\t}\n\n\t\tpublic override void PerformCheat()\n\t\t{\n\n\t\t}\n\t}\n}";

        [MenuItem("Assets/Create/C# Custom Classes/C# Ability Script", false, 0)]
        public static void CreateAbilityScript()
        {
            CreateScriptFromTemplate("NewAbilityScript.cs", ABILITY_TEMPLATE);
        }

        [MenuItem("Assets/Create/C# Custom Classes/C# Cheat Class", false, 1)]
        public static void CreateCheatScript()
        {
            CreateScriptFromTemplate("NewCheatClass.cs", CHEAT_TEMPLATE);
        }

        private static void CreateScriptFromTemplate(string defaultName, string template)
        {
            string path = GetSelectedPathOrFallback() + "/" + defaultName;
            var endNameEditAction = ScriptableObject.CreateInstance<DoCreateScriptAsset>();
            endNameEditAction.Template = template;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                endNameEditAction,
                path,
                null,
                null);
        }

        private static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
                else if (Directory.Exists(path))
                {
                    break;
                }
            }
            return path;
        }

        private class DoCreateScriptAsset : UnityEditor.ProjectWindowCallback.EndNameEditAction
        {
            public string Template { get; set; }

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var fileName = Path.GetFileNameWithoutExtension(pathName);
                var fileContent = Template.Replace("#SCRIPTNAME#", fileName);
                File.WriteAllText(pathName, fileContent);
                AssetDatabase.ImportAsset(pathName);
                var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
                ProjectWindowUtil.ShowCreatedAsset(asset);
            }
        }
    }

}
