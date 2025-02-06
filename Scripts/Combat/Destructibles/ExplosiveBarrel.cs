using System.Collections;
using UnityEngine;

namespace TankLike.Combat.Destructible
{
    using TankLike.Cam;
    using TankLike.Environment;
    using UI.DamagePopUp;
    using UnitControllers;
    using Utils;

    [SelectionBase]
    public class ExplosiveBarrel : MonoBehaviour, IDamageable, IAimAssistTarget
    {
        [Header("Settings")]
        [SerializeField] private int _maxHealth;
        [SerializeField] private AOEWeapon _weapon;

        [Header("Timer")]
        [SerializeField] private bool _hasTimer;
        [SerializeField] private float _explosionTimer;
        [SerializeField] private Animation _timerAnimation;
        [SerializeField] private Vector2 _explosiveImpactExplosionTimeRange;

        [Header("Indicators")]
        [SerializeField] private bool _hasIndicator;
        [SerializeField] private IndicatorEffects.IndicatorType _indicatorType;

        [Header("Effects")]
        [SerializeField] protected Vector3 _explosionDecalSize = Vector3.one;
        [SerializeField] private float _hitFlashTime = 0.1f;

        [Header("References")]
        [SerializeField] private MeshRenderer[] _meshes;

        public bool IsInvincible { get; private set; }
        public Transform Transform { get; private set; }
        [field: SerializeField] public DamagePopUpAnchor PopUpAnchor { get; private set; }
        public bool IsDead { get; private set; }
        public System.Action<Transform> OnTargetDestroyed { get; set ; }

        private const string TEXTURE_IMPACT_KEY = "_TextureImpact";

        private int _currentHealth;
        private Coroutine _explosionCoroutine;
        private Indicator _indicator;
        private UnitComponents _instigator;
        private bool _explosionTriggered;

        private void Start()
        {
            _currentHealth = _maxHealth;
        }

        #region IAimAssistTarget
        public void AssignAsTarget(Room room)
        {
            if(room == null)
            {
                Debug.LogError("Room is null");
                return;
            }

            room.Spawnables.AddExplosive(transform);
            OnTargetDestroyed += room.Spawnables.RemoveExplosive;
        }
        #endregion

        #region IDamagable
        public void TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead)
            {
                return;
            }

            int damage = damageInfo.Damage;
            UnitComponents shooter = damageInfo.Instigator;

            if (_explosionTriggered)
            {
                // Replace the instigator
                if(shooter != null)
                {
                    _instigator = shooter;
                }

                ForceExplosion();
                return;
            }

            if (damageInfo.DamageType == ImpactType.Explosive)
            {
                if (_currentHealth <= 0)
                {
                    return;
                }

                _currentHealth = 0;
                _instigator = damageInfo.Instigator;
                StartExplosion(true);
                return;
            }

            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                _instigator = shooter;
                _explosionTriggered = true;
                StartExplosion();
            }

            if (PopUpAnchor != null)
            {
                if (damage > 0)
                {
                    GameManager.Instance.DamagePopUpManager.DisplayPopUp(
                        DamagePopUpType.Damage, damage, PopUpAnchor.Anchor);
                }
                else
                {
                    GameManager.Instance.DamagePopUpManager.DisplayPopUp(
                        DamagePopUpType.Heal, damage, PopUpAnchor.Anchor);
                }
            }

            StartCoroutine(HitFlashRoutine());
        }

        public void Die()
        {
            IsDead = true;

            OnTargetDestroyed?.Invoke(transform);
            OnTargetDestroyed = null;
        }
        #endregion

        private void StartExplosion(bool explosiveImpact = false)
        {
            if (explosiveImpact)
            {
                float timer = Random.Range(_explosiveImpactExplosionTimeRange.x, _explosiveImpactExplosionTimeRange.y);
                Invoke(nameof(Explode), timer);
                return;
            }

            if (_hasTimer)
            {
                this.StopCoroutineSafe(_explosionCoroutine);
                _explosionCoroutine = StartCoroutine(ExplosionRoutine());
            }
            else
            {
                Explode();
            }
        }

        private IEnumerator ExplosionRoutine()
        {
            float timer = 0f;

            this.PlayAnimation(_timerAnimation);

            if (_hasIndicator)
            {
                Vector3 indicatorSize = new Vector3(_weapon.ExplosionRadius * 2, 0f, _weapon.ExplosionRadius * 2);
                _indicator = GameManager.Instance.VisualEffectsManager.Indicators.GetIndicatorByType(_indicatorType);
                _indicator.gameObject.SetActive(true);
                var pos = transform.position;
                pos.y = Constants.GroundHeight;
                _indicator.transform.position = pos;
                _indicator.transform.rotation = Quaternion.identity;
                _indicator.transform.localScale = indicatorSize;
                _indicator.Play();
            }

            while (timer < _explosionTimer)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (_indicator != null)
            {
                _indicator.TurnOff();
            }

            Explode();
        }

        private void ForceExplosion()
        {
            if (_indicator != null)
            {
                _indicator.TurnOff();
            }

            Explode();
        }

        private void Explode()
        {
            Die();
            PlayExplosionEffects();
            _weapon.SetUp(_instigator);
            _weapon.SetExplosionCenter(transform);
            _weapon.OnShot();
            Destroy(gameObject); // TODO: return to pool
        }

        private void PlayExplosionEffects()
        {
            // Camera shake
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.EXPLOSION);

            Vector3 radius = new Vector3(_weapon.ExplosionRadius, _weapon.ExplosionRadius, _weapon.ExplosionRadius);

            // Explosion effect
            var explosion = GameManager.Instance.VisualEffectsManager.Explosions.AOEExplosion; // TODO: have different explosions for different barrels
            Vector3 pos = transform.position;
            pos.y = Constants.GroundHeight;
            explosion.transform.SetPositionAndRotation(pos, Quaternion.identity);
            explosion.transform.localScale = radius;
            explosion.gameObject.SetActive(true);
            explosion.Play();

            // Explosion decal effects
            var decal = GameManager.Instance.VisualEffectsManager.Explosions.ExplosionDecal;
            float randomAngle = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
            decal.transform.SetPositionAndRotation(pos, randomRotation);
            decal.transform.localScale = _explosionDecalSize;
            decal.gameObject.SetActive(true);
            decal.Play();
        }

        private IEnumerator HitFlashRoutine()
        {
            foreach (Renderer mesh in _meshes)
            {
                mesh.material.SetFloat(TEXTURE_IMPACT_KEY, 0);
            }

            yield return new WaitForSeconds(_hitFlashTime);

            foreach (Renderer mesh in _meshes)
            {
                mesh.material.SetFloat(TEXTURE_IMPACT_KEY, 1);
            }
        }
    }
}
