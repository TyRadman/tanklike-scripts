using System.Collections;
using UnityEngine;

namespace TankLike.Combat
{
    using UnitControllers;

    [CreateAssetMenu(fileName = NAME_PREFIX + "Summon", menuName = ASSET_MENU_ROOT + "Summon")]
    public class SummonTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private SummonAIController _summonPrefab;
        [SerializeField] private Vector2 _spawnRadiusRange;
        [SerializeField] private float _spawnHeight;
        [SerializeField] private float _effectDuration = 3f;

        private const float RESIZING_DURATION = 0.25f;
        private TankComponents _playerComponents;

        private SummonAIController _summon;
        private Coroutine _spawnSummonCoroutine;

        public override void SetUp(TankComponents tankTransform)
        {
            base.SetUp(tankTransform);

            _playerComponents = tankTransform;

            // create the summon instance
            _summon = Instantiate(_summonPrefab);
            GameManager.Instance.SetParentToSpawnables(_summon.gameObject);
            _summon.transform.position = Vector3.zero;
            _summon.gameObject.SetActive(false);

            _summon.SetSummoner(_playerComponents);
            _summon.SetSpawnPoint(_spawnHeight, _spawnRadiusRange);
        }

        public override void SetDuration()
        {
            _duration = RESIZING_DURATION * 2 + _effectDuration;
        }

        public override void UseTool()
        {
            base.UseTool();

            _summon.transform.parent = null;
            Vector3 offset = new Vector3(Random.Range(_spawnRadiusRange.x, _spawnRadiusRange.y), _spawnHeight, Random.Range(_spawnRadiusRange.x, _spawnRadiusRange.y));
            _summon.transform.position = _playerComponents.transform.position + offset;

            _summon.gameObject.SetActive(true);

            _spawnSummonCoroutine =  _tank.StartCoroutine(SpawnSummonRoutine());
        }

        private IEnumerator SpawnSummonRoutine()
        {
            float time = 0f;

            // scale up the summon
            while (time < RESIZING_DURATION)
            {
                time += Time.deltaTime;
                float t = time / RESIZING_DURATION;
                _summon.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, t);

                yield return null;
            }

            _summon.Activate();
            GameManager.Instance.SummonsManager.AddSummon(_summon);

            yield return new WaitForSeconds(_effectDuration);

            // disable and reset the summon
            _summon.Restart();
            GameManager.Instance.SummonsManager.RemoveSummon(_summon);

            time = 0f;

            // scale down the summon
            while (time < RESIZING_DURATION)
            {
                time += Time.deltaTime;
                float t = time / RESIZING_DURATION;
                _summon.transform.localScale = Vector3.one * Mathf.Lerp(1f, 0f, t);

                yield return null;
            }

        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }

        public override void Dispose()
        {
            base.Dispose();

            _summon.Dispose();
        }
    }
}
