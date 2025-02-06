using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Combat.Abilities;
    using Utils;
    using Sound;
    using TankLike.Combat.SkillTree.Upgrades;
    using UI.HUD;
    using TankLike.Combat.SkillTree;

    public class PlayerShooter : TankShooter, IInput, IDisplayedInput, IEquippableWeapon
    {
        public bool IsWeaponEquipped { get; set; } = false;

        private PlayerComponents _playerComponents;
        private PlayerHUD _HUD;
        private Audio _emptyAmmoSound;
        private SkillProfile _startingWeaponProfile;

        public override void SetUp(IController controller)
        {
            PlayerComponents components = controller as PlayerComponents;

            if (components == null)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = components;

            base.SetUp(_playerComponents);

            _emptyAmmoSound = GameManager.Instance.Constants.Audios.OnEmptyAmmoSound;
            _HUD = GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex];

            _startingWeaponProfile = GameManager.Instance.StartingSkillsDB.GetStartingWeapon(_playerComponents.PlayerIndex);

            if(_startingWeaponProfile != null)
            {
                _startingWeaponProfile.RegisterSkill(_playerComponents);
                _startingWeaponProfile.EquipSkill(_playerComponents);
            }
            else
            {
                Debug.Log($"No starting weapon found for player {_playerComponents.PlayerIndex}");
            }
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Shoot.name).performed += HandleShootInput;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap playerMap = InputManager.GetMap(playerIndex, ActionMap.Player);
            playerMap.FindAction(c.Player.Shoot.name).performed -= HandleShootInput;
        }

        public void UpdateInputDisplay(int playerIndex)
        {
            int shootActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(
                InputManager.Controls.Player.Shoot.name, playerIndex);

            GameManager.Instance.HUDController.PlayerHUDs[playerIndex].SetWeaponInfo(_currentWeapon.GetIcon(), Helper.GetInputIcon(shootActionIconIndex));
            GameManager.Instance.HUDController.PlayerHUDs[playerIndex].SetUseWeaponKey(Helper.GetInputIcon(shootActionIconIndex));
        }
        #endregion

        private bool CanUseShootInput()
        {
            return _canShoot && IsActive && !_isOnCoolDown && !IsConstrained && IsWeaponEquipped;
        }

        private void HandleShootInput(InputAction.CallbackContext context)
        {
            if (!CanUseShootInput())
            {
                return;
            }

            //if (CanShoot(true))
            //{
            //    HandleVibration(context);
            //}

            // if there is no ammo left, player the sound effect and return
            if (!_playerComponents.Overheat.HasEnoughShots(1))
            {
                GameManager.Instance.AudioManager.Play(_emptyAmmoSound);
                return;
            }

            Shoot();
        }

        private void HandleVibration(InputAction.CallbackContext context)
        {
            Gamepad gamepad = context.control.device as Gamepad;
            GameManager.Instance.InputManager.PerformVibration(gamepad, 0.7f, 0.2f, 0.1f);
        }

        #region Add and Equip Weapon
        public override void EquipSkill(SkillHolder holder)
        {
            base.EquipSkill(holder);

            if (holder is not WeaponHolder weaponHolder)
            {
                Helper.LogWrongSkillHolder(gameObject.name, typeof(WeaponHolder).Name, holder.name);
                return;
            }

            _playerComponents.Overheat.UpdateCrossHairBars(_currentWeaponHolder.Weapon);
            UpdateInputDisplay(_playerComponents.PlayerIndex);
        }
        #endregion

        protected override void EnableShooting()
        {
            StartCoroutine(EnableShootingProcess());
        }

        private IEnumerator EnableShootingProcess()
        {
            float timer = 0f;
            _HUD.SetWeaponChargeAmount(1f);

            while (timer < _coolDownTime)
            {
                timer += Time.deltaTime;
                _HUD.SetWeaponChargeAmount(1 - timer / _coolDownTime);
                yield return null;
            }

            _HUD.OnWeaponCooldownFinished();
            _isOnCoolDown = false;
        }

        private void SetUpShootingEvent()
        {
            OnShootStarted += _playerComponents.CrosshairController.PlayShootingAnimation;
            OnShootStarted += _playerComponents.Overheat.ReduceAmmoBarByOne;
        }

        public override void UpgradeSkill(BaseWeaponUpgrade weaponUpgrade)
        {
            base.UpgradeSkill(weaponUpgrade);

            //_currentWeaponHolder.Weapon.Upgrade(weaponUpgrade);
            //ReEquipSkill();
        }

        #region Weapon Equipment
        public void Equip()
        {
            IsWeaponEquipped = true;

            _playerComponents.CrosshairController.ShowDots();
            _playerComponents.CrosshairController.EnableCrosshair(true);
        }

        public void Unequip()
        {
            IsWeaponEquipped = false;

            _playerComponents.CrosshairController.HideDots();
            _playerComponents.CrosshairController.EnableCrosshair(false);
        }

        public bool CanEquip()
        {
            return !_isOnCoolDown;
        }

        public bool CanUnequip()
        {
            return !_isOnCoolDown;
        }
        #endregion

        #region IController
        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Restart()
        {
            base.Restart();
            SetUpInput(_playerComponents.PlayerIndex);
            SetUpShootingEvent();
        }

        public override void Dispose()
        {
            base.Dispose();
            DisposeInput(_playerComponents.PlayerIndex);
            OnShootStarted = null;
        }
        #endregion
    }
}
