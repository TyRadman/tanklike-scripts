using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_ThreeCannon_Idle", menuName = MENU_PATH + "Three Cannon/Idle State")]
    public class ThreeCannonBossIdleState : BossIdleState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            _isActive = true;
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
            _isActive = false;
        }

        public override void OnDispose()
        {
        }
    }
}
