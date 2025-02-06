using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Utils
{
    public class StateMachine<TStatesType> where TStatesType : System.Enum
    {
        public IState _currentState { get; private set; }
        public TStatesType _currentStateType { get; private set; }

        private Dictionary<string, object> _blackBoard;
        private Dictionary<TStatesType, IState> _states;

        public StateMachine()
        {
            _blackBoard = new Dictionary<string, object>();
        }

        public void Init(Dictionary<TStatesType, IState> states)
        {
            _states = states;
        }

        public void SetInitialState(TStatesType stateType)
        {
            ChangeState(stateType);
        }

        public void SetBlackBoardValue(string name, object value)
        {
            if (_blackBoard.ContainsKey(name))
                _blackBoard[name] = value;
            else
                _blackBoard.Add(name, value);
        }

        public object GetBlackboardValue(string name)
        {
            if (_blackBoard.ContainsKey(name))
                return _blackBoard[name];

            return null;
        }

        public void ChangeState(TStatesType stateType)
        {
            if (_states.ContainsKey(stateType))
            {
                if (_currentState != null)
                {
                    _currentState.OnExit();
                }

                _currentStateType = stateType;
                _currentState = _states[stateType];
                _currentState.OnEnter();
            }
            else
            {
                throw new System.ArgumentException(string.Format("State of type {0} is not present in State Machine!", stateType.ToString()));
            }
        }

        public void Update()
        {
            if (_currentState != null)
                _currentState.OnUpdate();
        }

        public void OnExit()
        {
            if (_currentState != null)
            {
                _currentState.OnExit();
            }

            _currentState = null;
        }

        public void Dispose()
        {
            if (_states == null)
            {
                return;
            }

            foreach (var state in _states)
            {
                state.Value.OnDispose();
            }

            _states = null;
            _currentState = null;
        }
    }
}
