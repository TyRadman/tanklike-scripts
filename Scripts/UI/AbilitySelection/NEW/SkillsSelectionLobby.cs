using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike
{
    public class SkillsSelectionLobby : MonoBehaviour
    {
        [System.Serializable]
        public struct StartingMenus
        {
            public GameObject SkillsSelectionMenu;
            public GameObject LobbyMenu;
        }

        [SerializeField] private List<StartingMenus> _menus;

        public System.Action<int> OnPlayerJoinedAction { get; set; }
        public System.Action<int> OnPlayerLeftAction { get; set; }

        public void OnPlayerJoined(PlayerInput player)
        {
            //Debug.Log("Player joined");

            PlayerInputHandler handler = player.GetComponent<PlayerInputHandler>();
            GameManager.Instance.InputManager.AddHandler(handler);
            OnPlayerJoinedAction?.Invoke(handler.PlayerIndex);
        }

        public void OnPlayerLeft(PlayerInput player)
        {
            //Debug.Log("Player left");

            int index = player.GetComponent<PlayerInputHandler>().PlayerIndex;
            OnPlayerLeftAction?.Invoke(index);
            GameManager.Instance.InputManager.RemoveHandler(index);
        }
    }
}
