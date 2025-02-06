using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using CustomCLITool;

namespace CustomCLITool
{
    public struct CommandContext
    {
        public Dictionary<string, List<object>> CachedAssets;
    }

    public class CustomCLITool : EditorWindow
    {
        private string commandHistory = "";
        private string inputCommand = "";
        private Vector2 scrollPosition; 
        
        private CommandContext context = new CommandContext
        {
            CachedAssets = new Dictionary<string, List<object>>()
        };

        [MenuItem("Tools/Custom CLI Tool")]
        public static void ShowWindow()
        {
            GetWindow<CustomCLITool>("Custom CLI Tool");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Command Line Interface", EditorStyles.boldLabel);

            // Scrollable area for command history
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
            GUI.enabled = false; // Disable editing of history
            EditorGUILayout.TextArea(commandHistory, GUILayout.ExpandHeight(true));
            GUI.enabled = true;
            EditorGUILayout.EndScrollView();

            // Input field for new commands
            GUI.SetNextControlName("CommandInput");
            inputCommand = EditorGUILayout.TextField(inputCommand);

            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "CommandInput")
            {
                ExecuteCommand(inputCommand);
                inputCommand = ""; // Clear input field
                Repaint(); // Ensure GUI updates with the new command history
            }
        }

        private void ExecuteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return;

            // Append input command to history
            AppendToHistory($"> {command}");

            string[] splitCommand = command.Split(' ', 2);
            string commandName = splitCommand[0];
            string arguments = splitCommand.Length > 1 ? splitCommand[1] : "";

            ICommand commandHandler = CommandFactory.GetCommand(commandName);
            if (commandHandler != null)
            {
                commandHandler.Execute(arguments, ref commandHistory, context);
            }
            else
            {
                AppendToHistory($"Unknown command: {commandName}");
            }
        }

        private void AppendToHistory(string message)
        {
            commandHistory += message + "\n";
        }
    }

    public interface ICommand
    {
        void Execute(string arguments, ref string commandHistory, CommandContext context);
    }

    public static class CommandFactory
    {
        public static ICommand GetCommand(string commandName)
        {
            return commandName switch
            {
                "FindAssetOfType" => new FindAssetOfTypeCommand(),
                "Clear" => new ClearCommand(),
                "ChangeValue" => new ChangeValueCommand(),
                "SetValueForCollectionElements" => new SetValueForCollectionElementsCommand(),
                _ => null,
            };
        }
    }

    public class FindAssetOfTypeCommand : ICommand
    {
        public void Execute(string arguments, ref string commandHistory, CommandContext context)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                commandHistory += "Error: Type name is required for FindAssetOfType\n";
                return;
            }

            string[] guids = AssetDatabase.FindAssets($"t:{arguments}");
            List<object> assets = new List<object>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            if (assets.Count > 0)
            {
                context.CachedAssets[arguments] = assets;
                commandHistory += $"Found and cached {assets.Count} assets of type {arguments}\n";
            }
            else
            {
                commandHistory += $"No assets of type {arguments} were found.\n";
            }
        }
    }


    public class ClearCommand : ICommand
    {
        public void Execute(string arguments, ref string commandHistory, CommandContext context)
        {
            commandHistory = "Cleared all cached data and output.\n";
        }
    }

    public class ChangeValueCommand : ICommand
    {
        private static Dictionary<string, object> cachedVariables = new Dictionary<string, object>();

        public void Execute(string arguments, ref string commandHistory , CommandContext context)
        {
            string[] parts = arguments.Split('=');
            if (parts.Length != 2)
            {
                commandHistory += "Error: Invalid syntax for ChangeValue. Use ChangeValue {VariableName} = <Type>(Value)\n";
                return;
            }

            string variableName = parts[0].Trim();
            string valuePart = parts[1].Trim();

            if (valuePart.StartsWith("<int>(") && valuePart.EndsWith(")"))
            {
                if (int.TryParse(valuePart.Substring(6, valuePart.Length - 7), out int intValue))
                {
                    cachedVariables[variableName] = intValue;
                    commandHistory += $"Set {variableName} to {intValue} (int)\n";
                }
                else commandHistory += "Error: Invalid integer value.\n";
            }
            else if (valuePart.StartsWith("<float>(") && valuePart.EndsWith(")"))
            {
                if (float.TryParse(valuePart.Substring(8, valuePart.Length - 9), out float floatValue))
                {
                    cachedVariables[variableName] = floatValue;
                    commandHistory += $"Set {variableName} to {floatValue} (float)\n";
                }
                else commandHistory += "Error: Invalid float value.\n";
            }
            else if (valuePart.StartsWith("<string>(") && valuePart.EndsWith(")"))
            {
                string stringValue = valuePart.Substring(9, valuePart.Length - 10);
                cachedVariables[variableName] = stringValue;
                commandHistory += $"Set {variableName} to \"{stringValue}\" (string)\n";
            }
            else
            {
                commandHistory += "Error: Unsupported or invalid value type.\n";
            }
        }
    }
}