using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment
{
    public class GroundTile : MonoBehaviour
    {
        [field:SerializeField] public MeshRenderer MeshRenderer { get; private set; }
        [field:SerializeField] public MeshFilter MeshFilter { get; private set; }
    }
}
