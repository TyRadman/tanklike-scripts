using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace TankLike.Environment.Shops
{
    using System;
    using UI.Workshop;

    public class WorkshopController : MonoBehaviour, IManager
    {
        public bool IsActive { get; private set; }
        /// <summary>
        /// The UI part of the workshop. This is where most of the workshop's functionality is located.
        /// </summary>
        [field: SerializeField] public WorkShopTabsNavigatable WorkshopUI { get; private set; }
        public Workshop_InteractableArea WorkShopArea { get; private set; }
        public Action OnWorkshopSpawned { get; set; }
        
        [SerializeField] private Workshop_InteractableArea _workshop;

        private CancellationTokenSource _spawnCancellationToken;

        private const int SPAWN_POINT_ORDER_FROM_PLAYER = 0;

        #region IManager
        public void SetUp()
        {

            Room shopRoom = GameManager.Instance.RoomsManager.Rooms.Find(r => r.RoomType == RoomType.Shop);

            WorkShopArea = Instantiate(_workshop);
            WorkShopArea.gameObject.SetActive(false);
            WorkshopUI.SetUp();

            _spawnCancellationToken = new CancellationTokenSource();
        }

        public void Dispose()
        {
            WorkshopUI.Dispose();
            OnWorkshopSpawned = null;
        }
        #endregion

        internal void SpawnWorkshopInRoom()
        {
            StartCoroutine(SpawnProcessRoutine());
            OnWorkshopSpawned?.Invoke();
        }

        private IEnumerator SpawnProcessRoutine()
        {
            GameManager manager = GameManager.Instance;

            WorkShopArea.transform.eulerAngles = Vector3.zero;
            Room currentRoom = manager.RoomsManager.CurrentRoom;

            Vector3 playerPosition = manager.PlayersManager.GetPlayer(0).transform.position;
            Vector3 spawnPoint = currentRoom.Spawner.SpawnPoints.
                GetClosestSpawnPointToPosition(playerPosition, SPAWN_POINT_ORDER_FROM_PLAYER, 2f).position;
            spawnPoint.y = Constants.GroundHeight;

            WorkShopArea.transform.position = spawnPoint;
            WorkShopArea.transform.parent = currentRoom.Spawnables.SpawnablesParent;

            float spawnDuration = manager.VisualEffectsManager.Misc.PlayPlayerSpawnVFX(spawnPoint, 2f) / 2;

            yield return new WaitForSeconds(spawnDuration);

            WorkShopArea.gameObject.SetActive(true);

            yield return new WaitForSeconds(spawnDuration);
        }

        public async Task SpawnProcessAsync()
        {
            try
            {
                CancellationToken token = _spawnCancellationToken.Token;

                GameManager manager = GameManager.Instance;

                WorkShopArea.transform.eulerAngles = Vector3.zero;
                Room currentRoom = manager.RoomsManager.CurrentRoom;

                Vector3 playerPosition = manager.PlayersManager.GetPlayer(0).transform.position;
                Vector3 spawnPoint = currentRoom.Spawner.SpawnPoints.
                    GetClosestSpawnPointToPosition(playerPosition, SPAWN_POINT_ORDER_FROM_PLAYER).position;

                WorkShopArea.transform.position = spawnPoint;

                float spawnDuration = manager.VisualEffectsManager.Misc.PlayEnemySpawnVFX(spawnPoint) / 2;

                await Task.Delay(TimeSpan.FromSeconds(spawnDuration), token);

                WorkShopArea.gameObject.SetActive(true);

                await Task.Delay(TimeSpan.FromSeconds(spawnDuration), token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Spawn process was canceled.");
            }
        }

        private void OnDestroy()
        {
            if(_spawnCancellationToken == null)
            {
                return;
            }

            _spawnCancellationToken.Cancel();
            _spawnCancellationToken.Dispose();
        }
    }
}
