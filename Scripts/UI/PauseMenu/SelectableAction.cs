using UnityEngine;
using UnityEngine.Events;

namespace TankLike.UI
{
    [System.Serializable]
    public class SelectableAction
    {
        [HideInInspector] public string Name;
#if UNITY_EDITOR
        [HideInInspector] [ReadOnly()] public Direction Direction;
#else
        [HideInInspector] public Direction Direction;
#endif
        public UnityEvent Action;

        public void AddListener(UnityAction action)
        {
            Action.AddListener(action);
        }

        public void RemoveAllListeners()
        {
            Action.RemoveAllListeners();
        }
    }
}
