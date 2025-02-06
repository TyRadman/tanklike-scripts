using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = "Tools_DB_Default", menuName = Directories.TOOLS_SETTINGS + "Tools DataBase", order = 0)]
    public class ToolsDatabase : ScriptableObject
    {
        [SerializeField] private List<ToolInfo> _tools;

        private Dictionary<string, ToolInfo> _toolsDB;

        private void OnEnable()
        {
            _toolsDB = new Dictionary<string, ToolInfo>();

            foreach (var tool in _tools)
                _toolsDB.Add(tool.GUID, tool);
        }

        public ToolInfo GetToolDataFromGUID(string guid)
        {
            if (_toolsDB.ContainsKey(guid))
                return _toolsDB[guid];

            Debug.Log("Tools DB does not contain a tool with GUID -> " + guid);
            return null;
        }

        public List<ToolInfo> GetAllTools()
        {
            return _tools;
        }
    }
}