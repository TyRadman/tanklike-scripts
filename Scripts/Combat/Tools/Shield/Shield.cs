using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.Shields
{
    using Combat;
    using Utils;

    public class Shield : MonoBehaviour, IDamageable
    {
        public bool IsInvincible { get; private set; }
        public Transform Transform { get; private set; }
        public DamagePopUpAnchor PopUpAnchor { get; private set; }
        public bool IsDead { get; private set; }
        public float Size { get; private set; }

        [Header("References")]
        [SerializeField] private SphereCollider _collider;
        [SerializeField] private MeshRenderer _shieldMesh;

        [Header("Damage Taking")]
        [SerializeField] private ParticleSystem _rippleParticles;
        [SerializeField] private ParticleSystem _impactParticle;
        [SerializeField] private Animation _animation;

        [Header("Modules")]
        [SerializeField] private List<ShieldOnImpactModule> _onImpactModules = new List<ShieldOnImpactModule>();
        private List<ShieldOnImpactModule> _onImpactModulesInstances = new List<ShieldOnImpactModule>();

        [Header("Other")]
        [SerializeField] private float _playerRotationFollowSpeed = 0.3f;
        
        /// <summary>
        /// The transform that the shield follows when activated.
        /// </summary>
        private Transform _owner;
        private TankHealth _tankHealth;
        private Material _shieldMaterial;
        private Material _shieldRippleMaterial;
        private float _maxAlpha;
        private int _previousTankLayer;
        private bool _isActive = false;

        private const string SHIELD_ALPHA_KEY = "_ShieldAlpha";
        private const string SPHERE_CENTER_KEY = "_SphereCenter";
        private const string RIPPLE_MASK_RADIUS = "_Radius";
        private const string ALPHA = "_ShieldAlpha";
        public const float FADING_DURATION = 0.2f;

        public void SetUp(TankComponents components)
        {
            _shieldRippleMaterial = _rippleParticles.GetComponent<ParticleSystemRenderer>().material;
            _shieldMaterial = _shieldMesh.material;
            _collider.enabled = false;

            if (components is PlayerComponents playerComponents)
            {
                _owner = playerComponents.Shield.GetShieldParent();
            }
            else
            {
                _owner = components.transform;
            }
            
            
            _tankHealth = components.Health;

            _maxAlpha = _shieldMaterial.GetFloat(ALPHA);

            SetShieldAlpha(0f);

            for (int i = 0; i < _onImpactModules.Count; i++)
            {
                ShieldOnImpactModule module = Instantiate(_onImpactModules[i]);
                module.SetUp(components, this);
                _onImpactModulesInstances.Add(module);
            }
        }

        #region Activation
        public void ActivateShield()
        {
            _collider.enabled = true;
            transform.parent = null;

            transform.localScale = Vector3.one * Size;

            _isActive = true;
            StartCoroutine(ShieldFollowProcess(_owner));
            StartCoroutine(ShowShieldRoutine());

            // cache the owner's layer before changing it
            _previousTankLayer = _tankHealth.GetDamageDetectorsLayer();

            // change the layer of the owner's damagables
            int layer = GameManager.Instance.Constants.Alignments.Find(a => a.Alignment == TankAlignment.NEUTRAL).LayerNumber;
            _tankHealth.SetDamageDetectorsLayer(layer);
        }

        private IEnumerator ShowShieldRoutine()
        {
            float time = 0f;

            while (time < FADING_DURATION)
            {
                time += Time.deltaTime;
                SetShieldAlpha(Mathf.Lerp(0f, _maxAlpha, time / FADING_DURATION));
                yield return null;
            }
        }

        public void DeactivateShield()
        {
            StartCoroutine(ShieldDeactivationRoutine());
        }

        private IEnumerator ShieldDeactivationRoutine()
        {
            float currentAlpha = _shieldMaterial.GetFloat(ALPHA);
            float time = 0f;

            while (time < FADING_DURATION)
            {
                time += Time.deltaTime;
                SetShieldAlpha(Mathf.Lerp(currentAlpha, 0f, time / FADING_DURATION));
                yield return null;
            }

            transform.parent = _owner;
            _isActive = false;
            _collider.enabled = false;
            _tankHealth.SetDamageDetectorsLayer(_previousTankLayer);
        }

        private IEnumerator ShieldFollowProcess(Transform target)
        {
            while (_isActive)
            {
                transform.SetPositionAndRotation(target.position, Quaternion.Slerp(transform.rotation, target.rotation, _playerRotationFollowSpeed));
                yield return null;
            }
        }

        public void SetShieldAlpha(float value)
        {
            _shieldMaterial.SetFloat(SHIELD_ALPHA_KEY, value);
        }
        #endregion

        public void SetShieldUser(TankAlignment side)
        {
            TankSide sideInfo = GameManager.Instance.Constants.Alignments.Find(s => s.Alignment == side);
            gameObject.layer = sideInfo.LayerNumber;
        }

        public void SetSize(float size)
        {
            Size = size;
            transform.localScale = Vector3.one * size;
            _shieldRippleMaterial.SetFloat(RIPPLE_MASK_RADIUS, size / 2f);
        }

        private void PlayImpactParticles(Vector3 position)
        {
            _impactParticle.transform.position = position;
            _impactParticle.Play();
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            _onImpactModulesInstances.ForEach(m => m.OnImpact(
                damageInfo.DamageDealer,
                damageInfo.Direction,
                damageInfo.BulletPosition
                ));

            PlayImpactParticles(damageInfo.BulletPosition);

            ShowDamage(damageInfo.BulletPosition);

            this.PlayAnimation(_animation);
        }

        public void ShowDamage(Vector3 contactPoint)
        {
            Vector3 position = (contactPoint - transform.position).normalized + transform.position;
            _shieldRippleMaterial.SetVector(SPHERE_CENTER_KEY, position);
            _rippleParticles.Play();
        }

        public T GetShieldOnImpactModule<T>() where T : ShieldOnImpactModule
        {
            ShieldOnImpactModule module = _onImpactModulesInstances.Find(m => m is T);

            if (module == null)
            {
                Debug.LogError($"No module of type {typeof(T)} found in {this.name}");
            }

            return module as T;
        }

        public void Die()
        {

        }
    }
}
