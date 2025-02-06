using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Combat.SkillTree;
    using System;

    /// <summary>
    /// Manages the abilities that the player gains through the skill tree.
    /// </summary>
    public class PlayerSkillsController : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }
        [SerializeField] private SkillTreePrefab _SkillTreePrefab;

        public void SetUp(IController controller)
        {

        }

        public SkillTreeHolder GetSkillTree()
        {
            return _SkillTreePrefab.SkillTree;
        }

        #region IController
        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        public void Restart()
        {

        }

        public void Dispose()
        {

        }
        #endregion
    }
}
