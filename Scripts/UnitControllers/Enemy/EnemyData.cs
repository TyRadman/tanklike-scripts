using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    /// <summary>
    /// A unique
    /// </summary>
    [CreateAssetMenu(fileName = "ED_NAME", menuName = Directories.ENEMIES + "Enemy data")]
    public class EnemyData : UnitData
    {
        [field: SerializeField] public string EnemyName { get; private set; }
        [field: SerializeField] public EnemyType EnemyType { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public int ExperiencePerKill { get; private set; }
        [field: SerializeField] public int Rank { get; private set; }
        [field: SerializeField] public UnitParts PartsPrefab { get; private set; }
        [field: SerializeField] public Sprite Avatar { get; private set; }

    }
}
