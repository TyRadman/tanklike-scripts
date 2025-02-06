using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment
{
    [SelectionBase]
    public class WallTile : MonoBehaviour
    {
        [field: SerializeField] public Transform BaseTransform { get; private set; }
        [field: SerializeField] public Transform TopTransform { get; private set; }

    }
}
