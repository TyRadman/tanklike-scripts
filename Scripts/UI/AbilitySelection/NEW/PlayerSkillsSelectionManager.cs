using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.Combat.SkillTree
{
    public class PlayerSkillsSelectionManager : MonoBehaviour, IManager
    {
        public bool IsActive { get; private set; }
        public Action OnSkillsSelected { get; internal set; }

        [SerializeField] private List<PlayerSkillsSelectionNavigator> _navigators;
        [SerializeField] private GameObject _content;
        [SerializeField] private SkillsSelectionLobby _lobby;

        [SerializeField] private Sprite _keyboardSprite;
        [SerializeField] private Sprite _gamepadSprite;

        [SerializeField] private Canvas _parentCanvas;
        [SerializeField] private UI.TutorialMenuUIController _tutorialMenu;
        [SerializeField] private PlayerInputManager _inputManager;

        private int _playersCount = 0;
        private int _readyPlayersCount = 0;

        public void Open()
        {
            int playersCount = GameManager.Instance.GameData.PlayersCount;

            _content.SetActive(true);

            _lobby.OnPlayerJoinedAction += OnPlayerJoined;
            _lobby.OnPlayerLeftAction += OnPlayerLeft;

            for (int i = 0; i < _navigators.Count; i++)
            {
                _navigators[i].SetUp();
            }
        }

        private void OnPlayerJoined(int index)
        {
            var closeNavigator = _navigators.Find(n => !n.IsOpened);

            if (closeNavigator == null)
            {
                Debug.LogError("No available navigators");
                return;
            }

            int navigatorIndex = _navigators.IndexOf(closeNavigator);
            closeNavigator.OnPlayerReady += OnPlayerReady;
            closeNavigator.OnPlayerNotReady += OnPlayerNotReady;
            closeNavigator.Open(navigatorIndex);

            string scheme = GameManager.Instance.InputManager.GetHandler(index).Playerinputs.currentControlScheme;
            Sprite deviceIcon = null;

            if(scheme == "Keyboard")
            {
                deviceIcon = _keyboardSprite;
            }
            else if (scheme == "Controller")
            {
                deviceIcon = _gamepadSprite;
            }
            else
            {
                Debug.LogError("Unknown control scheme: " + scheme);
            }

            closeNavigator.SetDeviceIcon(deviceIcon);
        }

        private void OnPlayerLeft(int index)
        {
            var navigator = _navigators.Find(n => n.PlayerIndex == index);

            if (navigator == null)
            {
                Debug.LogError("No navigator found for player index: " + index);
                return;
            }

            int navigatorIndex = _navigators.IndexOf(navigator);
            navigator.OnPlayerReady -= OnPlayerReady;
            navigator.OnPlayerNotReady -= OnPlayerNotReady;
            navigator.Close(navigatorIndex);
        }

        public void Close()
        {
            _content.SetActive(false);

            for (int i = 0; i < _navigators.Count; i++)
            {
                if (_navigators[i].IsOpened)
                {
                    _navigators[i].Close(i);
                }
            }

            GameManager.Instance.InputManager.EnableInput(ActionMap.Player);
            OnSkillsSelected?.Invoke();
            OnSkillsSelected = null;
            _tutorialMenu.Dispose();
            _parentCanvas.gameObject.SetActive(false);
            _inputManager.DisableJoining();
            _inputManager.enabled = false;
        }

        public void SetUp()
        {

        }

        public void Dispose()
        {

        }

        private void OnPlayerReady()
        {
            _readyPlayersCount++;

            if (_readyPlayersCount == GameManager.Instance.GameData.PlayersCount)
            {
                Close();
            }
        }

        private void OnPlayerNotReady()
        {
            _readyPlayersCount--;
        }
    }
}
