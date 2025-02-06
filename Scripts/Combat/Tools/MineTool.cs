using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Combat
{
    [CreateAssetMenu(fileName = NAME_PREFIX + "Mine", menuName = ASSET_MENU_ROOT + "Mine")]
    public class MineTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private Mine _minePrefab;
        private Transform _tankTransform;

        private Pool<Mine> _minePool;

        public override void SetUp(TankComponents tankTransform)
        {
            base.SetUp(tankTransform);
            _currentAmount = _maxAmount;

            InitPool();

            _tankTransform = tankTransform.transform;
        }

        private void InitPool()
        {
            _minePool = new Pool<Mine>(
                () =>
                {
                    var obj = Instantiate(_minePrefab);
                    GameManager.Instance.SetParentToSpawnables(obj.gameObject);
                    return obj;
                },
                (Mine obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (Mine obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (Mine obj) => obj.GetComponent<IPoolable>().Clear(),
                0
            );
        }

        // We might need to add the mine user as a parameter later
        public override void UseTool()
        {
            base.UseTool();

            // set position
            Vector3 position = _tankTransform.position;
            position.y = 0.025f;

            Mine mine = _minePool.RequestObject(position, Quaternion.identity);
            mine.gameObject.SetActive(true);

            // set targets for the mine
            TankComponents components = _tankTransform.GetComponent<TankComponents>();
            mine.SetTriggerers(components);
        }
    }
}
