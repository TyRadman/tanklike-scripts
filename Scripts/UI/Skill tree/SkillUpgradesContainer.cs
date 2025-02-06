using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using TankLike.Utils;
    using UnitControllers;

    /// <summary>
    /// The class that holds a sequence of upgrades of a skill that are available in the order set in the inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "UC_NAME", menuName = Directories.SKILL_TREE + "Upgrades Container")]
    public class SkillUpgradesContainer : SkillUpgrade
    {
        private int _nextUpgradeIndex = 0;
        [field : SerializeField] public List<SkillUpgrade> Upgrades { get; private set; }

        public override void SetUp(PlayerComponents player)
        {
            IsAvailable = true;
            UpgradeProperties = string.Empty;
            _player = player;
            _properties.Clear();
            _nextUpgradeIndex = 0;
            Upgrades.ForEach(u => u.SetUp(player));
        }

        public override SkillUpgrade GetUpgrade()
        {
            if (_nextUpgradeIndex >= Upgrades.Count)
            {
                return null;
            }

            SkillUpgrade upgradeToReturn = Upgrades[_nextUpgradeIndex].GetUpgrade();

            _nextUpgradeIndex++;

            if (_nextUpgradeIndex >= Upgrades.Count)
            {
                IsAvailable = false;
            }

            return upgradeToReturn;
        }

        public override void ApplyUpgrade()
        {
            if (_nextUpgradeIndex >= Upgrades.Count)
            {
                return;
            }

            SkillUpgrade upgradeToReturn = Upgrades[_nextUpgradeIndex].GetUpgrade();

            _nextUpgradeIndex++;

            if (_nextUpgradeIndex >= Upgrades.Count)
            {
                IsAvailable = false;
            }

            upgradeToReturn.IsAvailable = false;
            upgradeToReturn.ApplyUpgrade();
            Debug.Log($"Applied upgrade to player {_player.PlayerIndex}. upgrade is {this.GetInstanceID()}".Color(Colors.Red));
        }

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);
            Upgrades.ForEach(u => u.SetUpgradeProperties(player));
        }

        public override string GetUpgradeProperties()
        {
            try
            {
            return Upgrades[_nextUpgradeIndex].UpgradeProperties;
            }
            catch(System.Exception e)
            {

                Debug.LogError($"No index {_nextUpgradeIndex}");
                return string.Empty;
            }
        }

        public override UpgradeMainInfo GetUpgradeMainInfo()
        {
            if (Upgrades[_nextUpgradeIndex].MainInfo.OverrideParentMainInfo)
            {
                return Upgrades[_nextUpgradeIndex].MainInfo;
            }
            else
            {
                return MainInfo;
            }
        }

        public bool IsMaxLevelReached()
        {
            return _nextUpgradeIndex == Upgrades.Count;
        }

        public int GetNextUpgradeIndex()
        {
            return _nextUpgradeIndex;
        }

        internal override SkillUpgrade Clone()
        {
            SkillUpgradesContainer clone = Instantiate(this);

            clone.Upgrades.Clear();

            for (int i = 0; i < Upgrades.Count; i++)
            {
                clone.Upgrades.Add(Upgrades[i].Clone());
            }

            return clone;
        }
    }
}
