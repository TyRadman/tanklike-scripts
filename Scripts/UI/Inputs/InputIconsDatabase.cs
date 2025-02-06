using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UI
{
    [CreateAssetMenu(fileName = "InputIcons_DB_Default", menuName = Directories.UI + "Input Icons DB")]
    public class InputIconsDatabase : ScriptableObject
    {
        [SerializeField] private InputActionAsset _controls;
        [SerializeField] private List<InputIconsData> _inputIcons = new List<InputIconsData>();

        public int GetSpriteIndexFromBinding(string action, int controlSchema)
        {
            foreach (InputIconsData inputIcon in _inputIcons)
            {
                if(inputIcon.HasAction(action))
                {
                    if(controlSchema == 0)
                    {
                        return inputIcon.KeyboardSpriteIndex;
                    }
                    else
                    {
                        return inputIcon.ControllerSpriteIndex;
                    }
                }
            }

            Debug.LogError("No entry found for " + action);

            return -1;
        }

        // Step 4: Method to get and format binding display strings
        string GetFormattedBindingDisplayStrings(InputAction action)
        {
            var bindingStrings = new System.Text.StringBuilder();

            // Iterate through each binding of the action
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (i > 0)
                {
                    bindingStrings.Append("|");
                }
                // Get the display string for the binding and remove whitespaces
                string displayString = action.GetBindingDisplayString(i);
                bindingStrings.Append(displayString);
            }

            return bindingStrings.ToString();
        }
    }
}
