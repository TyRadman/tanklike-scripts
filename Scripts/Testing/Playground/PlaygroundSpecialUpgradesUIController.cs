using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Testing.Playground
{
    using Combat.SkillTree.Upgrades;
    using Combat.SkillTree;
    using UnitControllers;

    public class PlaygroundSpecialUpgradesUIController : MonoBehaviour
    {
        [SerializeField] private List<PlaygroundSpecialUpgradeButton> _buttons;

        private PlayerComponents _currentPlayer;
        private List<SkillUpgrade> _specialUpgrades;

        public void SetUp(PlaygroundManager playgroundManager)
        {
            _currentPlayer = GameManager.Instance.PlayersManager.GetPlayer(0);
            SetUpgrades();

            _buttons.ForEach(b => b.gameObject.SetActive(false));

            for (int i = 0; i < _specialUpgrades.Count; i++)
            {
                _buttons[i].SetUp(this, _specialUpgrades[i]);
                _buttons[i].gameObject.SetActive(true);
            }
        }

        public void Init()
        {

        }

        private List<SkillUpgrade> GetSpecialUpgrades()
        {
            return _currentPlayer.Upgrades.GetSpecialUpgrades();
        }

        public void SetUpgrades()
        {
            List<SkillUpgrade> playerUpgrades = GetSpecialUpgrades();

            _specialUpgrades = new List<SkillUpgrade>();

            for (int i = 0; i < playerUpgrades.Count; i++)
            {
                if (playerUpgrades[i] == null)
                {
                    continue;
                }

                SkillUpgrade upgradeClone = playerUpgrades[i].Clone();
                upgradeClone.name = playerUpgrades[i].name + "_CLONE";
                //Debug.Log($"Cloned upgrade: {upgradeClone.GetInstanceID()}, and the original is {upgrades[i].GetInstanceID()}");
                upgradeClone.SetUp(_currentPlayer);
                _specialUpgrades.Add(upgradeClone);
            }
        }
    }
}
