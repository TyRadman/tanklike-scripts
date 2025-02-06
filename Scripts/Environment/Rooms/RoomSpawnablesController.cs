using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike.Environment
{
    public class RoomSpawnablesController : MonoBehaviour
    {
        [field: SerializeField] public Transform SpawnablesParent { get; private set; }

        private List<Transform> _droppers = new List<Transform>();
        private List<Transform> _explosives = new List<Transform>();

        /// <summary>
        /// Adds a dropper to the room.
        /// </summary>
        /// <param name="dropper">The dropper to add.</param>
        public void AddDropper(Transform dropper)
        {
            _droppers.Add(dropper);
        }

        /// <summary>
        /// Removes a dropper from the room.
        /// </summary>
        /// <param name="dropper">The dropper to remove.</param>
        public void RemoveDropper(Transform dropper)
        {
            _droppers.Remove(dropper);
        }

        public List<Transform> GetAimAssistTargets()
        {
            _droppers.RemoveAll(d => d == null);
            _explosives.RemoveAll(d => d == null);

            return _droppers.Concat(_explosives).ToList();
        }

        #region Explosives
        public void AddExplosive(Transform explosive)
        {
            _explosives.Add(explosive);
        }

        public void RemoveExplosive(Transform explosive)
        {
            _explosives.Remove(explosive);
        }

        internal List<Transform> GetExplosives()
        {
            return _explosives;
        }
        #endregion
    }
}
