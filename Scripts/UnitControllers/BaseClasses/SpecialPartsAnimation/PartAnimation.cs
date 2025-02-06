using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;

    [System.Serializable]
    public class PartAnimation
    {
        [SerializeField] private Animation Animation;
        [SerializeField] private List<PartAnimationReference> AnimationReferences = new List<PartAnimationReference>();

        public void PlayAnimation(PartAnimationReference animationReference)
        {
            if (Animation.isPlaying)
            {
                Animation.Stop();
            }

            Animation.clip = animationReference.AnimationClip;
            Animation.Play();
        }
        
        public bool ReferenceExists(PartAnimationReference animationReference)
        {
            return AnimationReferences.Contains(animationReference);
        }
    }
}
