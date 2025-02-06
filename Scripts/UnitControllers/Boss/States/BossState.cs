using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    public class BossState : ScriptableObject, IState
    {
        [field: SerializeField] public BossStateType BossStateType;

        protected StateMachine<BossStateType> _stateMachine;
        protected BossComponents _bossComponents;

        public const string MENU_PATH = Directories.BOSSES + "States/";

        public virtual void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            _stateMachine = stateMachine;
            _bossComponents = bossComponents;
        }

        public virtual void OnEnter()
        {
            _bossComponents.AIController.SetCurrentState( BossStateType);
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnDispose()
        {
        }
    }
}
