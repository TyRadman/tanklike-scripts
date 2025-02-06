using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.HUD
{
    using Combat;
    using TankLike.UnitControllers;
    using Utils;

    public class PlayerHUD : MonoBehaviour, IManager
    {
        public int PlayerIndex { get; set; }

        [field: SerializeField] public PlayerStatsModifiersDisplayer StatModifiersDisplayer { get; private set; }

        [Header("Parents")]
        [SerializeField] private GameObject _playerHUDParent;
        [SerializeField] private GameObject _miniPlayerHUDParent;
        [SerializeField] private Animation _parentsAnimation;
        [SerializeField] private AnimationClip _defaultParentIdleClip;
        [SerializeField] private AnimationClip _switchToMiniPlayerHUD;
        [SerializeField] private AnimationClip _switchToPlayerHUD;

        [Header("Avatar")]
        [SerializeField] private Image _avatarImage;
        
        [Header("Health")]
        [SerializeField] private SlantedHealthBar _healthBar;

        [Header("Active Skills")]
        [SerializeField] private HUDSkillIcon _superAbility;
        [SerializeField] private HUDSkillIcon _weaponAbility;
        [SerializeField] private HUDSkillIcon _holdDownAbility;
        [SerializeField] private HUDSkillIcon _boostAbility;
        [SerializeField] private PlayerHUDAnimation _weaponSwapAnimation;

        [Header("Tools")]
        [SerializeField] private ToolIcon _selectedToolIcon;
        [SerializeField] private ToolIcon _previousToolIcon;
        [SerializeField] private ToolIcon _nextToolIcon;

        [Header("Experience")]
        [SerializeField] private Image _experienceFillImage;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("Fuel")]
        [SerializeField] private SlantedBar _fuelBar;

        [Header("Energy")]
        [SerializeField] private SlantedBar _energyBar;
        [SerializeField] private TextMeshProUGUI _energyKeyText;

        [Header("Wealth")]
        [SerializeField] private TextMeshProUGUI _coinsText;

        [Header("Inventory")]
        [SerializeField] private TextMeshProUGUI _inventoryKeyText;

        [Header("Extra")]
        [SerializeField] private Sprite _emptyToolSprite;

        [Header("Defaults")]
        [SerializeField] private Sprite _emptyAbilitySprite;

        [Header("Input Keys")]
        [SerializeField] private TextMeshProUGUI _weaponSwapKeyText;
        [SerializeField] private TextMeshProUGUI _useWeaponKeyText;

        [Header("Mini Player HUD")]
        [SerializeField] private HUDSkillIcon _miniPlayerWeaponAbility;
        [SerializeField] private TextMeshProUGUI _useMiniPlayerWeaponKeyText;

        public bool IsActive { get; private set; }

        private int _selectedWeapon; // 0 = base, 1 = super

        private readonly float _healthFlashThreshold = 0.2f;

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _superAbility.SetUp();
            StatModifiersDisplayer.SetUp();

            _selectedWeapon = 0;

            _parentsAnimation.clip = _defaultParentIdleClip;
            _parentsAnimation.Play();
        }

        public void ResetAbilitiesIcons()
        {
            _superAbility.SetIconSprite(_emptyAbilitySprite);
            _weaponAbility.SetIconSprite(_emptyAbilitySprite);
            _holdDownAbility.SetIconSprite(_emptyAbilitySprite);
            _boostAbility.SetIconSprite(_emptyAbilitySprite);
        }

        public void Dispose()
        {
            IsActive = false;

            _healthBar.StopHealthBarFlash();
        }

        public void OnPlayerDeath()
        {
            _healthBar.StopHealthBarFlash();
            _superAbility.StopPulseAnimation();
            _weaponAbility.SetFillAmount(0f);
            _superAbility.SetFillAmount(0f);
        }
        #endregion

        public void Enable()
        {
            _playerHUDParent.SetActive(true);
            _miniPlayerHUDParent.SetActive(true);
            _superAbility.OnResumed();
            _healthBar.OnResumed();
        }

        public void Disable()
        {
            _playerHUDParent.SetActive(false);
            _miniPlayerHUDParent.SetActive(false);
            _superAbility.OnPaused();
            _healthBar.OnPaused();
        }

        #region Health
        public void SetupHealthBar(int maxHealth)
        {
            if (_healthBar == null)
            {
                return;
            }

            // _healthBar.SetMaxSize(maxHealth / highestHealthPossible, true); // TODO: let's see if we're gonna update max health visually too
            _healthBar.SetMaxHealth(1, true);

            _healthBar.StopHealthBarFlash();
        }

        public void UpdateHealthBar(HealthChangeArgs args)
        {
            if(!args.UpdateBar)
            {
                return;
            }

            float currentHealth = args.CurrentHP;
            float maxHealth = args.MaxHP;
            float lastCurrentHealth = args.LastHP;

            if (_healthBar == null)
            {
                return;
            }

            if(currentHealth <= 0f)
            {
                ResetHealthBar();
                GameManager.Instance.HUDController.DamageScreenUIController.HideLowHealthOverlay(PlayerIndex);
                return;
            }

            _healthBar.SetValue((float)lastCurrentHealth, (float)currentHealth, (float)maxHealth);

            // If we're healing
            if(lastCurrentHealth < currentHealth)
            {
                _healthBar.ResetDamageBar();
            }

            if ((float)currentHealth / (float)maxHealth <= _healthFlashThreshold)
            {
                GameManager.Instance.HUDController.DamageScreenUIController.ShowLowHealthOverlay(PlayerIndex);
                _healthBar.StartHealthBarFlash();
            }
            else
            {
                if(lastCurrentHealth > currentHealth)
                {
                    GameManager.Instance.HUDController.DamageScreenUIController.ShowDamageScreen(PlayerIndex);
                }

                GameManager.Instance.HUDController.DamageScreenUIController.HideLowHealthOverlay(PlayerIndex);
                _healthBar.StopHealthBarFlash();
            }

            //_healthFillImage.fillAmount = (float)currentHealth / (float)maxHealth;
        }

        public void ResetHealthBar()
        {
            if (_healthBar == null)
            {
                return;
            }

            _healthBar.StopHealthBarFlash();
            _healthBar.ResetValue();
        }
        #endregion

        #region Experience
        public void ResetExperienceBar()
        {
            if (_experienceFillImage == null)
            {
                return;
            }

            _experienceFillImage.fillAmount = 0f;
        }

        public void UpdateExperienceBar(int currentExperience, int maxExperience)
        {
            if (_experienceFillImage == null)
            {
                return;
            }

            _experienceFillImage.fillAmount = (float)currentExperience / (float)maxExperience;
        }

        public void UpdateLevelText(int currentLevel)
        {
            if (_levelText == null)
            {
                return;
            }

            _levelText.text = currentLevel.ToString();
        }
        #endregion

        #region Skills
        #region Super Ability
        public void SetSuperAbilityInfo(Sprite icon, string key)
        {
            _superAbility.SetIconSprite(icon);
            _superAbility.SetKey(key);
        }

        public void SetSuperAbilityChargeAmount(float chargeAmount, int playerIndex)
        {
            _superAbility.SetFillAmount(chargeAmount);
        }

        public void OnAbilityFullyCharged()
        {
            _superAbility.PlayPulseAnimation();
        }

        public void OnAbilityChargeEmptied()
        {
            _superAbility.StopPulseAnimation();
        }
        #endregion

        #region Weapon
        public void SetWeaponInfo(Sprite icon, string key)
        {
            _weaponAbility.SetIconSprite(icon);
            //_weaponAbility.SetKey(key);
        }

        public void SetUseWeaponKey(string key)
        {
            _useWeaponKeyText.text = key;
        }

        public void SetWeaponChargeAmount(float amount)
        {
            _weaponAbility.SetFillAmount(1 - amount);
        }

        public void OnWeaponCooldownFinished()
        {
            _weaponAbility.PlayReadyAnimation();
        }
        #endregion

        #region Hold down
        public void SetHoldDownInfo(Sprite icon, string key)
        {
            _holdDownAbility.SetIconSprite(icon);
            _holdDownAbility.SetKey(key);
        }

        public void SetHoldDownChargeAmount(float amount, int playerIndex)
        {
            _holdDownAbility.SetFillAmount(amount);
            //_holdDownAbility.PlayAnimation(amount <= 0f);
        }
        #endregion

        #region Boost
        public void SetBoostInfo(Sprite icon, string key)
        {
            _boostAbility.SetIconSprite(icon);
            _boostAbility.SetKey(key);
        }
        #endregion
        #endregion

        #region Tools
        public void UpdateTools(Tool activeTool, Tool previousTool, Tool nextTool)
        {
            _selectedToolIcon.SetUp(activeTool);

            if (previousTool != null) _previousToolIcon.SetUp(previousTool);
            else _previousToolIcon.ResetIcon(_emptyToolSprite);

            if (nextTool != null) _nextToolIcon.SetUp(nextTool);
            else _nextToolIcon.ResetIcon(_emptyToolSprite);
        }

        public void UpdateActiveTool(string amountText)
        {
            int amount = int.Parse(amountText);
            _selectedToolIcon.SetAmount(amount);
        }

        public void SetToolsKeys(string selected, string previous, string next)
        {
            _selectedToolIcon.SetKey(selected);
            _previousToolIcon.SetKey(previous);
            _nextToolIcon.SetKey(next);
        }

        public void SetActiveToolIconAsEmpty()
        {
            _selectedToolIcon.ResetIcon(_emptyToolSprite);
        }

        #endregion

        #region Wealth
        public void UpdateCoinsText(int amount)
        {
            if (_coinsText == null) return;

            _coinsText.text = amount.ToString();
        }
        #endregion

        #region Inventory
        public void SetInventoryKey(string key)
        {
            _inventoryKeyText.text = key;
        }
        #endregion

        #region Fuel
        public void UpdateFuelBar(float currentFuel, float maxFuel)
        {
            if (_fuelBar == null)
            {
                return;
            }

            _fuelBar.UpdateBar(currentFuel, maxFuel);
        }
        #endregion

        #region Energy
        public void UpdateEnergyBar(float currentEnergy, float maxEnergy)
        {
            if (_energyBar == null)
            {
                return;
            }

            _energyBar.UpdateBar(currentEnergy, maxEnergy);
        }

        public void SetEnergyKey(string key)
        {
            _energyKeyText.text = key;
        }
        #endregion

        #region Avatar
        public void SetPlayerAvatar(Sprite avatar)
        {
            _avatarImage.sprite = avatar;
        }
        #endregion

        #region Weapon Swapping
        public void SetWeaponSwapKey(string key)
        {
            _weaponSwapKeyText.text = key;
        }

        public void OnBaseWeaponEquipped()
        {
            if(_selectedWeapon != 0)
            {
                _weaponSwapAnimation.OnBaseWeaponEquipped();
                _selectedWeapon = 0;
            }
        }

        public void OnSuperAbilityEquipped()
        {
            if (_selectedWeapon != 1)
            {
                _weaponSwapAnimation.OnSuperAbilityEquipped();
                _selectedWeapon = 1;
            }
        }
        #endregion

        #region Mini Player
        public void SwitchToMiniPlayerHUD()
        {
            _parentsAnimation.clip = _switchToMiniPlayerHUD;
            _parentsAnimation.Play();
        }

        public void SwitchToPlayerHUD()
        {
            _parentsAnimation.clip = _switchToPlayerHUD;
            _parentsAnimation.Play();
        }

        public void SetMiniPlayerWeaponChargeAmount(float amount)
        {
            _miniPlayerWeaponAbility.SetFillAmount(1 - amount);
        }

        public void OnMiniPlayerWeaponCooldownFinished()
        {
            _miniPlayerWeaponAbility.PlayReadyAnimation();
        }

        public void SetMiniPlayerWeaponInfo(Sprite icon)
        {
            _miniPlayerWeaponAbility.SetIconSprite(icon);
        }

        public void SetUseMiniPlayerWeaponKey(string key)
        {
            _useMiniPlayerWeaponKeyText.text = key;
        }
        #endregion
    }
}
