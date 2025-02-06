using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Testing.Playground
{
    using Combat.SkillTree.Upgrades;

    public class PlaygroundSpecialUpgradeButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _skillIconImage;
        [SerializeField] private Image _unlockedIconImage;

        private PlaygroundSpecialUpgradesUIController _uiController;
        private SkillUpgrade _currentUpgrade;
        private bool _isUnlocked;

        public void SetUp(PlaygroundSpecialUpgradesUIController uiController, SkillUpgrade upgrade)
        {
            _uiController = uiController;
            _currentUpgrade = upgrade;

            _skillIconImage.sprite = upgrade.MainInfo.Icon;
            _isUnlocked = false;
            _unlockedIconImage.gameObject.SetActive(_isUnlocked);

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                _isUnlocked = !_isUnlocked;
                _unlockedIconImage.gameObject.SetActive(_isUnlocked);
                if (_isUnlocked)
                {
                    _currentUpgrade.ApplyUpgrade();
                }
                else
                {
                    _currentUpgrade.CancelUpgrade();
                }
            });
        }
    }
}
