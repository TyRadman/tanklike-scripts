using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UnitControllers
{
    using Combat.Abilities;
    using Combat.SkillTree.Upgrades;
    using Sound;
    using Combat;
    using UI.HUD;
    using Utils;
    using TankLike.Cam;

    public class MiniPlayerShooter : UnitShooter, IInput, IDisplayedInput, IConstraintedComponent, ISkill, IController
    {
        public bool IsActive { get; protected set; }
        public List<Transform> ShootingPoints { get; set; }
        public System.Action OnShootStarted { get; set; }
        public bool IsConstrained { get; set; } = false;

        [SerializeField] protected float _shootAnimationDelay = 0f;
        [SerializeField] protected LayerMask _targetLayers;
        protected Audio _onShootAudio;

        [SerializeField] private WeaponHolder _startWeaponHolder;

        private MiniPlayerComponents _playerComponents;
        private PlayerHUD _HUD;
        private Audio _emptyAmmoSound;
        protected Weapon _currentWeapon;
        private Dictionary<WeaponHolder, WeaponHolder> _weapons = new Dictionary<WeaponHolder, WeaponHolder>();
        private System.Action<Transform, float> ShootingMethod;
        private WaitForSeconds _shootWaitForSeconds;
        protected WeaponHolder _currentWeaponHolder;
        private SkillHolder _lastWeaponHolder;
        protected float _coolDownTime = 3f;
        private bool _isOnCoolDown = false;
        private bool _canShoot = false;

        public void SetUp(IController controller)
        {
            if (controller is not MiniPlayerComponents player)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = player;

            _emptyAmmoSound = GameManager.Instance.Constants.Audios.OnEmptyAmmoSound;
            _HUD = GameManager.Instance.HUDController.PlayerHUDs[_playerComponents.PlayerIndex];

            _shootWaitForSeconds = new WaitForSeconds(_shootAnimationDelay);

            CacheShootingPoints();

            if (_startWeaponHolder != null)
            {
                AddSkill(_startWeaponHolder);
            }
            else
            {
                Debug.LogError($"No start weapon holder for {gameObject.name}");
            }
        }

        private void CacheShootingPoints()
        {
            TankTurret turret = (TankTurret)_playerComponents.BodyParts.GetBodyPartOfType(BodyPartType.Turret);
            ShootingPoints = new List<Transform>();

            if(turret.ShootingPoints.Length == 0)
            {
                Debug.LogError($"No shooting points found for {gameObject.name}");
                return;
            }

            ShootingPoints.AddRange(turret.ShootingPoints);
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

            GameManager.Instance.HUDController.PlayerHUDs[playerIndex].SetMiniPlayerWeaponInfo(_currentWeapon.GetIcon());
            GameManager.Instance.HUDController.PlayerHUDs[playerIndex].SetUseMiniPlayerWeaponKey(Helper.GetInputIcon(shootActionIconIndex));
        }
        #endregion

        private bool CanUseShootInput()
        {
            return _canShoot && IsActive && !_isOnCoolDown && !IsConstrained;
        }

        private void HandleShootInput(InputAction.CallbackContext context)
        {
            if (!CanUseShootInput())
            {
                return;
            }

            if (!_playerComponents.OverHeat.HasEnoughShots(1))
            {
                GameManager.Instance.AudioManager.Play(_emptyAmmoSound);
                return;
            }

            Shoot();
        }

        public virtual void Shoot(bool hasCoolDown = true, Transform shootingPoint = null, float angle = 0f)
        {
            if (!CanShoot(hasCoolDown))
            {
                return;
            }

            if (shootingPoint == null)
            {
                shootingPoint = ShootingPoints[0];
            }

            // TODO: remove it after fixing the air drone shooter
            OnShootStarted?.Invoke();
            _playerComponents.Animation.PlayShootAnimation();
            //PlayShootAudio();

            StartCoroutine(ShootingRoutine(shootingPoint, angle));


            if (hasCoolDown)
            {
                _isOnCoolDown = true;
                EnableShooting();
            }
        }

        protected bool CanShoot(bool hasCoolDown)
        {
            bool cooldownInPlace = _isOnCoolDown && hasCoolDown;
            return _canShoot && !cooldownInPlace && !IsConstrained;
        }

        private IEnumerator ShootingRoutine(Transform shootingPoint = null, float angle = 0f)
        {
            yield return _shootWaitForSeconds;
            ShootingMethod(shootingPoint, angle);
        }

        public virtual void DefaultShot(Transform shootingPoint = null, float angle = 0)
        {
            if (_currentWeapon == null)
            {
                return;
            }

            _currentWeapon.OnShot(shootingPoint, angle);
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);
        }

        #region Add and Equip Weapon
        public void AddSkill(SkillHolder holder)
        {
            if (holder == null)
            {
                Debug.LogError($"No weapon passed to {gameObject.name}");
                return;
            }

            if (holder is not WeaponHolder weaponHolder)
            {
                Helper.LogWrongSkillHolder(gameObject.name, typeof(WeaponHolder).Name, holder.name);
                return;
            }

            // ensure the entity doesn't hold duplicate weapons
            if (_weapons.ContainsKey(weaponHolder))
            {
                Debug.LogError($"Entity {gameObject.name} already has the weapon {weaponHolder.name}");
                return;
            }

            WeaponHolder newHolder = Instantiate(weaponHolder);
            _weapons.Add(weaponHolder, newHolder);

            Weapon weapon = Instantiate(weaponHolder.Weapon);
            newHolder.SetWeapon(weapon);

            weapon.SetUp(_playerComponents);

            if (_currentWeaponHolder == null)
            {
                EquipSkill(weaponHolder);
            }
        }

        public void EquipSkill(SkillHolder holder)
        {
            if (holder is not WeaponHolder weaponHolder)
            {
                Helper.LogWrongSkillHolder(gameObject.name, typeof(WeaponHolder).Name, holder.name);
                return;
            }

            // add the weapon the list of weapons if it was never added and set up
            if (!_weapons.ContainsKey(weaponHolder))
            {
                AddSkill(weaponHolder);
            }

            _lastWeaponHolder = weaponHolder;
            _currentWeaponHolder = _weapons[weaponHolder];
            _currentWeapon = _currentWeaponHolder.Weapon;

            _currentWeapon.SetTargetLayer(_targetLayers);
            _currentWeapon.OnWeaponShot += PlayShootingEffects;

            _coolDownTime = _currentWeapon.GetCoolDownTime();

            // set the audio if the bullet has special audios
            if (_currentWeapon.ShotAudio != null)
            {
                _onShootAudio = _currentWeapon.ShotAudio;
            }

            _playerComponents.OverHeat.UpdateCrossHairBars(_currentWeaponHolder.Weapon);
            UpdateInputDisplay(_playerComponents.PlayerIndex);
        }

        public void UpgradeSkill(BaseWeaponUpgrade weaponUpgrade)
        {
        }

        public void ReEquipSkill()
        {
            EquipSkill(_lastWeaponHolder);
        }
        #endregion

        #region Utilities
        public void PlayShootingEffects()
        {
            // TODO: finish this after implementing the mini player animation component
            //if (_playerComponents.Animation == null)
            //{
            //    return;
            //}

            //_playerComponents.Animation.PlayShootAnimation(2f); //dirty, get the speed from the weapon
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);
        }

        protected void EnableShooting()
        {
            //Debug.Log("StartCoroutine ");

            StartCoroutine(EnableShootingProcess());
        }

        private IEnumerator EnableShootingProcess()
        {
            float timer = 0f;
            _HUD.SetMiniPlayerWeaponChargeAmount(1f);

            //Debug.Log("SetMiniPlayerWeaponChargeAmount " + _coolDownTime);

            while (timer < _coolDownTime)
            {
                timer += Time.deltaTime;
                //Debug.Log("Cooldown " + (1 - timer / _coolDownTime));
                _HUD.SetMiniPlayerWeaponChargeAmount(1 - timer / _coolDownTime);
                yield return null;
            }

            _HUD.OnMiniPlayerWeaponCooldownFinished();
            _isOnCoolDown = false;
        }

        private void SetUpShootingEvent()
        {
            OnShootStarted += _playerComponents.Crosshair.PlayShootingAnimation;
            OnShootStarted += _playerComponents.OverHeat.ReduceAmmoBarByOne;
        }

        public override List<Transform> GetShootingPoints()
        {
            return ShootingPoints;
        }
        #endregion

        #region Constraints
        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canShoot = (constraints & AbilityConstraint.Shooting) == 0;
            IsConstrained = !canShoot;
        }
        #endregion


        #region IController
        public void Activate()
        {
            IsActive = true;
            _canShoot = true;
        }

        public void Deactivate()
        {
            IsActive = false;
            _canShoot = false;
        }

        public void Restart()
        {
            ShootingMethod = DefaultShot;
            _isOnCoolDown = false;
            IsConstrained = false;

            SetUpInput(_playerComponents.PlayerIndex);
            SetUpShootingEvent();
        }

        public void Dispose()
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.DisposeWeapon();
            }

            DisposeInput(_playerComponents.PlayerIndex);
            
            ShootingMethod = null;
            OnShootStarted = null;
        }
        #endregion
    }
}

