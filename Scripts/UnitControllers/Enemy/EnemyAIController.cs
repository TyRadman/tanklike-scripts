using TankLike.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using UnitControllers.States;

    public class EnemyAIController : MonoBehaviour, IController
    {
        [Header("Debug")]
        [SerializeField] protected bool _movementDebug;
        [SerializeField] protected EnemyStateType _currentState;

        [Header("State Machine")]
        [SerializeField] protected EnemyStateType _initialState;
        [SerializeField] protected EnemyStateType _activationState;
        [SerializeField] protected List<State> _states;
        //[field: SerializeField] public BehaviorTreeRunner _BTRunner;

        [Header("Settings")]
        [SerializeField] protected EnemyType _enemyType;
        [SerializeField] protected bool _initialInactiveState;
        [SerializeField] protected CollisionEventPublisher _aggroTrigger;

        protected StateMachine<EnemyStateType> _stateMachine;
        protected EnemyComponents _enemyComponents;
        protected EnemyMovement _movement;
        protected EnemyShooter _shooter;
        protected EnemyHealth _health;
        protected EnemyTurretController _turretController;

        protected Dictionary<EnemyStateType, IState> _statesDictionary = new Dictionary<EnemyStateType, IState>();

        public bool IsActive { get; set; }

        public void SetUp(IController controller)
        {
            if (controller is not EnemyComponents enemyComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _enemyComponents = enemyComponents;

            _movement = (EnemyMovement)_enemyComponents.Movement;
            _shooter = (EnemyShooter)_enemyComponents.Shooter;
            _health = (EnemyHealth)_enemyComponents.Health;
            _turretController = (EnemyTurretController)_enemyComponents.TurretController;

            InitStateMachine();

            if (_initialState != EnemyStateType.IDLE)
            {
                Activate();
            }

            // TODO: see if still need the aggro trigger
            if(_aggroTrigger != null)
            {
                _aggroTrigger.enabled = false;
            }
        }

        protected virtual void InitStateMachine()
        {
            _stateMachine = new StateMachine<EnemyStateType>();
            _statesDictionary = new Dictionary<EnemyStateType, IState>();

            foreach (var state in _states)
            {
                var newState = Instantiate(state);
                newState.SetUp(_stateMachine, _enemyComponents);
                _statesDictionary.Add(state.EnemyStateType, newState);
            }

            _stateMachine.Init(_statesDictionary);
        }

        public State GetStateByType(EnemyStateType type)
        {
            return (State)_statesDictionary[type];
        }

        public void ChangeState(EnemyStateType type)
        {
            _stateMachine.ChangeState(type);
        }

        private void Update()
        {
            if (_stateMachine != null)
            {
                _stateMachine.Update();
            }
        }

        public void ActivateEnemy()
        {
            _stateMachine.ChangeState(_activationState);
            //_BTRunner.Run();
        }

        private void OnPlayerDetected(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Invoke(nameof(ActivateEnemy), 0.5f);
                _aggroTrigger.enabled = false;
            }
        }

        public void SetCurrentState(EnemyStateType state)
        {
            _currentState = state;
        }

        #region IController
        public void Activate()
        {
            //Debug.Log("Activate AI controller");

            // TODO: see if still need the aggro trigger
            if (!_initialInactiveState)
            {
                Invoke(nameof(ActivateEnemy), 0.5f);
            }
            else
            {
                _aggroTrigger.OnTriggerEnterEvent += OnPlayerDetected;
            }
        }

        public void Deactivate()
        {
            _stateMachine.ChangeState(_initialState);
        }

        public void Restart()
        {
            _stateMachine.SetInitialState(_initialState);

            //if (_initialInactiveState)
            //{
            //    _aggroTrigger.OnTriggerEnterEvent -= OnPlayerDetected;
            //    _aggroTrigger.enabled = true;
            //}
        }

        public void Dispose()
        {
            if (_initialInactiveState)
            {
                _aggroTrigger.OnTriggerEnterEvent -= OnPlayerDetected;
                _aggroTrigger.enabled = true;
            }
        }
        #endregion
    }
}
