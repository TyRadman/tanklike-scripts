using UnityEngine;

namespace TankLike.Combat.SkillTree
{
    [CreateAssetMenu(fileName = "Preset_STLine", menuName = Directories.PRESETS + "Skill Tree Line")]
    public class SkillTreeLinePreset : ScriptableObject, IPreset
    {
        [Header("Unlocking line")]
        public Vector2 UnlockingLineScale;
        public float UnlockingLineAlpha;
        [Header("Locking line")]
        public Vector2 LockingLineScale;
        public float LockingLineAlpha;
    }
}
