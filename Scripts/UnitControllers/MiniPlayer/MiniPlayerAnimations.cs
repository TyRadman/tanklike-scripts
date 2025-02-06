using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;

    public class MiniPlayerAnimations : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }

        private MiniPlayerComponents _components;
        private Animator _turretAnimator;
        private Animator _wheelAnimator;
        private List<ParticleSystem> _dustParticles = new List<ParticleSystem>();

        //Turret
        protected readonly int _moveForward = Animator.StringToHash("MoveForward");
        protected readonly int _moveBackward = Animator.StringToHash("MoveBackward");
        protected readonly int _turnAmount = Animator.StringToHash("TurnAmount");
        protected readonly int _shootHash = Animator.StringToHash("Shoot");


        public void SetUp(IController controller)
        {
            if (controller is not MiniPlayerComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _components = components;
            MiniPlayerBodyParts parts = components.BodyParts;

            var carrier = (TankCarrier)parts.GetBodyPartOfType(BodyPartType.Carrier);
            var turret = (TankTurret)parts.GetBodyPartOfType(BodyPartType.Turret);

            if (carrier != null)
            {
                _wheelAnimator = carrier.Animator;
                _dustParticles = carrier.DustParticles;
            }

            _turretAnimator = turret.Animator;
        }

        public void PlayShootAnimation(float speed = 1f)
        {
            _turretAnimator.speed = speed;
            _turretAnimator.SetLayerWeight(1, 1);
            _turretAnimator.SetTrigger(_shootHash);
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
            _turretAnimator.ResetTrigger(_moveForward);
            _turretAnimator.ResetTrigger(_moveBackward);
            _turretAnimator.SetLayerWeight(2, 0);
            _turretAnimator.SetFloat(_turnAmount, 0f);

            _turretAnimator.SetLayerWeight(1, 0);
            _turretAnimator.ResetTrigger(_shootHash);
        }
        #endregion
    }
}
