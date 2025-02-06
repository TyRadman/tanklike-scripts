using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
using Combat.Destructible;
    
    public class DestructiblesManager : MonoBehaviour, IManager
    {
        [Header("References")]
        [SerializeField] private LevelDestructibleData_SO _currentDestructibleDropData;

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            // set the highest drop chance for each dropper
            _currentDestructibleDropData.DropsData.ForEach(d => d.SetUp());
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        /// <summary>
        /// Sets the items that the dropper will drop when destroyed
        /// </summary>
        /// <param name="dropper"></param>
        public void SetDestructibleValues(IDropper dropper)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            // TODO: make it work
            // retreieve the data based on the tag
            //DestructibleDrop selectedTagCollectables = _currentDestructibleDropData.DropsData.Find(d => d.Tag == dropper.Tag);
            // set the drops tags to the destructible just build
            //dropper.SetCollectablesToSpawn(selectedTagCollectables);
        }

        public LevelDestructibleData_SO GetDestructibleDropData()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return null;
            }

            return _currentDestructibleDropData;
        }
    }
}
