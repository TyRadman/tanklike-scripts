using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TankLike.Tutorial
{
    using Utils;

    [CreateAssetMenu(fileName = "TutorialInstruction", menuName = Directories.TUTORIAL + "Instruction")]
    public class TutorialInstructionAsset : ScriptableObject
    {
        [field: SerializeField, TextArea(2, 10)] public string Message { get; private set; }
        [SerializeField] private string[] _inputs;

        private const string INPUT_PLACE_HOLDER = "<#>";

        /// <summary>
        /// Returns the message with input icons.
        /// </summary>
        /// <param name="indexIncrement">0 for keyboard icons and 1 for controller icons.</param>
        /// <returns></returns>
        public string GetMessage(int indexIncrement)
        {
            string message = Message;

            for (int i = 0; i < _inputs.Length; i++)
            {
                int inputIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndexByScheme(_inputs[i], indexIncrement);
                string inputText = Helper.GetInputIcon(inputIndex);

                int indexOfPlaceholder = message.IndexOf(INPUT_PLACE_HOLDER);
                message = message.Remove(indexOfPlaceholder, INPUT_PLACE_HOLDER.Length);
                message = message.Insert(indexOfPlaceholder, inputText);
            }

            return message;
        }
    }
}
