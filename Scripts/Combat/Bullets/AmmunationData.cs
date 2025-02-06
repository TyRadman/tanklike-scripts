using System.Collections;
using System.Collections.Generic;
using TankLike.Misc;
using UnityEngine;

namespace TankLike.Combat
{
    /// <summary>
    /// Holds the data of a single type of ammuniation
    /// </summary>
    [CreateAssetMenu(fileName = "AmmunationData_NAME", menuName = Directories.AMMUNITION + "Ammunition data")]
    public class AmmunationData : ScriptableObject
    {
        [field: SerializeField] public string GUID { get; set; }
        [field: SerializeField] public Ammunition Ammunition { get; private set; }
        [field: SerializeField] public ParticleSystemHandler MuzzleFlash { get; private set; }
        [field: SerializeField] public ParticleSystemHandler Impact { get; private set; }
    }
}
