using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.UI.HUD;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class MiniPlayerSpawner : MonoBehaviour, IController
    {
        public bool IsActive{ get; private set; }

        [SerializeField] private MiniPlayerComponents _miniPlayerPrefab;

        private MiniPlayerComponents _miniPlayerInstance;
        private PlayerComponents _components;

        public void SetUp(IController controller)
        {
            if(controller is not PlayerComponents playerComponents)
            {
                Debug.LogError("MiniPlayerSpawner can only be set up with PlayerComponents");
                return;
            }

            _components = playerComponents;
        }

        public void SpawnMiniPlayer()
        {
            bool hasBeenSpawned = _miniPlayerInstance != null;

            if (!hasBeenSpawned)
            {
                _miniPlayerInstance = Instantiate(_miniPlayerPrefab, transform.position, Quaternion.identity);
                GameManager.Instance.PlayersManager.AddMiniPlayer(_miniPlayerInstance);
                _miniPlayerInstance.SetUp(_components);
            }
            else
            {
                _miniPlayerInstance.PositionUnit(transform);
            }

            if (!_miniPlayerInstance.gameObject.activeSelf)
            {
                _miniPlayerInstance.gameObject.SetActive(true);
            }

            _miniPlayerInstance.Restart();
            _miniPlayerInstance.Activate();

            if(!hasBeenSpawned)
            {
                Color skinColor = _miniPlayerInstance.Stats.Skins[_miniPlayerInstance.PlayerIndex].Color;
                OffScreenIndicatorTarget indicatorTarget = _miniPlayerInstance.GetComponent<OffScreenIndicatorTarget>();

                if (indicatorTarget != null)
                {
                    indicatorTarget.SetColor(skinColor);
                }

                
            }
        }

        internal void DespawnMiniPlayer()
        {
            if(_miniPlayerInstance == null)
            {
                Debug.LogError("No MiniPlayerInstance to despawn");
                return;
            }

            _miniPlayerInstance.Deactivate();
            _miniPlayerInstance.Dispose();
            _miniPlayerInstance.gameObject.SetActive(false);
        }

        public Transform GetMiniPlayerTransform()
        {
            return _miniPlayerInstance.transform;
        }

        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        public void Dispose()
        {

        }

        public void Restart()
        {

        }
    }
}
