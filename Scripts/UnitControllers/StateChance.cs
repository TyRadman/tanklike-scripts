using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers.States;
using UnityEngine;

namespace TankLike
{
    /// <summary>
    /// Holds an enemy or boss state with a spawn chance for it
    /// </summary>
    [Tooltip("Holds an boss state with a spawn chance for it")]
    [System.Serializable]
    public class StateChance
    {
        [field: SerializeField] public BossState State { get; private set; }
        [SerializeField][Range(0f, 1f)] private float _startChance = 0.5f;
        [SerializeField] private int _maxRepetitions = 2;
        [field: SerializeField] public float Chance { get; private set; }
        [field: SerializeField] public bool UseAttack { get; private set; }


        public void SetUp()
        {
            Chance = _startChance;
        }

        public void OnStateSelected()
        {
            // take the chance down according to the number of times the ability is allowed to be used
            Chance = Mathf.Max(0f, Chance - _startChance / _maxRepetitions);
        }

        public void IncreaseChance()
        {
            Chance = Mathf.Min(_startChance, Chance + _startChance / _maxRepetitions);
        }
    }
}
