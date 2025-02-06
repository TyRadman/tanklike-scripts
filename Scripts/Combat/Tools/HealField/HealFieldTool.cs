using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = NAME_PREFIX + "HealField", menuName = ASSET_MENU_ROOT + "Heal Field")]
    public class HealFieldTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private HealField _healFieldPrefab;
        [SerializeField] private float _spawnHeight = 0.55f;

        private Transform _tankTransform;
        private HealField _healField;

        private Pool<HealField> _healFieldPool;

        public override void SetUp(TankComponents tankTransform)
        {
            base.SetUp(tankTransform);

            _tankTransform = tankTransform.transform;

            InitPool();
        }

        private void InitPool()
        {
            _healFieldPool = new Pool<HealField>(
                () =>
                {
                    var obj = Instantiate(_healFieldPrefab);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj;
                },
                (HealField obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (HealField obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (HealField obj) => obj.GetComponent<IPoolable>().Clear(),
                0
            );
        }

        public override void UseTool()
        {
            base.UseTool();

            Vector3 position = _tankTransform.position;
            position.y = _spawnHeight;

            _healField = _healFieldPool.RequestObject(position, Quaternion.identity);
            _healField.gameObject.SetActive(true);
            _healField.Activate(true, _duration);
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }

        public override void Dispose()
        {
            base.Dispose();

            _healFieldPool.Clear();
        }
    }
}
