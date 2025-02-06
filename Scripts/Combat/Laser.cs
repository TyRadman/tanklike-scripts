using System;
using System.Collections;
using UnityEngine;

namespace TankLike.Combat
{
    using TankLike.Sound;
    using UnitControllers;
    using Utils;

    public class Laser : Ammunition
    {
        public Vector3 ForwardDirection;
        public Vector3 ImpactPoint;
        public GameObject ImpactedObject { get; private set; }

        [Header("Visuals")]
        [SerializeField] private LineRenderer _beam;
        [SerializeField] protected ParticleSystem _hitParticles;
        [SerializeField] protected ParticleSystem _muzzleFlashParticles;
        [SerializeField] private float _textureTilingSize = 5;

        [Header("Audio")]
        [SerializeField] private Audio _onShootingAudio;

        private AudioSource _audioSource;

        private Material _material;
        private Action<IPoolable> OnRemoveFromActivePoolables;
        private Coroutine _laserCoroutine;

        private float _duration;
        private float _maxLength;
        private int _damageOverTime;
        public const float DAMAGE_INTERVAL = 0.1f;
        private float _thickness = 0.3f;
        private float _damageTimer;
        private bool _isDealingDamage = false;


        /// <summary>
        /// The texture's tiling property on the x axis. Determines how stretched the texture will be across the laser
        /// </summary>
        private const float LASER_FADE_OUT_TIME = 1f;
        private const string LENGTH_ID = "_TilingLength";
        private const string ALPHA_CLIP_ID = "_CutoffHeight";
        private const string NOISE_CLIP_ID = "_NoiseMaxIn";

        private void Awake()
        {
            _material = _beam.material;
            _hitParticles.Stop();
            _muzzleFlashParticles.Stop();

            _damageTimer = DAMAGE_INTERVAL;
            _isActive = false;
            _beam.enabled = false;
            _beam.SetPosition(0, transform.position);
            _beam.SetPosition(1, transform.position);
        }

        public void SetUp(UnitComponents shooter = null, Action<IPoolable> RemoveFromActivePoolables = null)
        {
            OnRemoveFromActivePoolables = RemoveFromActivePoolables;

            if (shooter != null)
            {
                Instigator = shooter;
            }
        }

        public void SetValues(float maxLength, float thickness, int damageOverTime, float duration, bool canBeDeflected)
        {
            _maxLength = maxLength;
            _thickness = thickness;
            _damageOverTime = damageOverTime;
            _duration = duration;
            CanBeDeflected = canBeDeflected;
        }

        /// <summary>
        /// Starts the laser.
        /// </summary>
        /// <param name="autoStop">Dictates whether the laser should stop by itself after a preset given time, or continue running until stopped by the weapon that started it.</param>
        public void Activate(bool autoStop = true)
        {
            _audioSource = GameManager.Instance.AudioManager.Play(_onShootingAudio);

            _beam.enabled = true;
            _isActive = true;
            _isDealingDamage = true;
            _hitParticles.Play();
            _muzzleFlashParticles.Play();
            SetMaterialDisolveProperties(0);

            if (autoStop)
            {
                this.StopCoroutineSafe(_laserCoroutine);
                _laserCoroutine = StartCoroutine(LaserCountdownRoutine());
            }

            _damageTimer = DAMAGE_INTERVAL;
        }

        private void SetMaterialDisolveProperties(float value)
        {
            _material.SetFloat(ALPHA_CLIP_ID, 1 - value);
            _material.SetFloat(NOISE_CLIP_ID, 1 + value);
        }

        private IEnumerator LaserCountdownRoutine()
        {
            yield return new WaitForSeconds(_duration);
            Deactivate();
        }

        private void FixedUpdate()
        {
            if (!_isDealingDamage || !_beam.enabled)
            {
                return;
            }

            UpdateLaser();
        }

