using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using ItemsSystem;
    using Utils;

    /// <summary>
    /// Component that is attached to enemies that is resposnsible for dropping collectables that the players can collect.
    /// </summary>
    public class EnemyItemDropper : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }

        // to be set from the enemy data
        [SerializeField] private List<CollectableType> _droppedCollectables;

        private EnemyComponents _enemyComponents;
        private List<CollectableType> _collectablesToDrop;
        // TODO: must be replaced with a drop chance instead
        private float _dropChance = 0.5f;
        private bool _dropItemsOnDeath = true;

        public void SetUp(IController controller)
        {
            if (controller is not EnemyComponents enemyComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _enemyComponents = enemyComponents;
        }

        public void DropItem()
        {
            if (!_dropItemsOnDeath)
            {
                return;
            }

            GameManager.Instance.CollectableManager.SpawnRandomCollectables(_dropChance, transform.position, _collectablesToDrop.RandomItem());

            _enemyComponents.TankBodyParts.HandlePartsExplosion();
        }

        public void SetAsKeyHolder()
        {
            _collectablesToDrop = new List<CollectableType>();
            _collectablesToDrop.Add(CollectableType.BossKey);
            _dropChance = 1f;
        }

        /// <summary>
        /// The enemy won't drop any collectables upon death.
        /// </summary>
        public void DisableDrops()
        {
            _dropItemsOnDeath = false;
        }

        /// <summary>
        /// The enemy will drop any collectables upon death.
        /// </summary>
        public void EnableDrops()
        {
            _dropItemsOnDeath = true;
        }


        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            _collectablesToDrop = _droppedCollectables;
            _dropItemsOnDeath = true;
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
