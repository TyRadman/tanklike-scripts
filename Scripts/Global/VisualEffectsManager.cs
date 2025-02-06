using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.Misc;
using TankLike.Utils;
using UnityEngine;

namespace TankLike
{
    public class VisualEffectsManager : MonoBehaviour, IManager
    {
        [field: SerializeField] public ExplosionEffects Explosions { get; private set; }
        [field: SerializeField] public BuffEffects Buffs { get; private set; }
        [field: SerializeField] public MuzzleFlashEffects MuzzleFlashes { get; private set; }
        [field: SerializeField] public TelegraphEffects Telegraphs { get; private set; }
        [field: SerializeField] public BulletEffects Bullets { get; private set; }
        [field: SerializeField] public LaserEffects Lasers { get; private set; }
        [field: SerializeField] public IndicatorEffects Indicators { get; private set; }
        [field: SerializeField] public MiscEffects Misc { get; private set; }
        [field: SerializeField] public ImpactEffects Impacts { get; private set; }

        public bool IsActive { get; private set; }

        private AmmunitionDatabase _bulletsDatabase;

        public void SetReferences(AmmunitionDatabase bulletsDatabase)
        {
            _bulletsDatabase = bulletsDatabase;

            Bullets.SetReferences(_bulletsDatabase.GetAllBullets());
            Lasers.SetReferences(_bulletsDatabase.GetAllLaser());
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            Explosions.SetUp();
            Buffs.SetUp();
            MuzzleFlashes.SetUp();
            Bullets.SetUp();
            Telegraphs.SetUp();
            Lasers.SetUp();
            Indicators.SetUp();
            Misc.SetUp();
            Impacts.SetUp();
        }

        public void Dispose()
        {
            IsActive = false;

            Explosions.Dispose();
            Buffs.Dispose();
            MuzzleFlashes.Dispose();
            Bullets.Dispose();
            Telegraphs.Dispose();
            Lasers.Dispose();
            Indicators.Dispose();
            Misc.Dispose();
            Impacts.Dispose();
        }
        #endregion
    }

    #region Effects Classes
    public abstract class VisualEffects
    {
        public abstract void SetUp();
        public abstract void Dispose();

