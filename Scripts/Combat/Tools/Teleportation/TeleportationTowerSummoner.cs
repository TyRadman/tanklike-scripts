using UnityEngine;

namespace TankLike.Combat.Tools
{
    using TankLike.Utils;
    using UnitControllers;

    [CreateAssetMenu(fileName = NAME_PREFIX + "TeleportationTower", menuName = ASSET_MENU_ROOT + "Teleportation Tower")]
    public class TeleportationTowerSummoner : Tool
    {
        [SerializeField] private TeleportationTowerInteractableArea _teleportationTowerPrefab;

        private Pool<TeleportationTowerInteractableArea> _teleportationTowerPool;


        public override void SetUp(TankComponents tank)
        {
            base.SetUp(tank);

            _teleportationTowerPool = CreatePool(_teleportationTowerPrefab);
        }

        private Pool<TeleportationTowerInteractableArea> CreatePool(TeleportationTowerInteractableArea prefab)
        {
            var pool = new Pool<TeleportationTowerInteractableArea>(
                () =>
                {
                    var obj = MonoBehaviour.Instantiate(prefab);
                    GameManager.Instance.SetParentToRoomSpawnables(obj.gameObject);
                    return obj;
                },
                (TeleportationTowerInteractableArea obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (TeleportationTowerInteractableArea obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (TeleportationTowerInteractableArea obj) => obj.GetComponent<IPoolable>().Clear(),
                0
           );
            return pool;
        }

        public override void SetDuration()
        {
            base.SetDuration();
        }

        public override void UseTool()
        {
            base.UseTool();

            Vector3 spawnPosition = GameManager.Instance.RoomsManager.CurrentRoom.
                GetClosestSpawnPointToPosition(_tank.transform.position, 2);

            TeleportationTowerInteractableArea teleportationTower = _teleportationTowerPool.RequestObject(spawnPosition, Quaternion.identity);
            teleportationTower.gameObject.SetActive(true);
            teleportationTower.Activate();
        }

        public override bool CanUseTool()
        {
            if (GameManager.Instance.EnemiesManager.IsFightActivated())
            {
                Debug.Log("There are enemies, can't use tool!");
                return false;
            }

            return true;
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }

        public override void Dispose()
        {
            base.Dispose();
            _teleportationTowerPool.Clear();
            Debug.Log("DISPOSE TOWER SUMMONER");
        }
    }
}
