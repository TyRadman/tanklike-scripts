using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankBodyPart : MonoBehaviour
    {
        [field: SerializeField] public List<Renderer> Meshes { get; private set; }
    }
}
