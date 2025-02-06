using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers.States;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public enum BossType
    {
        None = 0,
        ThreeCannon = 1,
    }

    public class BossAIController : MonoBehaviour, IController
    {
        public System.Action<BossAIController> OnBossDeath;

        [Header("Debug")]
        [SerializeField] protected bool _movementDebug;
        [SerializeField] protected BossStateType _currentState;

        [Header("State Machine")]
        [SerializeField] protected BossStateType _initialState;
        [SerializeField] protected BossStateType _activationState;
        [SerializeField] protected List<BossState> _states;

        [Header("Settings")]
        [SerializeField] protected BossType _bossType;
        [SerializeField] protected bool _initialInactiveState;

        protected StateMachine<BossStateType> _stateMachine;
        protected BossComponents _bossComponents;
        protected BossHealth _health;
        protected BossMovementController _movementController;
        protected BossAttackController _attackController;

        protected Dictionary<BossStateType, IState> _statesDictionary = new Dictionary<BossStateType, IState>();
        [SerializeField] private ThreeCannonBossAttackState _attackState;

        public bool IsActive { get; private set; }

        public void SetUp(IController controller)
        {
            BossComponents components = controller as BossComponents;

            if (components == null)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _bossComponents = components;

            _movementController = (BossMovementController)_bossComponents.Movement;
            _attackController = _bossComponents.AttackController;
            _health = (BossHealth)_bossComponents.Health;

            InitStateMachine();

            _stateMachine.SetInitialState(_initialState);

            StartBossIntroduction();
        }

        protected virtual void InitStateMachine()
        {
            //Debug.Log("Init state machine");
            _stateMachine = new StateMachine<BossStateType>();
            _statesDictionary = new Dictionary<BossStateType, IState>();

            foreach (BossState state in _states)
            {
                BossState newState = Instantiate(state);
                newState.SetUp(_stateMachine, _bossComponents);
                _statesDictionary.Add(state.BossStateType, newState);

                if(newState is ThreeCannonBossAttackState attackState)
                {
                    _attackState = attackState;
                }
            }

            _stateMachine.Init(_statesDictionary);
        }

        private void Update()
        {
            if (_stateMachine != null)
            {
                _stateMachine.Update();
            }
        }

        public void SetCurrentState(BossStateType state)
        {
            _currentState = state;
        }

        public void ActivateBoss()
        {
            Debug.Log("ACTIVATE BOSS");
            _stateMachine.ChangeState(_activationState);
            _bossComponents.Movement.Activate();
        }

        public State GetStateByType(BossStateType type)
        {
            return (State)_statesDictionary[type];
        }

        public void StartBossIntroduction()
        {
            _stateMachine.ChangeState(BossStateType.Introduction);
        }

        public void FinishBossIntroduction()
        {
            Activate();
        }

        #region IController
        public void Activate()
        {
            Debug.Log("ACTIVATE");
            IsActive = true;

            Invoke(nameof(ActivateBoss), 0.5f);
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
            _stateMachine.ChangeState(BossStateType.Death);
        }
        #endregion
    }
}
