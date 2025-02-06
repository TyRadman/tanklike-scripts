using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = "Preset_STCell", menuName = Directories.PRESETS + "Skill Tree Cell")]
    public class SkillTreeCellPreset : ScriptableObject, IPreset
    {
        [field: SerializeField] public float ProgressAmount { get; private set; } = 0.0f;
        [field: SerializeField] public float ContentScale { get; private set; } = 1.0f;
    }
}