        // TODO: clean up
        #region CreatePool Overloads
        protected Pool<ParticleSystemHandler> CreatePool(ParticleSystemHandler prefab, int preFill)
        {
            var pool = new Pool<ParticleSystemHandler>(
                () =>
                {
                    var obj = MonoBehaviour.Instantiate(prefab);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj;
                },
                (ParticleSystemHandler obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (ParticleSystemHandler obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (ParticleSystemHandler obj) => obj.GetComponent<IPoolable>().Clear(),
                preFill
           );
            return pool;
        }

        protected Pool<Ammunition> CreatePool(Ammunition prefab, int preFill)
        {
            var pool = new Pool<Ammunition>(
                () =>
                {
                    var obj = MonoBehaviour.Instantiate(prefab);
                    GameManager.Instance.SetParentToRoomSpawnables(obj.gameObject);
                    return obj;
                },
                (Ammunition obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (Ammunition obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (Ammunition obj) => obj.GetComponent<IPoolable>().Clear(),
                preFill
           );
            return pool;
        }

        protected Pool<Bullet> CreatePool(Bullet prefab, int preFill)
        {
            var pool = new Pool<Bullet>(
                () =>
                {
                    var obj = MonoBehaviour.Instantiate(prefab);
                    GameManager.Instance.SetParentToRoomSpawnables(obj.gameObject);
                    return obj;
                },
                (Bullet obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (Bullet obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (Bullet obj) => obj.GetComponent<IPoolable>().Clear(),
                preFill
           );
            return pool;
        }

        protected Pool<Laser> CreatePool(Laser prefab, int preFill)
        {
            var pool = new Pool<Laser>(
                () =>
                {
                    var obj = MonoBehaviour.Instantiate(prefab);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj;
                },
                (Laser obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (Laser obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (Laser obj) => obj.GetComponent<IPoolable>().Clear(),
                preFill
           );
            return pool;
        }

        protected Pool<Indicator> CreatePool(Indicator prefab, int preFill)
        {
            var pool = new Pool<Indicator>(
                () =>
                {
                    var obj = MonoBehaviour.Instantiate(prefab);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj;
                },
                (Indicator obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (Indicator obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (Indicator obj) => obj.GetComponent<IPoolable>().Clear(),
                preFill
           );
            return pool;
        }
        #endregion
    }

    [System.Serializable]
    public class ExplosionEffects : VisualEffects
    {
        [SerializeField] private ParticleSystemHandler _deathExplosion;
        [SerializeField] private ParticleSystemHandler _explosionDecal;
        [SerializeField] private ParticleSystemHandler _electricExplosion;
        [SerializeField] private ParticleSystemHandler _aoeExplosion;
        [SerializeField] private PoolableParticlesReference[] _explosionEffects;

        private Pool<ParticleSystemHandler> _deathExplosionPool;
        private Pool<ParticleSystemHandler> _explosionDecalPool;
        private Pool<ParticleSystemHandler> _electricExplosionPool;
        private Pool<ParticleSystemHandler> _aoeExplosionPool;
        private Dictionary<PoolableParticlesReference, Pool<ParticleSystemHandler>> _pool = new Dictionary<PoolableParticlesReference, Pool<ParticleSystemHandler>>();

        public ParticleSystemHandler DeathExplosion { get { return _deathExplosionPool.RequestObject(); } }
        public ParticleSystemHandler ExplosionDecal { get { return _explosionDecalPool.RequestObject(); } }
        public ParticleSystemHandler ElectricExplosion { get { return _electricExplosionPool.RequestObject(); } }
        public ParticleSystemHandler AOEExplosion { get { return _aoeExplosionPool.RequestObject(); } }

        public override void SetUp()
        {
            _deathExplosionPool = CreatePool(_deathExplosion, 1);
            _explosionDecalPool = CreatePool(_explosionDecal, 1);
            _electricExplosionPool = CreatePool(_electricExplosion, 1);
            _aoeExplosionPool = CreatePool(_aoeExplosion, 1);

            foreach (var effect in _explosionEffects)
            {
                if (_pool.ContainsKey(effect))
                {
                    return;
                }

                _pool.Add(effect, CreatePool(effect.Poolable, 0));
            }
        }

        public override void Dispose()
        {
            _deathExplosionPool.Clear();
            _explosionDecalPool.Clear();

            // Remove the references so it can be garbage collected
            _deathExplosionPool = null;
            _explosionDecalPool = null;

            foreach (KeyValuePair<PoolableParticlesReference, Pool<ParticleSystemHandler>> effect in _pool)
            {
                effect.Value.Clear();
            }

            _pool.Clear();
        }

        public ParticleSystemHandler GetExplosion(PoolableParticlesReference reference)
        {
            if (!_pool.ContainsKey(reference))
            {
                _pool.Add(reference, CreatePool(reference.Poolable, 0));
            }

            return _pool[reference].RequestObject();

        }
    }

    [System.Serializable]
    public class BuffEffects : VisualEffects
    {
        [SerializeField] private ParticleSystemHandler _levelUpEffect;
        [SerializeField] private ParticleSystemHandler _superAbilityEffect;
        [SerializeField] private ParticleSystemHandler _healOnceEffect;

        private Pool<ParticleSystemHandler> _levelUpEffectPool;
        private Pool<ParticleSystemHandler> _superAbilityEffectPool;
        private Pool<ParticleSystemHandler> _healOnceEffectPool;

        public ParticleSystemHandler LevelUp { get { return _levelUpEffectPool.RequestObject(); } }
        public ParticleSystemHandler SuperAbility { get { return _superAbilityEffectPool.RequestObject(); } }
        public ParticleSystemHandler HealOnce { get { return _healOnceEffectPool.RequestObject(); } }

        public override void SetUp()
        {
            _levelUpEffectPool = CreatePool(_levelUpEffect, 1);
            _superAbilityEffectPool = CreatePool(_superAbilityEffect, 1);
            _healOnceEffectPool = CreatePool(_healOnceEffect, 1);
        }

        public override void Dispose()
        {
            _levelUpEffectPool.Clear();
            _superAbilityEffectPool.Clear();
            _healOnceEffectPool.Clear();

            // Remove the references so it can be garbage collected
            _levelUpEffectPool = null;
            _superAbilityEffectPool = null;
            _healOnceEffectPool = null;
        }
    }

    [System.Serializable]
    public class MuzzleFlashEffects : VisualEffects
    {
        [SerializeField] private ParticleSystemHandler _iceMuzzleFlash;

        private Pool<ParticleSystemHandler> _iceMuzzleFlashPool;

        public ParticleSystemHandler IceMuzzleFlash { get { return _iceMuzzleFlashPool.RequestObject(); } }

        public override void SetUp()
        {
            _iceMuzzleFlashPool = CreatePool(_iceMuzzleFlash, 1);
        }

        public override void Dispose()
        {
            _iceMuzzleFlashPool.Clear();

            // Remove the references so it can be garbage collected
            _iceMuzzleFlashPool = null;
        }
    }

    [System.Serializable]
    public class BulletEffects : VisualEffects
    {
        private Dictionary<string, Pool<Bullet>> _bulletsPool = new Dictionary<string, Pool<Bullet>>();
        private Dictionary<string, Pool<ParticleSystemHandler>> _muzzleFlashEffectsPool = new Dictionary<string, Pool<ParticleSystemHandler>>();
        private Dictionary<string, Pool<ParticleSystemHandler>> _impactEffectsPool = new Dictionary<string, Pool<ParticleSystemHandler>>();

        private List<AmmunationData> _bullets;

        public void SetReferences(List<AmmunationData> bullets)
        {
            _bullets = bullets;
        }

        public override void SetUp()
        {
            foreach (AmmunationData bullet in _bullets)
            {
                // It fixes a very weird error (key already exists). We do the check already in the AmmuitionDatabase
                if (_bulletsPool.ContainsKey(bullet.GUID))
                {
                    return;
                }

                _bulletsPool.Add(bullet.GUID, CreatePool((Bullet)bullet.Ammunition, 0));

                if (bullet.MuzzleFlash != null)
                {
                    _muzzleFlashEffectsPool.Add(bullet.GUID, CreatePool(bullet.MuzzleFlash, 0));
                }

                if(bullet.Impact != null)
                {
                    _impactEffectsPool.Add(bullet.GUID, CreatePool(bullet.Impact, 0));
                }
            }
        }

        public override void Dispose()
        {
            foreach (KeyValuePair<string, Pool<Bullet>> bullet in _bulletsPool)
            {
                bullet.Value.Clear();
            }

            _bulletsPool.Clear();

            foreach (KeyValuePair<string, Pool<ParticleSystemHandler>> muzzleFlashEffect in _muzzleFlashEffectsPool)
            {
                muzzleFlashEffect.Value.Clear();
            }

            _muzzleFlashEffectsPool.Clear();

            foreach (KeyValuePair<string, Pool<ParticleSystemHandler>> impactEffect in _impactEffectsPool)
            {
                impactEffect.Value.Clear();
            }

            _impactEffectsPool.Clear();
        }

        public Bullet GetBullet(string guid)
        {
            return _bulletsPool[guid].RequestObject();
        }

        public ParticleSystemHandler GetImpact(string guid)
        {
            if (!_impactEffectsPool.ContainsKey(guid))
            {
                return null;
            }

            return _impactEffectsPool[guid].RequestObject();
        }

        public ParticleSystemHandler GetMuzzleFlash(string guid)
        {
            if(!_muzzleFlashEffectsPool.ContainsKey(guid))
            {
                return null;
            }

            return _muzzleFlashEffectsPool[guid].RequestObject();
        }
    }

    [System.Serializable]
    public class TelegraphEffects : VisualEffects
    {
        [SerializeField] private ParticleSystemHandler _enemyTelegraphEffect;

        private Pool<ParticleSystemHandler> _enemyTelegraphEffectPool;

        public ParticleSystemHandler EnemyTelegraph { get { return _enemyTelegraphEffectPool.RequestObject(); } }

        public override void SetUp()
        {
            _enemyTelegraphEffectPool = CreatePool(_enemyTelegraphEffect, 0);
        }

        public override void Dispose()
        {
            _enemyTelegraphEffectPool.Clear();

            // Remove the references so it can be garbage collected
            _enemyTelegraphEffectPool = null;
        }
    }

    [System.Serializable]
    public class MiscEffects : VisualEffects
    {
        [SerializeField] private ParticleSystemHandler _enemySpawningEffect;
        [SerializeField] private ParticleSystemHandler _playerSpawningEffect;
        [SerializeField] private ParticleSystemHandler _bossKeyEffect;
        [SerializeField] private ParticleSystemHandler _bossKeyImpact;
        [SerializeField] private ParticleSystemHandler _onCollectedPoof;
        [SerializeField] private ParticleSystemHandler _onEnergyCollectedPoof;
        [SerializeField] private ParticleSystemHandler _onShieldImpactedParticles;

        private Pool<ParticleSystemHandler> _enemySpawningEffectPool;
        private Pool<ParticleSystemHandler> _playerSpawningEffectPool;
        private Pool<ParticleSystemHandler> _bossKeyEffectPool;
        private Pool<ParticleSystemHandler> _bossKeyImpactPool;
        private Pool<ParticleSystemHandler> _onCollectedPoofPool;
        private Pool<ParticleSystemHandler> _onEnergyCollectedPoofPool;
        private Pool<ParticleSystemHandler> _onShieldImpactedParticlesPool;

        public ParticleSystemHandler EnemySpawning { 
            get
            { 
                return _enemySpawningEffectPool.RequestObject(); 
            } 
        }
        public ParticleSystemHandler PlayerSpawning { get { return _playerSpawningEffectPool.RequestObject(); } }
        public ParticleSystemHandler BossKey { get { return _bossKeyEffectPool.RequestObject(); } }
        public ParticleSystemHandler BossKeyImpact { get { return _bossKeyImpactPool.RequestObject(); } }
        public ParticleSystemHandler OnCollectedPoof { get { return _onCollectedPoofPool.RequestObject(); } }
        public ParticleSystemHandler OnEnergyCollectedPoof { get { return _onEnergyCollectedPoofPool.RequestObject(); } }
        public ParticleSystemHandler OnShieldImpactedParticles { get { return _onShieldImpactedParticlesPool.RequestObject(); } }

        public override void SetUp()
        {
            _enemySpawningEffectPool = CreatePool(_enemySpawningEffect, 0);
            _playerSpawningEffectPool = CreatePool(_playerSpawningEffect, 0);
            _bossKeyEffectPool = CreatePool(_bossKeyEffect, 0);
            _bossKeyImpactPool = CreatePool(_bossKeyImpact, 0);
            _onCollectedPoofPool = CreatePool(_onCollectedPoof, 0);
            _onEnergyCollectedPoofPool = CreatePool(_onEnergyCollectedPoof, 0);
            _onShieldImpactedParticlesPool = CreatePool(_onShieldImpactedParticles, 0);
        }

        public override void Dispose()
        {
            _enemySpawningEffectPool.Clear();
            _playerSpawningEffectPool.Clear();
            _bossKeyEffectPool.Clear();
            _bossKeyImpactPool.Clear();
            _onCollectedPoofPool.Clear();
            _onEnergyCollectedPoofPool.Clear();
            _onShieldImpactedParticlesPool.Clear();

            // Remove the references so it can be garbage collected
            _enemySpawningEffectPool = null;
            _playerSpawningEffectPool = null;
            _bossKeyEffectPool = null;
            _bossKeyImpactPool = null;
            _onCollectedPoofPool = null;
            _onEnergyCollectedPoofPool = null;
            _onShieldImpactedParticlesPool = null;
        }

        public float PlayPlayerSpawnVFX(Vector3 position, float size = 1f)
        {
            PlaySpawningEffect(PlayerSpawning, position, size);
            return PlayerSpawning.Particles.main.startLifetime.constant / 2;
        }

        public float GetPlayerSpawnVFXDuration()
        {
            return PlayerSpawning.Particles.main.startLifetime.constant / 2;
        }

        public float PlayEnemySpawnVFX(Vector3 position)
        {
            PlaySpawningEffect(EnemySpawning, position);
            return EnemySpawning.Particles.main.startLifetime.constant / 2;
        }

        public void PlaySpawningEffect(ParticleSystemHandler particle, Vector3 position, float size = 1f)
        {
            var vfx = particle;
            vfx.transform.SetPositionAndRotation(position, Quaternion.identity);
            vfx.gameObject.SetActive(true);

            if(size != 1f)
            {
                vfx.transform.localScale *= size;
            }

            vfx.Play();
        }
    }

    [System.Serializable]
    public class LaserEffects : VisualEffects
    {
        [SerializeField] private Laser _laser_01;

        //private Pool<Laser> _laserPool;
        private Dictionary<string, Pool<Laser>> _lasersPool = new Dictionary<string, Pool<Laser>>();
        private Dictionary<string, Pool<ParticleSystemHandler>> _muzzleFlashEffectsPool = new Dictionary<string, Pool<ParticleSystemHandler>>();
        private Dictionary<string, Pool<ParticleSystemHandler>> _impactEffectsPool = new Dictionary<string, Pool<ParticleSystemHandler>>();

        private List<AmmunationData> _lasers;

        public void SetReferences(List<AmmunationData> lasers)
        {
            _lasers = lasers;
        }

        public override void SetUp()
        {
            foreach (var laser in _lasers)
            {
                if (_lasersPool.ContainsKey(laser.GUID))
                {
                    return;
                }

                _lasersPool.Add(laser.GUID, CreatePool((Laser)laser.Ammunition, 0));
                _muzzleFlashEffectsPool.Add(laser.GUID, CreatePool(laser.MuzzleFlash, 0));
                _impactEffectsPool.Add(laser.GUID, CreatePool(laser.Impact, 0));
            }
        }

        public override void Dispose()
        {
            foreach (KeyValuePair<string, Pool<Laser>> laser in _lasersPool)
            {
                laser.Value.Clear();
            }

            _lasersPool.Clear();

            foreach (KeyValuePair<string, Pool<ParticleSystemHandler>> muzzleFlashEffect in _muzzleFlashEffectsPool)
            {
                muzzleFlashEffect.Value.Clear();
            }

            _muzzleFlashEffectsPool.Clear();

            foreach (KeyValuePair<string, Pool<ParticleSystemHandler>> impactEffect in _impactEffectsPool)
            {
                impactEffect.Value.Clear();
            }

            _impactEffectsPool.Clear();
        }

        public Laser GetLaser(string guid)
        {
            return _lasersPool[guid].RequestObject();
        }

        public ParticleSystemHandler GetImpact(string guid)
        {
            return _impactEffectsPool[guid].RequestObject();
        }

        public ParticleSystemHandler GetMuzzleFlash(string guid)
        {
            return _muzzleFlashEffectsPool[guid].RequestObject();
        }
    }

    [System.Serializable]
    public class IndicatorEffects : VisualEffects
    {
        public enum IndicatorType
        {
            None = 0,
            Circle = 1,
            Square = 2,
            Line = 3,
            RocketRed = 4,
            RocketBlue = 5,
        }

        private Dictionary<IndicatorType, Pool<Indicator>> _indicatorEffectsPool = new Dictionary<IndicatorType, Pool<Indicator>>();

        [SerializeField] private Indicator _circleIndicatorEffect;
        [SerializeField] private Indicator _squareIndicatorEffect;
        [SerializeField] private Indicator _lineIndicatorEffect;
        [SerializeField] private Indicator _rocketRedIndicatorEffect;
        [SerializeField] private Indicator _rocketBlueIndicatorEffect;

        public override void SetUp()
        {
            _indicatorEffectsPool.Add(IndicatorType.Circle, CreatePool(_circleIndicatorEffect, 0));       
            _indicatorEffectsPool.Add(IndicatorType.Square, CreatePool(_squareIndicatorEffect, 0));       
            _indicatorEffectsPool.Add(IndicatorType.Line, CreatePool(_lineIndicatorEffect, 0));       
            _indicatorEffectsPool.Add(IndicatorType.RocketRed, CreatePool(_rocketRedIndicatorEffect, 0));       
            _indicatorEffectsPool.Add(IndicatorType.RocketBlue, CreatePool(_rocketBlueIndicatorEffect, 0));       
        }

        public override void Dispose()
        {
            foreach (KeyValuePair<IndicatorType, Pool<Indicator>> indicatorEffect in _indicatorEffectsPool)
            {
                indicatorEffect.Value.Clear();
            }

            _indicatorEffectsPool.Clear();
        }

        public Indicator GetIndicatorByType(IndicatorType type)
        {
            return _indicatorEffectsPool[type].RequestObject();
        }
    }

    [System.Serializable]
    public class ImpactEffects : VisualEffects
    {
        [SerializeField] private PoolableParticlesReference[] _impactEffects;

        private Dictionary<PoolableParticlesReference, Pool<ParticleSystemHandler>> _pool = new Dictionary<PoolableParticlesReference, Pool<ParticleSystemHandler>>();

        private List<AmmunationData> _lasers;

        public override void SetUp()
        {
            foreach (var effect in _impactEffects)
            {
                if (_pool.ContainsKey(effect))
                {
                    return;
                }

                _pool.Add(effect, CreatePool(effect.Poolable, 0));
            }
        }

        public override void Dispose()
        {
            foreach (KeyValuePair<PoolableParticlesReference, Pool<ParticleSystemHandler>> effect in _pool)
            {
                effect.Value.Clear();
            }

            _pool.Clear();
        }

        public ParticleSystemHandler GetImpact(PoolableParticlesReference reference)
        {
            if (!_pool.ContainsKey(reference))
            {
                _pool.Add(reference, CreatePool(reference.Poolable, 0));
            }

            return _pool[reference].RequestObject();

        }
    }
    #endregion
}
