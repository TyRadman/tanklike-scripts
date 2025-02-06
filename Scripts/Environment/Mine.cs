using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankLike.Utils;
using System;
using TankLike.Combat;

namespace TankLike.UnitControllers
{
    public class Mine : MonoBehaviour, IPoolable
    {
        // public only because the editor is using its value
        public float DetectionRaduiss = 1f;
        public SphereCollider MineCollider;
        [Header("Stats")]
        public float DamageRaduis = 3f;
        [SerializeField] private int _damage = 10;
        [SerializeField] private float _triggerCountDown = 3f;
        [Header("Visuals")]
        [SerializeField] private MeshRenderer _mesh;
        [SerializeField] private Vector2 _emissionRange;
        [SerializeField] private Vector2 _beepingFrequencyRange;
        [SerializeField] private LayerMask _targetsMask;
        [SerializeField] private List<string> _targetTags;
        private TankComponents _minesUser;

        public Action<IPoolable> OnReleaseToPool { get; private set; }

        private Material _mineMaterial;
        private const string EMISSION_KEY = "_EmissionStrength";
        private const string DISTANCE_KEY = "_Distance";

        private void Awake()
        {
            _mineMaterial = _mesh.material;
        }

        private void OnTriggerEnter(Collider other)
        {
            // TODO: use alignment instead
            if (_targetTags.Exists(t => other.CompareTag(t)))
            {
                StartCoroutine(ExplosionProcess());
            }
        }

        private IEnumerator ExplosionProcess()
        {
            float time = 0f;
            float timer = 0f;

            // the death beeping
            while (time < _triggerCountDown)
            {
                float deltaTime = Time.deltaTime;
                time += deltaTime;
                float t = time / _triggerCountDown;
                // transition to the red color
                _mineMaterial.SetFloat(DISTANCE_KEY, t);
                // beeping process
                timer += deltaTime * _beepingFrequencyRange.Lerp(t);
                float emission = Mathf.Abs(_emissionRange.y * Mathf.Sin(timer * Mathf.PI) + _emissionRange.x);
                _mineMaterial.SetFloat(EMISSION_KEY, emission);
                yield return null;
            }

            Collider[] targets = Physics.OverlapSphere(transform.position, DamageRaduis, _targetsMask);

            // inflict damage to all targets in Area
            for (int i = 0; i < targets.Length; i++)
            {
                DamageInfo damageInfo = DamageInfo.Create()
                    .SetDamage(_damage)
                    .SetInstigator(_minesUser)
                    .SetDirection(Vector3.one)
                    .SetBulletPosition(transform.position)
                    .Build();

                targets[i].GetComponent<IDamageable>().TakeDamage(damageInfo);
            }

            // visuals
            var vfx = GameManager.Instance.VisualEffectsManager.Explosions.DeathExplosion; // temporary
            vfx.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            vfx.gameObject.SetActive(true);
            vfx.Play();

            OnReleaseToPool(this);
            // reset the mine's emission color and strength
            _mineMaterial.SetFloat(DISTANCE_KEY, 0);
            _mineMaterial.SetFloat(EMISSION_KEY, _emissionRange.y);
        }

        public void SetTriggerers(TankComponents user)
        {
            TankAlignment userTag = user.Alignment;
            _targetTags.Clear();
            // we cache the user of the mines to keep track of scores and level ups if the mine lands any kills
            _minesUser = user;

            // set the target according to the user's tankTag
            if(userTag == TankAlignment.PLAYER)
            {
                _targetsMask = GameManager.Instance.Constants.EnemiesLayerMask;
                _targetTags.Add(GameManager.Instance.Constants.EnemyTag);
            }
            else if (userTag == TankAlignment.ENEMY)
            {
                _targetsMask = GameManager.Instance.Constants.PlayersLayerMask;
                _targetTags.Add(GameManager.Instance.Constants.PlayerTag);
            }
        }

        #region Pool
        public void Init(Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }

        public void OnRequest()
        {
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }

#if UNITY_EDITOR
    // decided to mess around and see if we can create our own draggable gizmos :)
    [CustomEditor(typeof(Mine))]
    public class CirclePointEditor : Editor
    {
        private void OnSceneGUI()
        {
            Mine mine = (Mine)target;
            Vector3 minePosition = mine.transform.position;
            // caching it is always more performant
            float detection = mine.DetectionRaduiss;

            // Draw draggable handle on circle for min
            Handles.color = Color.yellow;
            Vector3 newPosition = Handles.FreeMoveHandle(minePosition + Vector3.left * detection, Quaternion.identity, 0.1f, Vector3.zero, Handles.DotHandleCap);
            detection = Vector3.Distance(minePosition, newPosition);
            // draw the actualy circles
            Handles.DrawWireDisc(minePosition, Vector3.up, detection);

            float damage = mine.DamageRaduis;

            // Draw draggable handle on circle for max
            Handles.color = Color.red;
            newPosition = Handles.FreeMoveHandle(minePosition + Vector3.right * damage, Quaternion.identity, 0.1f, Vector3.zero, Handles.DotHandleCap);
            damage = Vector3.Distance(minePosition, newPosition);
            Handles.DrawWireDisc(minePosition, Vector3.up, damage);

            mine.MineCollider.radius = detection;
            mine.DetectionRaduiss = detection;
            mine.DamageRaduis = damage;
        }
    }
#endif
}
