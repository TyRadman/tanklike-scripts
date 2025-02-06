using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class SummonAIController : MonoBehaviour, IController, IPoolable
    {
        [Header("State Machine")]
        [SerializeField] protected SummonStateType _initialState;
        [SerializeField] protected SummonStateType _activationState;

        [Header("Components")]
        [SerializeField] protected SummonMovement _summonMovement;
        [SerializeField] protected SummonShooter _summonShooter;
        protected StateMachine<SummonStateType> _stateMachine;

        [Header("Settings")]
        [SerializeField] protected float _standbyDuration;
        [SerializeField] protected float _attackStopDistance;
        [SerializeField] protected float _retreatDistance;

        private TankComponents _summoner;
        private float _spawnHeight;
        private Vector2 _spawnRadiusRange;
        private bool _isSpawned;

        public System.Action<IPoolable> OnReleaseToPool { get; private set; }

        public bool IsActive { get; set; }

        public void SetUp(IController controller)
        {
            if (controller == null || controller is not TankComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            TankComponents components = (TankComponents)controller;
        }

        public void SetSummoner(TankComponents summoner)
        {
            _summoner = summoner;

            _summonMovement.SetUp(summoner.transform);
            InitStateMachine();

            _stateMachine.SetInitialState(_initialState);
        }

        public void SetSpawnPoint(float height, Vector2 radiusRange)
        {
            _spawnHeight = height;
            _spawnRadiusRange = radiusRange;
        }

        protected virtual void InitStateMachine()
        {
            _stateMachine = new StateMachine<SummonStateType>();

            var states = new Dictionary<SummonStateType, IState>();

            var idleState = new SummonIdleState();
            states.Add(SummonStateType.IDLE, idleState);

            var followState = new SummonFollowState(_stateMachine, _summonMovement);
            states.Add(SummonStateType.FOLLOW, followState);

            var standbyState = new SummonStandbyState(_stateMachine, _summonMovement, _summonShooter, _standbyDuration, _retreatDistance);
            states.Add(SummonStateType.STANDBY, standbyState);

            var attackState = new SummonAttackState(_stateMachine, _summonMovement, _summonShooter, _attackStopDistance);
            states.Add(SummonStateType.ATTACK, attackState);

            _stateMachine.Init(states);
        }

        private void Update()
        {
            if (!IsActive)
                return;

            if (_stateMachine != null)
                _stateMachine.Update();
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
            Invoke(nameof(ActivateSummon), 0.2f);
            if (_isSpawned)
            {
                Vector3 offset = new Vector3(Random.Range(_spawnRadiusRange.x, _spawnRadiusRange.y), _spawnHeight, Random.Range(_spawnRadiusRange.x, _spawnRadiusRange.y));
                transform.position = _summoner.transform.position + offset;
            }
            _isSpawned = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void ActivateSummon()
        {
            _stateMachine.ChangeState(_activationState);
        }

        public void Dispose()
        {
            _stateMachine.Dispose();
        }

        public void Restart()
        {
            IsActive = false;
            _stateMachine.SetInitialState(_initialState);
            _isSpawned = false;
        }
        #endregion

        #region Pool
        public void Init(System.Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
        }

        public void OnRequest()
        {
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
