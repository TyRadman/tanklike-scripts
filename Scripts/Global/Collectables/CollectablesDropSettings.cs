using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ItemsSystem
{
    /// <summary>
    /// Holds settings for the drop. It's passed whenever the Collectables manager drops collectables.
    /// </summary>
    [System.Serializable]
    public class CollectablesDropSettings
    {
        [field : SerializeField] public bool HasDeathCountDown { get; set; }
    }
}
