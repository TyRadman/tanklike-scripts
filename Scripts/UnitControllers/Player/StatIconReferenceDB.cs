using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    /// <summary>
    /// Scriptable object that holds a list of StatIconReference.
    /// </summary>
    [CreateAssetMenu(fileName = "DB_StatIconReference", menuName = Directories.SETTINGS + "Stat Icon References DB")]
    public class StatIconReferenceDB : ScriptableObject
    {
        [SerializeField] private List<StatIconReference> _statIcons = new List<StatIconReference>();

        public StatIconReference GetStatIconReference(StatModifierType statType)
        {
            return _statIcons.Find(icon => icon.StatType == statType);
        }
    }

    public enum StatType
    {
        SpeedIncrement = 0,
        SpeedDecrement = 1,
        FullEnergy = 2,
        HealthRegeneration = 3,
    }
}
