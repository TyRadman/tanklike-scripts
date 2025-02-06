using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    [System.Serializable]
    public class UpgradeMainInfo
    {
        [field : SerializeField] public string Name { get; private set; }
        [field : SerializeField, TextArea(0, 10)] public string Description { get; private set; }
        [field : SerializeField] public Sprite Icon { get; private set; }
        [Tooltip("If true, the parent upgrade's main info will be overriden by this one.")]
        [field: SerializeField] public bool OverrideParentMainInfo { get; private set; } = false;
    }
}
