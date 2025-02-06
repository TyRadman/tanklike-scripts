using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    using UnitControllers;
    using Misc;
    using Utils;
    using Combat.SkillTree.Upgrades;

    [CreateAssetMenu(fileName = "W_Shot_Default", menuName = DIRECTORY + "Normal Shot")]
    public class NormalShot : Weapon
    {
        #region Consecutive shots struct
        [System.Serializable]
        public struct ShotsConsecution
        {
            public bool HasConsecutiveShots;
            public int NumberOfShots;
            public float TimeBetweenShots;
        }
        #endregion

        public const string ROOT_PATH = Directories.ABILITIES + "Normal Shot/";
        [field: SerializeField] public float BulletSpeed { get; private set; }
        [field: SerializeField] public float MaxDistance = 100f;
        [field: SerializeField] public int ShotsNumber = 1;
        [field: SerializeField] public float AngleBetweenShots = 0f;
        [field: SerializeField] public ShotsConsecution ConsecutiveShots { get; private set; } = new ShotsConsecution();

        [field: SerializeField, Header("Gravity")] public bool UseGravity;
        [field: SerializeField] public float GravityMultiplier = 0f;

        [Header("Animations")]
        [SerializeField] protected float _shootAnimationDelay = 0.05f;

        private WaitForSeconds _waitBetweenShots;
        private BulletData _bulletData;
        private Coroutine _shootingCoroutine;
        private CoroutinesManager _coroutinesManager;
        private List<Transform> _shootingPoints = new List<Transform>();
        private WaitForSeconds _shootAnimationDelayWait;
        public override void SetUp(UnitComponents components)
        {
            base.SetUp(components);

            _waitBetweenShots = new WaitForSeconds(ConsecutiveShots.TimeBetweenShots);
            _shootAnimationDelayWait = new WaitForSeconds(_shootAnimationDelay);

            if (_bulletData == null)
            {
                _bulletData = new BulletData()
                {
                    Speed = BulletSpeed,
                    Damage = Damage,
                    MaxDistance = MaxDistance,
                    CanBeDeflected = CanBeDeflected,
                    UseGravity = UseGravity,
                    GravityMultiplier = GravityMultiplier,
                    OnShotAudio = ShotAudio
                };
            }

            _coroutinesManager = GameManager.Instance.CoroutineManager;

            if(_components.GetShooter() != null)
            {
                _shootingPoints = _components.GetShooter().GetShootingPoints();
            }
            else
            {
                Debug.LogError("No shooting points found for the weapon");
            }
        }

        public override void OnShot(Transform shootingPoint = null, float angle = 0, bool freeRotation = false)
        {
            base.OnShot();

            _shootingCoroutine = _coroutinesManager.StartExternalCoroutine(OnShotProcess(shootingPoint, angle, freeRotation));
        }

        private IEnumerator OnShotProcess(Transform shootingPoint = null, float angle = 0, bool freeRotation = false)
        {
            float angleIncement = AngleBetweenShots / (ShotsNumber - 1);
            int wavesCount = ConsecutiveShots.HasConsecutiveShots ? ConsecutiveShots.NumberOfShots : 1;

            for (int j = 0; j < wavesCount; j++)
            {
                float currentAngle = angle;

                if (ShotsNumber > 1)
                {
                    currentAngle = _tankTransform.eulerAngles.y - AngleBetweenShots / 2;
                }

                _coroutinesManager.StartExternalCoroutine(ShootWave(shootingPoint, freeRotation, currentAngle, angleIncement));

                //yield return new WaitForSeconds(0.5f);

                //for (int i = 0; i < ShotsNumber; i++)
                //{
                //    // TODO: this should be an event that the shooters subscribe to
                //    //_components.Shooter.PlayShootingEffects();
                //    OnWeaponShot?.Invoke();

                //    Bullet bullet = SpawnProjectile();
                //    SpawnBullet(bullet, shootingPoint, currentAngle, freeRotation);

                //    ParticleSystemHandler muzzle = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
                //    ShowShootingEffects(muzzle, shootingPoint);

                //    currentAngle += angleIncement;
                //}

                yield return _waitBetweenShots;
            }
        }

        private IEnumerator ShootWave(Transform shootingPoint, bool freeRotation, float currentAngle, float angleIncement)
        {
            yield return _shootAnimationDelayWait;

            for (int i = 0; i < ShotsNumber; i++)
            {
                // TODO: this should be an event that the shooters subscribe to
                //_components.Shooter.PlayShootingEffects();
                OnWeaponShot?.Invoke();

                Bullet bullet = SpawnProjectile();
                SpawnBullet(bullet, shootingPoint, currentAngle, freeRotation);

                ParticleSystemHandler muzzle = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
                ShowShootingEffects(muzzle, shootingPoint);

                currentAngle += angleIncement;
            }
        }

        public override void OnShot(Vector3 spawnPoint, Vector3 rotation, float angle = 0)
        {
            base.OnShot(spawnPoint, rotation, angle);

            _shootingCoroutine = _coroutinesManager.StartExternalCoroutine(OnShotProcess(spawnPoint, rotation, angle));
        }

        private IEnumerator OnShotProcess(Vector3 spawnPoint, Vector3 rotation, float angle = 0)
        {
            float angleIncement = AngleBetweenShots / (ShotsNumber - 1);
            int wavesCount = ConsecutiveShots.HasConsecutiveShots ? ConsecutiveShots.NumberOfShots : 1;

            for (int j = 0; j < wavesCount; j++)
            {
                float currentAngle = angle;

                if (ShotsNumber > 1)
                {
                    currentAngle = _tankTransform.eulerAngles.y - AngleBetweenShots / 2;
                }

                for (int i = 0; i < ShotsNumber; i++)
                {
                    //_components.Shooter.PlayShootingEffects();
                    OnWeaponShot?.Invoke();

                    Bullet bullet = SpawnProjectile();
                    ParticleSystemHandler muzzle = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
                    SpawnBullet(bullet, spawnPoint, rotation, currentAngle);
                    ShowShootingEffects(muzzle, spawnPoint, rotation);

                    currentAngle += angleIncement;
                }

                yield return _waitBetweenShots;
            }
        }

        public override void OnShot(System.Action<Bullet, Transform, float> spawnBulletAction, Transform shootingPoint, float angle)
        {
            base.OnShot(spawnBulletAction, shootingPoint, angle);

            _shootingCoroutine = _coroutinesManager.StartExternalCoroutine(OnShotProcess(spawnBulletAction, shootingPoint, angle));
        }

        private IEnumerator OnShotProcess(System.Action<Bullet, Transform, float> spawnBulletAction, Transform shootingPoint, float angle)
        {
            float angleIncement = AngleBetweenShots / (ShotsNumber - 1);
            int wavesCount = ConsecutiveShots.HasConsecutiveShots ? ConsecutiveShots.NumberOfShots : 1;

            for (int j = 0; j < wavesCount; j++)
            {
                float currentAngle = angle;

                if (ShotsNumber > 1)
                {
                    currentAngle = shootingPoint.transform.eulerAngles.y - AngleBetweenShots / 2;
                }

                for (int i = 0; i < ShotsNumber; i++)
                {
                    //_components.Shooter.PlayShootingEffects();
                    OnWeaponShot?.Invoke();

                    Bullet bullet = SpawnProjectile();
                    GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(BulletData.GUID);
                    spawnBulletAction.Invoke(bullet, shootingPoint, currentAngle);

                    currentAngle += angleIncement;
                }

                yield return _waitBetweenShots;
            }
        }

        private Bullet SpawnProjectile()
        {
            Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(BulletData.GUID);
            bullet.SetUpBulletdata(BulletData);
            bullet.SetTargetLayerMask(_targetLayerMask);
            bullet.SetValues(_bulletData);

            return bullet;
        }

        #region SpawnBullet Overloads
        public void SpawnBullet(Bullet bullet, Transform shootingPoint = null, float angle = 0, bool freeRotation = false)
        {
            // create the bullet
            bullet.gameObject.SetActive(true);

            // handle position and rotation
            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            if (shootingPoint != null)
            {
                if (freeRotation)
                {
                    rotation = shootingPoint.rotation;
                }
                else
                {
                    Vector3 eulerRotation = shootingPoint.eulerAngles;
                    eulerRotation.x = 0;
                    eulerRotation.z = 0;
                    rotation = Quaternion.Euler(eulerRotation);
                }

                position = shootingPoint.position;
            }
            else
            {
                if (_shootingPoints.Count > 0)
                {
                    Vector3 eulerRotation = _shootingPoints[0].eulerAngles;
                    eulerRotation.x = 0;
                    eulerRotation.z = 0;
                    rotation = Quaternion.Euler(eulerRotation);
                    position = _shootingPoints[0].position;
                    position.y = Constants.ShootingPointHeight;
                }
            }

            if (angle != 0)
            {
                rotation *= Quaternion.Euler(0f, angle, 0f);
            }

            bullet.transform.SetPositionAndRotation(position, rotation);
            bullet.StartBullet(_components);
            bullet.SetTargetLayerMask(Helper.GetOpposingTag(_components.Alignment));
        }

        public void SpawnBullet(Bullet bullet, Vector3 spawnPoint, Vector3 direction, float angle = 0)
        {
            bullet.gameObject.SetActive(true);

            direction.x = 0;
            direction.y += angle;
            Quaternion rotation = Quaternion.Euler(direction);

            spawnPoint.y = Constants.ShootingPointHeight;
            bullet.transform.SetPositionAndRotation(spawnPoint, rotation);
            bullet.StartBullet(_components);
            bullet.SetTargetLayerMask(Helper.GetOpposingTag(_components.Alignment));
        }
        #endregion

        #region SpawnShootingEffects Overloads
        public void ShowShootingEffects(ParticleSystemHandler muzzleEffect, Transform shootingPoint = null)
        {
            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;

            if (shootingPoint != null)
            {
                position = shootingPoint.position;
                rotation = shootingPoint.rotation;
            }
            else
            {
                if (_shootingPoints.Count > 0)
                {
                    rotation = _shootingPoints[0].rotation;
                    position = _shootingPoints[0].position;
                }
            }

            if (muzzleEffect != null)
            {
                muzzleEffect.transform.SetPositionAndRotation(position, rotation);
                muzzleEffect.gameObject.SetActive(true);
                muzzleEffect.Play();
            }
        }

        public void ShowShootingEffects(ParticleSystemHandler muzzleEffect, Vector3 position, Vector3 rotation)
        {
            Quaternion effectRotation = Quaternion.Euler(rotation);
            muzzleEffect.transform.SetPositionAndRotation(position, effectRotation);
            muzzleEffect.gameObject.SetActive(true);
            muzzleEffect.Play();
        }
        #endregion

        #region Utilities
        public override void SetSpeed(float speed)
        {
            BulletSpeed = speed;
        }

        internal override float GetCoolDownTime()
        {
            return CoolDownTime + ConsecutiveShots.TimeBetweenShots * (ConsecutiveShots.NumberOfShots - 1);
        }
        #endregion

        public override void Upgrade(BaseWeaponUpgrade weaponUpgrade)
        {
            CoolDownTime += weaponUpgrade.CoolDownTime;
            AmmoCapacity += weaponUpgrade.Ammo;

            Damage += weaponUpgrade.Damage;

            BulletSpeed += weaponUpgrade.ProjectileSpeed;

            _bulletData.Speed = BulletSpeed;
            _bulletData.Damage = Damage;

        }

        public override void UpgradeDamage(int damage)
        {
            base.UpgradeDamage(damage);
            _bulletData.Damage = Damage;
        }

        public void SetImpactType(OnImpact impact)
        {
            _bulletData.Impact = impact;
        }

        public BulletData GetBulletData()
        {
            return _bulletData;
        }

        public override void DisposeWeapon()
        {
            base.DisposeWeapon();

            _coroutinesManager.StopExternalCoroutine(_shootingCoroutine);
        }
    }
}
