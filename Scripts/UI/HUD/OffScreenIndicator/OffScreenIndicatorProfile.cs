using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.HUD
{
    /// <summary>
    /// A profile for the offscreen indicators manager. It holds the target's transform, the icon and other additional informaiton.
    /// </summary>
    [System.Serializable]
    public class OffScreenIndicatorProfile
    {
        public OffScreenIcon IconPrefab;

        [HideInInspector] public bool FollowTarget = false;
        [HideInInspector] public bool IsShown = false;
        [HideInInspector] public Transform TargetTransform;
        [HideInInspector] public OffScreenIcon Icon;

        public void SetUp(Transform target, bool isShown = true, bool followTarget = true)
        {
            TargetTransform = target;
            IsShown = isShown;
            FollowTarget = followTarget;
        }
    }
}
