using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using ItemsSystem;

    /// <summary>
    /// Represents the details of a specific item drop, including its type, chance of dropping, quantity range, 
    /// and any factors that can affect its drop probability. Used as part of the <b>DestructibleDrop</b> class 
    /// to define individual item drop chances for destructible objects.
    /// </summary>
    [System.Serializable]
    public class DropChance
    {
        [field: SerializeField] public CollectableType DropType { get; private set; }
        // [HideInInspector]
        [ReadOnly] public float CurrentChance = 1.0f;
        [Range(0f, 1f)] public float Chance = 1f;
        //[field: SerializeField, Header("Chances Affectors")] public ChanceAffector Affector { get; private set; }
        //public Vector2 EffectRange;
        [field : SerializeField] public AffectorData Affector;

        [System.Serializable]
        public struct AffectorData
        {
            public ChanceAffector AffectorType;
            public Vector2 EffectRange;
        }
    }
}
