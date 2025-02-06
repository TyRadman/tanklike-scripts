using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    public class State : ScriptableObject, IState
    {
        [field: SerializeField] public EnemyStateType EnemyStateType;

        protected StateMachine<EnemyStateType> _stateMachine;
        protected EnemyComponents _enemyComponents;

        public const string MENU_PATH = "TankLike/Enemies/States/";

        public virtual void SetUp(StateMachine<EnemyStateType> stateMachine, EnemyComponents enemyComponents)
        {
            _stateMachine = stateMachine;
            _enemyComponents = enemyComponents;
        }

        public virtual void OnEnter()
        {
            _enemyComponents.AIController.SetCurrentState(EnemyStateType);
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
