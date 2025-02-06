using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    public class ShopsBuilder : MonoBehaviour
    {
        [SerializeField] private InteractableArea _shop;
        [SerializeField] private InteractableArea _workshop;

        private bool _spawnShop = true;
        private bool _spawnWorkshop = true;

        public void BuildShops()
        {
            _spawnShop = GameManager.Instance.GameData.SpawnShop;
            _spawnWorkshop = GameManager.Instance.GameData.SpawnWorkshop;

            if (!_spawnShop && !_spawnWorkshop)
            {
                return;
            }

            Room shopRoom = GameManager.Instance.RoomsManager.Rooms.Find(r => r.RoomType == RoomType.Shop);

            if (_spawnShop)
            {
                InteractableArea shop = Instantiate(_shop, shopRoom.transform);
                Vector3 shopPosition = shopRoom.Spawner.SpawnPoints.GetRandomSpawnPoint(true).position;
                shop.transform.eulerAngles = Vector3.zero;
                shop.transform.position = shopPosition;
            }
        }

        public bool IsBuildShops()
        {
            return GameManager.Instance.GameData.SpawnWorkshop || GameManager.Instance.GameData.SpawnShop;
        }
    }
}
