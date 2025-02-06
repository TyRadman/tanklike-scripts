using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "SPAR_NAME", menuName = Directories.ABILITIES + "Special Part Animations")]
    public class PartAnimationReference : ScriptableObject
    {
        [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    }
}
