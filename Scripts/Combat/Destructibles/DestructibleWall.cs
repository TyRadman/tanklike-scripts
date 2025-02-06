using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.Combat.Destructible
{
    using Attributes;
    using UI.DamagePopUp;
    using Utils;

    [SelectionBase]
    public class DestructibleWall : DestructibleDropper
    {
        [System.Serializable]
        public struct DamageThreshold
        {
            public float Percentage;
            public Mesh Mesh;
        }

        [Header("Settings")]
        [SerializeField] private DamageThreshold[] _damageThresholds;

        [Header("References")]
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField, InSelf] private Collider _collider;
        [SerializeField, InSelf] private NavMeshObstacle _navMeshObstacle;

        [Header("Effects")]
        [SerializeField] private ParticleSystem _onShatterParticles;
        [SerializeField] private ParticleSystem _onHitParticles;
        [SerializeField] private ParticleSystem _dustShakeParticles;
        [SerializeField] private float _hitFlashTime = 0.1f;

        [Header("Animations")]
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _shakeAnimation;

        [Header("Customize")]
        [SerializeField] private bool _showDamagePopup;
        [SerializeField] private bool _applyHitEffect;

        private const string TEXTURE_IMPACT_KEY = "_TextureImpact";

        private int _currentMeshIndex;
        private Coroutine _hitFlashCoroutine;

        // For testing only
        private void Start()
        {
            _currentHealth = _maxHealth;
            _currentMeshIndex = 0;
        }

        public void SetUp()
        {
            _currentHealth = _maxHealth;
            _currentMeshIndex = 0;
        }

        public override void Die()
        {
            Debug.Log("Dead");
            //_meshFilter.gameObject.SetActive(false);
            _collider.enabled = false;
            _navMeshObstacle.enabled = false;

            CollectablesDropRequest collectablesDropSettings = new CollectablesDropRequest()
            {
                Position = transform.position,
                DropperTag = DropperTag,
                Settings = _dropSettings,
                Drops = _drops
            };

            GameManager.Instance.CollectableManager.SpawnCollectablesOfType(collectablesDropSettings);
        }

        public override void TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead)
            {
                return;
            }

            int damage = damageInfo.Damage;
            Vector3 direction = damageInfo.Direction;
            Vector3 bulletPosition = damageInfo.BulletPosition;

            _currentHealth -= damage;

            if (_showDamagePopup && PopUpAnchor != null)
            {
                DamagePopUpType type = damage > 0 ? DamagePopUpType.Damage : DamagePopUpType.Heal;
                GameManager.Instance.DamagePopUpManager.DisplayPopUp(type, damage, PopUpAnchor.Anchor);
            }

            if (_applyHitEffect)
            {
                this.StopCoroutineSafe(_hitFlashCoroutine);
                _hitFlashCoroutine = StartCoroutine(HitFlashRoutine());
            }

            // Play Effects
            _onHitParticles.transform.position = bulletPosition;
            
            if (direction != Vector3.zero)
            {
                _onHitParticles.transform.rotation = Quaternion.LookRotation(-direction);
            }

            _onHitParticles.Play();
            _dustShakeParticles.Play();

            // Play animation
            if (_animation.isPlaying)
            {
                _animation.Stop();
            }

            _animation.clip = _shakeAnimation;
            _animation.Play();

            for (int i = _currentMeshIndex; i < _damageThresholds.Length; i++)
            {
                if ((float)_currentHealth / (float)_maxHealth <= _damageThresholds[i].Percentage)
                {
                    _meshFilter.mesh = _damageThresholds[_currentMeshIndex].Mesh;
                    _currentMeshIndex++;
                    _onShatterParticles.Play();
                }
            }

            if (_currentHealth <= 0f)
            {
                _currentHealth = 0;
                Die();
            }
        }

        private IEnumerator HitFlashRoutine()
        {

            _meshRenderer.material.SetFloat(TEXTURE_IMPACT_KEY, 0);
            
            yield return new WaitForSeconds(_hitFlashTime);

            _meshRenderer.material.SetFloat(TEXTURE_IMPACT_KEY, 1);            
        }
    }
}