        private void UpdateLaser()
        {
            ForwardDirection = transform.forward;
            ForwardDirection.y = 0f;

            Ray ray = new Ray(transform.position, ForwardDirection);
            bool cast = Physics.SphereCast(ray, _thickness, out RaycastHit hit, _maxLength, _targetLayerMask);
            ImpactPoint = transform.position + ForwardDirection * _maxLength;
            _damageTimer += Time.fixedDeltaTime;

            if (cast && (1 << hit.transform.gameObject.layer & _targetLayerMask) == 1 << hit.transform.gameObject.layer)
            {
                ImpactedObject = hit.collider.gameObject;
                float distanceToTarget = Vector3.Distance(transform.position, hit.point);
                ImpactPoint = transform.position + distanceToTarget * ForwardDirection;

                // resize the texture on the laser
                Vector2 textureSeizRange = new Vector2(1f, _maxLength / _textureTilingSize);
                float tilingSize = textureSeizRange.Lerp(Mathf.InverseLerp(0f, _maxLength, distanceToTarget));
                _material.SetFloat(LENGTH_ID, tilingSize);


                if (_damageTimer >= DAMAGE_INTERVAL)
                {
                    if (cast && hit.transform.TryGetComponent(out IDamageable damagable))
                    {
                        // checks for damagables
                        if (damagable.IsInvincible)
                        {
                            return;
                        }

                        DamageInfo damageInfo = DamageInfo.Create()
                            .SetDamage(_damageOverTime)
                            .SetInstigator(Instigator)
                            .SetBulletPosition(ImpactPoint)
                            .SetDamageDealer(this)
                            .Build();

                        damagable.TakeDamage(damageInfo);
                    }

                    _damageTimer = 0f;
                }

                _beam.SetPosition(0, transform.position);
                _beam.SetPosition(1, ImpactPoint);

                _hitParticles.transform.position = ImpactPoint;
            }
        }

        public void UpdatePositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }

        public void Deactivate()
        {
            if(!_isActive)
            {
                return;
            }

            StartCoroutine(DeactivationProcess());
        }

        private IEnumerator DeactivationProcess()
        {
            StopEffects();
            _isDealingDamage = false;
            _damageTimer = DAMAGE_INTERVAL;

            float timeElapsed = 0f;
            StopAudio();

            while (timeElapsed < LASER_FADE_OUT_TIME)
            {
                timeElapsed += Time.deltaTime;
                float t = timeElapsed / LASER_FADE_OUT_TIME;
                SetMaterialDisolveProperties(t);

                yield return null;
            }

            ResetBeam();
            _isActive = false;

            OnReleaseToPool(this);
            OnRemoveFromActivePoolables(this);
            gameObject.SetActive(false);
        }

        private void StopEffects()
        {
            _hitParticles.Stop();
            _muzzleFlashParticles.Stop();
        }

        private void ResetBeam()
        {
            _beam.enabled = false;
            _beam.SetPosition(0, transform.position);
            _beam.SetPosition(1, transform.position);
        }

        private void StopAudio()
        {
            if (_audioSource == null)
            {
                Debug.LogError($"No audio source attached to {gameObject.name}");
                return;
            }

            _audioSource.Stop();
            _audioSource.loop = false;
            _audioSource.clip = null;
        }

        public void SetTargetLayerMask(string tag)
        {
            _targetLayerMask = 0;
            _targetLayerMask |= Constants.MutualHittableLayer;
            _targetLayerMask |= Constants.WallLayer;
            _targetLayerMask |= Constants.DestructibleLayer;

            if (tag == TanksTag.Enemy.ToString())
            {
                _targetLayerMask |= 1 << Constants.EnemyDamagableLayer;
            }
            else
            {
                _targetLayerMask |= 1 << Constants.PlayerDamagableLayer;
            }
        }

        private void OnDisable()
        {
            if (_isActive)
            {
                TurnOff();
            }
        }

        #region Pool
        public override void Init(Action<IPoolable> OnRelease)
        {
            base.Init(OnRelease);
        }

        public override void TurnOff()
        {
            base.TurnOff();

            StopAudio();

            OnReleaseToPool(this);
        }

        public override void OnRequest()
        {
            base.OnRequest();
        }

        public override void OnRelease()
        {
            Deactivate();
            GameManager.Instance.SetParentToSpawnables(gameObject);

            if (_laserCoroutine != null)
            {
                StopCoroutine(_laserCoroutine);
            }
        }

        public override void Clear()
        {
            base.Clear();
        }
        #endregion
    }
}
