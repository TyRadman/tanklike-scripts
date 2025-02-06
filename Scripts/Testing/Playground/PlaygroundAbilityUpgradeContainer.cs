using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.Testing.Playground
{
    using Combat.SkillTree.Upgrades;
    using UnitControllers;

    public class PlaygroundAbilityUpgradeContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _upgradeNameText;
        [SerializeField] private Image _upgradeIcon;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TextMeshProUGUI _upgradeLevelText;
        [SerializeField] private GameObject _maxLevelButton;

        private PlayerComponents _currentPlayer;
        private int _currentUpgradeLevel;

        public void SetUp(PlayerComponents player)
        {
            _currentPlayer = player;
            _currentUpgradeLevel = 0;
            _upgradeLevelText.text = _currentUpgradeLevel.ToString();
            _upgradeButton.gameObject.SetActive(false);
            _maxLevelButton.SetActive(true);
        }

        public void Init(SkillUpgradesContainer upgradesContainer)
        {
            _upgradeNameText.text = upgradesContainer.MainInfo.Name;
            _upgradeIcon.sprite = upgradesContainer.MainInfo.Icon;

            _currentUpgradeLevel = upgradesContainer.GetNextUpgradeIndex();
            _upgradeLevelText.text = _currentUpgradeLevel.ToString();

            if (upgradesContainer.IsMaxLevelReached())
            {
                _upgradeButton.gameObject.SetActive(false);
                _maxLevelButton.SetActive(true);
            }
            else
            {
                _upgradeButton.gameObject.SetActive(true);
                _maxLevelButton.SetActive(false);
            }

            _upgradeButton.onClick.RemoveAllListeners();
            _upgradeButton.onClick.AddListener(() =>
            {
                UpgradeAbility(upgradesContainer);
            });
        }

        private void UpgradeAbility(SkillUpgradesContainer upgradesContainer)
        {
            _currentUpgradeLevel++;
            _upgradeLevelText.text = _currentUpgradeLevel.ToString();
            upgradesContainer.ApplyUpgrade();

            if (upgradesContainer.IsMaxLevelReached())
            {
                _upgradeButton.gameObject.SetActive(false);
                _maxLevelButton.SetActive(true);
            }
            else
            {
                _upgradeButton.gameObject.SetActive(true);
                _maxLevelButton.SetActive(false);
            }
        }
    }
}
