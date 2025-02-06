using System.Collections;
using UnityEngine;

namespace TankLike
{
    [System.Serializable]
    public class StatIconReference
    {
        [field: SerializeField] public Sprite IconSprite { get; private set; }
        //[field: SerializeField] public StatType StatType { get; private set; }
        [field: SerializeField] public StatModifierType StatType { get; private set; }
    }
}
