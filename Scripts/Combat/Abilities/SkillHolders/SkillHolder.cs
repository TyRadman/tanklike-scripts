using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using Combat.SkillTree;
    using Combat.SkillTree.Upgrades;
    using TankLike.Utils;
    using UnitControllers;

    public abstract class SkillHolder : ScriptableObject
    {
        [SerializeField] protected string _name;
        [SerializeField] [TextArea(3, 10)] protected string _description;
        [SerializeField] private Sprite _icon;

        protected TankComponents _components;
        public string Name => _name;
        public string Description => _description;

        public virtual Sprite GetIcon()
        {
            if(_icon == null)
            {
                Debug.LogError($"No icon at skill holder {name}");
            }

            return _icon;
        }

        public virtual string GetName()
        {
            return _name;
        }

        public virtual string GetDescription()
        {
            return _description;
        }

        /// <summary>
        /// Caches all the skill properties in the ability for when they're needed by the skill tree. Only the skill tree and its components can call this method.
        /// </summary>
        public abstract void PopulateSkillProperties();

        public abstract List<SkillProperties> GetProperties(); 

        /// <summary>
        /// Applies and adds the skill to the unit passed. This is only called by the skill tree. The only exception would be if enemies could gain abilities in runtime
        /// </summary>
        public abstract void ApplySkill(TankComponents components);

        /// <summary>
        /// Equips the skill to the unit component.
        /// </summary>
        /// <param name="components">The unit that will equip the skill.</param>
        public abstract void EquipSkill(TankComponents components);

        /// <summary>
        /// Upgrades the skill holder and its ability
        /// </summary>
        /// <param name="upgrade"></param>
        public abstract void UpgradeSkill(SkillUpgrade upgrade, PlayerComponents player);

        #region Utilities
        public void AddSkillProperty(List<SkillProperties> properties, string propertyName, float value, Color valueColor, string valueUnit)
        {
            SkillProperties property = new SkillProperties()
            {
                Name = propertyName,
                Value = value.ToString(),
                DisplayColor = valueColor,
                UnitString = valueUnit
            };

            properties.Add(property);
        }

        public string GetPropertiesText()
        {
            string propertiesText = string.Empty;
            List<SkillProperties> properties = GetProperties();

            if (properties != null)
            {
                for (int i = 0; i < properties.Count; i++)
                {
                    SkillProperties property = properties[i];
                    string unit = property.UnitString;
                    string amount = $"{property.Value} {unit}".Color(property.DisplayColor);
                    propertiesText += $"{property.Name}: {amount}";
                    propertiesText += "\n";
                }
            }

            return propertiesText;
        }
        #endregion
    }
}
