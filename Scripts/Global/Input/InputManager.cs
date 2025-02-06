using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike
{
    using System;
    using System.Linq;
    using UI;
    using UnitControllers;

    public class InputManager : MonoBehaviour, IManager
    {
        public static PlayerInputActions Controls { get; private set; }
        public bool IsActive { get; private set; }

        [SerializeField] private PlayerInputHandler _playerPrefab;

        private List<PlayerInput> _playerInputs = new List<PlayerInput>();
        private List<PlayerInputHandler> _inputHandlers = new List<PlayerInputHandler>();
        private static List<ActionMapReference> _maps = new List<ActionMapReference>();
        private InputIconsDatabase _inputIconsDatabase;
        private int _playersCount = 0;
        private bool _playersAddedManually = false;

        #region Classes and constants
        public class ActionMapReference
        {
            public ActionMap MapTag;
            public int PlayerIndex;
            public InputActionMap Map;
        }

        private const string SINGLE_PLAYER_SCHEME = "SinglePlayer";
        private const string KEYBOARD_SCHEME = "Keyboard";
        private const string CONTROLLER_SCHEME = "Controller";
        #endregion

        private void Awake()
        {
            Controls = new PlayerInputActions();
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            if (!_playersAddedManually)
            {
                SetInputHandlers();

                _maps.Clear();

                for (int i = 0; i < _inputHandlers.Count; i++)
                {
                    _playerInputs.Add(_inputHandlers[i].Playerinputs);
                    var maps = _inputHandlers[i].Playerinputs.actions.actionMaps;

                    // add the maps to the static list
                    for (int j = 0; j < maps.Count; j++)
                    {
                        ActionMapReference map = new ActionMapReference()
                        {
                            MapTag = (ActionMap)j,
                            PlayerIndex = i,
                            Map = maps[j]
                        };

                        _maps.Add(map);
                    }
                }

                Debug.Log($"Created {_playerInputs.Count} player inputs");
            }

            EnablePlayerInput();
        }

        internal void AddHandler(PlayerInputHandler handler)
        {
            if(_inputHandlers.Count == 0)
            {
                GameManager.Instance.GameData.PlayersCount = 0;
                _playersCount = 0;
            }

            if (_inputHandlers.Exists(i => i == handler))
            {
                return;
            }

            _playersCount++;
            GameManager.Instance.GameData.PlayersCount = _playersCount;
            //Debug.Log($"Players count: {GameManager.Instance.GameData.PlayersCount}");
            _playersAddedManually = true;
            _inputHandlers.RemoveAll(i => i == null);
            _inputHandlers.Add(handler);
            int playerIndex = _inputHandlers.IndexOf(handler);
            handler.PlayerIndex = playerIndex;

            _maps.RemoveAll(m => m.PlayerIndex == playerIndex);
            _playerInputs.Add(handler.Playerinputs);
            var maps = handler.Playerinputs.actions.actionMaps;

            //Debug.Log($"Added player input handler at index {playerIndex}");

            // add the maps to the static list
            for (int j = 0; j < maps.Count; j++)
            {
                ActionMapReference map = new ActionMapReference()
                {
                    MapTag = (ActionMap)j,
                    PlayerIndex = playerIndex,
                    Map = maps[j]
                };

                _maps.Add(map);
            }
        }

        internal void RemoveHandler(int index)
        {
            var handler = _inputHandlers.Find(i => i.PlayerIndex == index);

            if (handler == null)
            {
                return;
            }

            _playersCount--;
            GameManager.Instance.GameData.PlayersCount = _playersCount;

            _inputHandlers.RemoveAll(i => i == handler);
            _playerInputs.RemoveAll(i => i == handler.Playerinputs);
            _maps.RemoveAll(m => m.PlayerIndex == index);

            //Destroy(handler.gameObject);
            //Debug.Log($"Removed player input handler at index {index}");
        }

        public PlayerInputHandler GetHandler(int index)
        {
            return _inputHandlers.Find(i => i.PlayerIndex == index);
        }

        public void SetInputHandlers()
        {
            _inputHandlers.Clear();
            _playersCount = GameManager.Instance.GameData.PlayersCount;

            for (int i = 0; i < _playersCount; i++)
            {
                PlayerInputHandler player = Instantiate(_playerPrefab, transform);

                if (_inputHandlers.Exists(i => i == player))
                {
                    return;
                }

                _inputHandlers.RemoveAll(i => i == null);
                _inputHandlers.Add(player);
            }

            SetInputScheme();
        }

        private void SetInputScheme()
        {
            // if there is only one player, then add every possible device as an input device for them
            if (_playersCount == 1)
            {
                InputDevice[] singlePlayerDevices = InputSystem.devices.Where(device => device is Keyboard or Mouse or Gamepad).ToArray();

                _inputHandlers[0].Playerinputs.SwitchCurrentControlScheme("SinglePlayer", singlePlayerDevices);
            }
            else if (_playersCount == 2)
            {
                InputDevice[] keyboardSchemeDevices = InputSystem.devices.Where(device => device is Keyboard or Mouse).ToArray();
                _inputHandlers[0].Playerinputs.SwitchCurrentControlScheme("Keyboard", keyboardSchemeDevices);

                InputDevice[] controllerSchemeDevices = InputSystem.devices.Where(device => device is Gamepad).ToArray();
                _inputHandlers[1].Playerinputs.SwitchCurrentControlScheme("Controller", controllerSchemeDevices);
            }
        }


        public void Dispose()
        {
            IsActive = false;

            for (int i = _playerInputs.Count - 1; i >= 0; i--)
            {
                Destroy(_playerInputs[i]);
            }

            _playerInputs.Clear();

            for (int i = _inputHandlers.Count - 1; i >= 0; i--)
            {
                Destroy(_inputHandlers[i].gameObject);
            }

            _inputHandlers.Clear();
            GameManager.Instance.GameData.PlayersCount = 0;
        }
        #endregion

        public void EnablePlayerInput(int playerIndex = -1)
        {
            EnableInput(ActionMap.Player, playerIndex);
        }

        public void EnableUIInput(int playerIndex = -1)
        {
            EnableInput(ActionMap.UI, playerIndex);
        }

        public void EnableInput(ActionMap actionMap, int playerIndex = -1)
        {
            if (playerIndex == -1)
            {
                _playerInputs.ForEach(i => i.SwitchCurrentActionMap(actionMap.ToString()));
            }
            else
            {
                _playerInputs[playerIndex].SwitchCurrentActionMap(actionMap.ToString());
            }
        }

        public string GetButtonBindingKey(string action, int playerIndex)
        {
            // TODO: fix this shite
            //int schemeIndex = _playerInputs[playerIndex].currentControlScheme == KEYBOARD_SCHEME ? 0 : 1;
            return Controls.FindAction(action).GetBindingDisplayString(playerIndex);
        }

        public int GetButtonBindingIconIndex(string action, int playerIndex)
        {
            int schemeIndex;

            // TODO: temporary for the SinglePlayer schema
            if (_playerInputs[playerIndex].currentControlScheme == SINGLE_PLAYER_SCHEME)
            {
                schemeIndex = 0;
            }
            else
            {
                schemeIndex = _playerInputs[playerIndex].currentControlScheme == KEYBOARD_SCHEME ? 0 : 1;
            }

            string actionName = Controls.FindAction(action).name;
            return _inputIconsDatabase.GetSpriteIndexFromBinding(actionName, schemeIndex);
        }

        public int GetButtonBindingIconIndexByScheme(string action, int schemeIndex)
        {
            string actionName = Controls.FindAction(action).name;
            return _inputIconsDatabase.GetSpriteIndexFromBinding(actionName, schemeIndex);
        }

        public void DisableInputs(int playerIndex = -1)
        {
            // if the player index doesn't fall within the players' count
            if(playerIndex >= PlayersManager.PlayersCount)
            {
                return;
            }

            EnableInput(ActionMap.Empty, playerIndex);
        }

        public int GetSpriteIndexByScheme(string action, int controlScheme)
        {
            return _inputIconsDatabase.GetSpriteIndexFromBinding(action, controlScheme);
        }

        public static InputActionMap GetMap(int player, ActionMap map)
        {
            //Debug.Log("trying to get input for player " + player);
            try
            {
                if (_maps.Count == 0)
                {
                    return null;
                }

                return _maps.Find(m => m.PlayerIndex == player && m.MapTag == map).Map;
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                Debug.Log("Failed to get input for player " + player);
            }

            return null;
        }

        public void SetReferences(InputIconsDatabase iconsDatabase)
        {
            _inputIconsDatabase = iconsDatabase;
        }

        /// <summary>
        /// Get the current input scheme for the player by the player index
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public int GetInputSchemeIndex(int playerIndex)
        {
            return _playerInputs[playerIndex].currentControlScheme == KEYBOARD_SCHEME ? 0 : 1;
        }

        #region Vibration
        public void PerformVibration(Gamepad gamepad, float lowFrequency, float highFrequency, float duration)
        {
            // Set vibration
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);

            // Stop vibration after the specified duration
            StartCoroutine(StopVibrationAfterDelay(gamepad, duration));
        }

        private IEnumerator StopVibrationAfterDelay(Gamepad gamepad, float duration)
        {
            yield return new WaitForSeconds(duration);

            // Ensure the gamepad is still valid
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(0, 0); // Stop vibration
            }
        }
        #endregion
    }

    public enum ActionMap
    {
        Player = 0, UI = 1, Lobby = 2, Inventory = 3, Empty = 4, EditorInput = 5
    }
}
