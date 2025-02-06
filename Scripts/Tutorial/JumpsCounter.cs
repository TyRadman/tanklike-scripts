using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TankLike.Tutorial
{
    public class JumpsCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private JumpAchievement _achievement;

        private const string MESSAGE = "Jumps: ";

        private void Awake()
        {
            _achievement.OnPlayerJumpedEvent += UpdateText;
        }

        private void UpdateText(int current, int required)
        {
            _text.text = $"{MESSAGE}\n{current}/{required}";
        }

        private void OnDestroy()
        {
            _achievement.OnPlayerJumpedEvent -= UpdateText;
        }
    }
}
