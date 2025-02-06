using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

namespace TankLike.UI.MainMenu
{
    public class MainMenuSettingsManager : SubMenu
    {
        private const float DIFFICULTY_INCREMENT = 0.5f;
        [SerializeField] private GameEditorData _gameData;
        [Header("Difficulty")]
        [SerializeField] private Image _difficultyBar;
        private float _difficulty = 0f;
        [Header("Starting Room")]
        [SerializeField] private TextMeshProUGUI _startRoomText;
        private RoomType _startRoomType;

        private void Awake()
        {
            // get the current difficulty
            _difficulty = _gameData.Difficulty;
            _difficultyBar.fillAmount = _difficulty;
            // get the current start room
            _startRoomType = _gameData.StartRoomType;
            string text = Regex.Replace(_startRoomType.ToString(), "(?<!^)([A-Z])", " $1");
            _startRoomText.text = text;
        }

        public void OnDifficultyChanged(bool isIncrement)
        {
            float amountToAdd = isIncrement ? DIFFICULTY_INCREMENT : -DIFFICULTY_INCREMENT;
            _difficulty = Mathf.Clamp01(_difficulty + amountToAdd);
            print(_difficulty);
            _difficultyBar.fillAmount = _difficulty;
            // update difficulty
            _gameData.Difficulty = _difficulty;
        }

        public void OnStartRoomChanged(bool isNextRoom)
        {
            int currentRoom = (int)_startRoomType;
            // make sure the value doesn't exceed the max enum values range
            currentRoom = (currentRoom + (isNextRoom? 1 : -1)) % System.Enum.GetValues(typeof(RoomType)).Length;
            _startRoomType = (RoomType)currentRoom;
            string text = Regex.Replace(_startRoomType.ToString(), "(?<!^)([A-Z])", " $1");
            _startRoomText.text = text;
            _gameData.StartRoomType = _startRoomType;
        }
    }
}
