using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class ThreeCannonBossAnimations : BossAnimations
    {
        [field: SerializeField] public Animator Animator { get; private set; }

        protected readonly int _moveHash = Animator.StringToHash("Move");
        protected readonly int _groundPoundHash = Animator.StringToHash("GroundPound");
        protected readonly int _rocketLauncherHash = Animator.StringToHash("RocketLauncher");
        protected readonly int _deathHash = Animator.StringToHash("Death");

        public void PlayMoveAnimation(bool value)
        {
            Animator.SetBool(_moveHash, value);
        }

        public void TriggerGroundPoundAnimation()
        {
            Animator.SetTrigger(_groundPoundHash);
        }

        public void TriggerRocketLauncherAnimation()
        {
            Animator.SetTrigger(_rocketLauncherHash);
        }

        public void TriggerDeathAnimation()
        {
            Animator.SetTrigger(_deathHash);
        }
    }
}
