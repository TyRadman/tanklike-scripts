using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    [CreateAssetMenu(fileName = "Level Destructible Data", menuName = "Level/Level Generation/ Destructible Data")]
    public class LevelDestructibleData_SO : ScriptableObject
    {
        public List<DestructibleDrop> DropsData = new List<DestructibleDrop>();
    }
}
