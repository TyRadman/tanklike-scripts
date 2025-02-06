using System;
using System.Reflection;

namespace CustomCLITool
{
    public class SetValueForCollectionElementsCommand : ICommand
    {
        public void Execute(string arguments, ref string commandHistory, CommandContext context)
        {
            string[] parts = arguments.Split('=', 2);
            if (parts.Length != 2)
            {
                commandHistory += "Error: Invalid syntax for SetValueForCollectionElements. Use SetValueForCollectionElements {VariableName} = <Type>(Value)\n";
                return;
            }

            string variableName = parts[0].Trim();
            string valuePart = parts[1].Trim();

            foreach (var collection in context.CachedAssets.Values)
            {
                foreach (var asset in collection)
                {
                    var assetType = asset.GetType();
                    var member = FindMemberInHierarchy(assetType, variableName);

                    if (member == null)
                    {
                        commandHistory += $"Error: Variable {variableName} does not exist on asset of type {assetType} or its hierarchy.\n";
                        continue;
                    }

                    try
                    {
                        if (member is FieldInfo field)
                        {
                            SetFieldValue(field, asset, valuePart, ref commandHistory, variableName);
                        }
                        else if (member is PropertyInfo property)
                        {
                            SetPropertyValue(property, asset, valuePart, ref commandHistory, variableName);
                        }
                    }
                    catch (Exception ex)
                    {
                        commandHistory += $"Error: Failed to set value for {variableName} on asset. {ex.Message}\n";
                    }
                }
            }
        }

        private MemberInfo FindMemberInHierarchy(Type type, string memberName)
        {
            while (type != null)
            {
                var field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    return field;
                }

                var property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null)
                {
                    return property;
                }

                type = type.BaseType;
            }
            return null;
        }

        private void SetFieldValue(FieldInfo field, object asset, string valuePart, ref string commandHistory, string variableName)
        {
            if (valuePart.StartsWith("<int>(") && valuePart.EndsWith(")"))
            {
                if (int.TryParse(valuePart.Substring(6, valuePart.Length - 7), out int intValue))
                {
                    field.SetValue(asset, intValue);
                    commandHistory += $"Set {variableName} to {intValue} (int) for asset {asset}\n";
                }
                else
                {
                    commandHistory += "Error: Invalid integer value.\n";
                }
            }
            else if (valuePart.StartsWith("<float>(") && valuePart.EndsWith(")"))
            {
                if (float.TryParse(valuePart.Substring(8, valuePart.Length - 9), out float floatValue))
                {
                    field.SetValue(asset, floatValue);
                    commandHistory += $"Set {variableName} to {floatValue} (float) for asset {asset}\n";
                }
                else
                {
                    commandHistory += "Error: Invalid float value.\n";
                }
            }
            else if (valuePart.StartsWith("<string>(") && valuePart.EndsWith(")"))
            {
                string stringValue = valuePart.Substring(9, valuePart.Length - 10);
                field.SetValue(asset, stringValue);
                commandHistory += $"Set {variableName} to \"{stringValue}\" (string) for asset {asset}\n";
            }
            else
            {
                commandHistory += "Error: Unsupported or invalid value type.\n";
            }
        }

        private void SetPropertyValue(PropertyInfo property, object asset, string valuePart, ref string commandHistory, string variableName)
        {
            if (!property.CanWrite)
            {
                commandHistory += $"Error: Property {variableName} is read-only.\n";
                return;
            }

            if (valuePart.StartsWith("<int>(") && valuePart.EndsWith(")"))
            {
                if (int.TryParse(valuePart.Substring(6, valuePart.Length - 7), out int intValue))
                {
                    property.SetValue(asset, intValue);
                    commandHistory += $"Set {variableName} to {intValue} (int) for asset {asset}\n";
                }
                else
                {
                    commandHistory += "Error: Invalid integer value.\n";
                }
            }
            else if (valuePart.StartsWith("<float>(") && valuePart.EndsWith(")"))
            {
                if (float.TryParse(valuePart.Substring(8, valuePart.Length - 9), out float floatValue))
                {
                    property.SetValue(asset, floatValue);
                    commandHistory += $"Set {variableName} to {floatValue} (float) for asset {asset}\n";
                }
                else
                {
                    commandHistory += "Error: Invalid float value.\n";
                }
            }
            else if (valuePart.StartsWith("<string>(") && valuePart.EndsWith(")"))
            {
                string stringValue = valuePart.Substring(9, valuePart.Length - 10);
                property.SetValue(asset, stringValue);
                commandHistory += $"Set {variableName} to \"{stringValue}\" (string) for asset {asset}\n";
            }
            else
            {
                commandHistory += "Error: Unsupported or invalid value type.\n";
            }
        }
    }

}