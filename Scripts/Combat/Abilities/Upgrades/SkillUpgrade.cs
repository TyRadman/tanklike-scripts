using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using System.Text;
    using UnitControllers;

    /// <summary>
    /// The base class of skill upgrades which includes both abilities and weapons.
    /// </summary>
    public abstract class SkillUpgrade : ScriptableObject
    {
        protected const string SPECIAL_VALUES_HEADER = "Special Values";
        protected const string ROOT = Directories.SKILL_TREE + "Skill Upgrades/";
        protected const string PREFIX = "Upgrade_";

        [field: SerializeField] public UpgradeMainInfo MainInfo { get; protected set; }
        [field: SerializeField] public UpgradeTypes UpgradeType { get; set; }
        public string UpgradeProperties { get; protected set; }

        /// <summary>
        /// When set to false, this upgrade will not be displayed in the random selection pool.
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        // DO WE REALLY NEED TO CACHE IT?
        protected List<SkillProperties> _properties = new List<SkillProperties>();
        protected PlayerComponents _player;

        public virtual void SetUp(PlayerComponents player)
        {
            IsAvailable = true;
            UpgradeProperties = string.Empty;
            _player = player;
            _properties.Clear();

            //SetUpgradeProperties(player);
        }

        public virtual void SetUpgradeProperties(PlayerComponents player)
        {
            _properties.Clear();
        }

        public virtual void ApplyUpgrade()
        {
            //Debug.Log($"Applied upgrade to player {_player.PlayerIndex}. upgrade is {this.GetInstanceID()}");
            IsAvailable = false;
        }

        public virtual void CancelUpgrade()
        {
            //Debug.Log($"Canceled upgrade to player {_player.PlayerIndex}. upgrade is {this.GetInstanceID()}");
            IsAvailable = true;
        }

        internal virtual SkillUpgrade Clone()
        {
            return Instantiate(this);
        }

        #region Utilities
        internal void RefreshProperties()
        {
            SetUpgradeProperties(_player);
        }

        public virtual UpgradeMainInfo GetUpgradeMainInfo()
        {
            return MainInfo;
        }

        public virtual SkillUpgrade GetUpgrade()
        {
            return this;
        }

        public virtual string GetUpgradeProperties()
        {
            return UpgradeProperties;
        }

        public void SaveProperties()
        {
            if (_properties.Count > 0)
            {
                UpgradeProperties = GetSkillPropertiesString(_properties);
            }
        }

        /// <summary>
        /// Converts a list of skill properties to a single string.
        /// </summary>
        /// <param name="properties">The list of properties to convert</param>
        /// <returns></returns>
        public static string GetSkillPropertiesString(List<SkillProperties> properties)
        {
            StringBuilder propertiesText = new StringBuilder();

            if (properties != null)
            {
                propertiesText.Append("\n");

                for (int i = 0; i < properties.Count; i++)
                {
                    SkillProperties property = properties[i];
                    propertiesText.Append(property.GetPropertiesAsString());
                }
            }

            return propertiesText.ToString();
        }
        #endregion
    }
}
