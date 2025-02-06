using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Utils;
using TankLike;

namespace TankLike
{
    public class DebugManager : MonoBehaviour
    {
        [System.Serializable]
        public class Debugs
        {
            public CD.DebugType Type;
            public bool IsActive = false;
        }

        [SerializeField] private List<Debugs> _debugs;

        public bool IsTypeDebugged(CD.DebugType type)
        {
            if(!_debugs.Exists(d => d.Type == type))
            {
                Debug.Log($"Debug type {type} isn't defined in the DebugManager");
                return false;
            }

            return _debugs.Find(d => d.Type == type).IsActive;
        }
    }
}

public static class CD
{
    public static void Log(this MonoBehaviour monoBehaviour, string message, DebugType type)
    {
        if (!GameManager.Instance.DebugManager.IsTypeDebugged(type)) return;

        Debug.Log(message);
    }

    public static void Log(this MonoBehaviour monoBehaviour, string message, Color color, DebugType type)
    {
        if (!GameManager.Instance.DebugManager.IsTypeDebugged(type)) return;

        Debug.LogFormat($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{message}</color>");
    }

    public enum DebugType
    {
        Tools, PlayerHealth, Elements, ShopUI, LevelGeneration
    }
}
