using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Combat;
    using Elements;
    using Utils;
    using Misc;
    using Sound;
    using Cam;
    using Combat.Abilities;
    using TankLike.Combat.SkillTree.Upgrades;
    using TankLike.Combat.SkillTree;

    public abstract class TankShooter : UnitShooter, IController, IConstraintedComponent, ISkill
    {
        public class WeaponData
        {
            public WeaponHolder OriginalHolder;
            public WeaponHolder Holder;
        }

        public bool IsActive { get; protected set; }

        public List<Transform> ShootingPoints { get; set; }

        public System.Action OnShootStarted { get; set; }
        public System.Action OnShootFinished { get; set; }
        public bool IsConstrained { get; set; }

        [Header("Settings")]
        [SerializeField] protected bool _canShoot;
        [SerializeField] protected float _shootAnimationDelay = 0f;
        [SerializeField] protected LayerMask _targetLayers;

        [Header("References")]
        [SerializeField] protected WeaponHolder _startWeaponHolder;
        [SerializeField] protected SkillProfile _startWeaponProfile;

        [SerializeField] protected Audio _onShootAudio;

        protected Animator _turretAnimator;
        protected Transform _turret;
        protected float _coolDownTime = 3f;
        protected bool _isOnCoolDown = false;
        protected Weapon _currentWeapon;
        protected WeaponHolder _currentWeaponHolder;
        protected List<IPoolable> _activePoolables = new List<IPoolable>();

        private Dictionary<WeaponHolder, WeaponHolder> _weapons = new Dictionary<WeaponHolder, WeaponHolder>();

        private System.Action<Transform, float> ShootingMethod;
        private TankComponents _components;
        private WaitForSeconds _shootWaitForSeconds;
        private SkillHolder _lastWeaponHolder;

        #region Shooting Method Overload
        protected bool CanShoot(bool hasCoolDown)
        {
            bool cooldownInPlace = _isOnCoolDown && hasCoolDown;
            return _canShoot && !cooldownInPlace && !IsConstrained;
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
            //_components.Animation.PlayShootAnimation();
            //PlayShootAudio();

            StartCoroutine(ShootingRoutine(shootingPoint, angle));

            if (hasCoolDown)
            {
                _isOnCoolDown = true;
                EnableShooting();
            }
        }

        private IEnumerator ShootingRoutine(Transform shootingPoint = null, float angle = 0f)
        {
            yield return _shootWaitForSeconds;
            ShootingMethod(shootingPoint, angle);
        }

        /// <summary>
        /// Shoots using the shots of a custom weapon
        /// </summary>
        /// <param name="weapon">The custom weapon</param>
        /// <param name="angle">The angle of the shot in degrees</param>
        /// <param name="hasCoolDown">True if you want the shot to trigger the down</param>
        public virtual void Shoot(Weapon weapon, float angle, bool hasCoolDown = true, bool canBeConstrained = false)
        {
            bool cooldownInPlace = _isOnCoolDown && hasCoolDown;
            bool isConstrained = IsConstrained && canBeConstrained;

            if (!_canShoot && cooldownInPlace || isConstrained)
            {
                return;
            }

            weapon.OnShot(null, angle);
            _components.Animation.PlayShootAnimation(2f); //dirty, get the speed from the weapon

            if (hasCoolDown)
            {
                _isOnCoolDown = true;
                EnableShooting();
            }
        }
        #endregion

        #region Enable Shooting
        /// <summary>
        /// this method is virtual so that the player can override it and enable shooting through a coroutine which will allow them to update the BarImage of the weapon icon
        /// </summary>
        protected virtual void EnableShooting()
        {
            Invoke(nameof(EnableShootingInvoke), _coolDownTime);
        }

        private void EnableShootingInvoke()
        {
            _isOnCoolDown = false;
        }
        #endregion

        public virtual void SetUp(IController controller)
        {
            if (controller is not TankComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _components = components;
            TankBodyParts parts = _components.TankBodyParts;

            TankTurret turret = (TankTurret)parts.GetBodyPartOfType(BodyPartType.Turret);

            _turret = turret.transform;
            ShootingPoints = new List<Transform>();
            ShootingPoints.AddRange(turret.ShootingPoints);
            _turretAnimator = turret.Animator;

            _shootWaitForSeconds = new WaitForSeconds(_shootAnimationDelay);
        }

        public void EnableShooting(bool canShoot)
        {
            _canShoot = canShoot;
        }

        public void PlayShootingEffects()
        {
            if(_components.Animation == null)
            {
                return;
            }

            _components.Animation.PlayShootAnimation(2f); //dirty, get the speed from the weapon
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.SHOOT);
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


        #region Extra
        protected virtual void OnShootExitHandler()
        {
            OnShootFinished?.Invoke();
        }

        public bool IsCanShoot()
        {
            return _canShoot && !IsConstrained;
        }     

        public Weapon GetWeapon()
        {
            return _currentWeapon;
        }

        public WeaponHolder GetWeaponHolder()
        {
            return _currentWeaponHolder;
        }

        public void SetWeaponDamage(int damage)
        {
            if (_currentWeaponHolder != null)
            {
                _currentWeaponHolder.Weapon.SetDamage(damage);
            }
            else
            {
                Debug.LogError("WHY NULL");
            }
        }

        #region Addition and Equipment
        public virtual void AddSkill(SkillHolder holder)
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

            weapon.SetUp(_components);

            if (_currentWeaponHolder == null)
            {
                EquipSkill(weaponHolder);
            }
        }

        /// <summary>
        /// Equips the entity with a weapon.
        /// </summary>
        /// <param name="weapon"></param>
        public virtual void EquipSkill(SkillHolder holder)
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
        }

        public void ReEquipSkill()
        {
            EquipSkill(_lastWeaponHolder);
        }
        #endregion

        public void SetWeaponSpeed(float bulletSpeed)
        {
            _currentWeaponHolder.Weapon.SetSpeed(bulletSpeed);
        }

        public void SetLaserDuration(float duration)
        {
            if(_currentWeapon is not LaserWeapon)
            {
                Debug.LogError($"{gameObject.name} doesn't have a laser weapon");
            }

            ((LaserWeapon)_currentWeapon).SetDuration(duration);
        }
        #endregion

        public void PlayShootAudio()
        {
            GameManager.Instance.AudioManager.Play(_onShootAudio);
        }

        public virtual void UpgradeSkill(BaseWeaponUpgrade weaponUpgrade)
        {

        }

        public override List<Transform> GetShootingPoints()
        {
            return ShootingPoints;
        }

        #region Constraints
        public void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canShoot = (constraints & AbilityConstraint.Shooting) == 0;
            IsConstrained = !canShoot;
        }
        #endregion

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
            _canShoot = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
            _canShoot = false;
        }

        public virtual void Restart()
        {
            ShootingMethod = DefaultShot;
            _isOnCoolDown = false;
            IsConstrained = false;
        }

        public virtual void Dispose()
        {
            ShootingMethod = null;

            if (_currentWeapon != null)
            {
                _currentWeapon.DisposeWeapon();
            }
        }
        #endregion
    }
}

public enum TanksTag
{
    Player, Enemy
}
