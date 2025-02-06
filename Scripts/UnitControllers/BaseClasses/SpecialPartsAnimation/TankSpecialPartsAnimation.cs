using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;

    public class TankSpecialPartsAnimation : MonoBehaviour, IController
    {
        [SerializeField] private List<PartAnimation> _partAnimations = new List<PartAnimation>();
        public bool IsActive { get; private set; }

        private PlayerComponents _playerComponents;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;
        }

        public void PlaySpecialPartAnimation(PartAnimationReference animationReference)
        {
            PartAnimation animation = _partAnimations.Find(a => a.ReferenceExists(animationReference));

            if (animation != null)
            {
                animation.PlayAnimation(animationReference);
            }
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
