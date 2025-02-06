using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.LevelGeneration.Quests
{
    [CreateAssetMenu(fileName = "Level Settings", menuName = "Other/ Level Settings")]
    public class LevelQuestSettings : ScriptableObject
    {
        [field: SerializeField] public List<EnemyType> EnemyTypes;
    }
}
