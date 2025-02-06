using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class AnimatorEventPublisher : StateMachineBehaviour
    {
        [SerializeField] private string _stateName;
        [SerializeField] private bool _hasSpecialEvent;
        [SerializeField] private float _specialEventTime = 1f;

        public string StateName => _stateName;

        public System.Action<float> OnEnter;
        public System.Action OnSpecialEvent;
        public System.Action OnExit;
        public System.Action OnUpdate;

        private bool _specialEventHasFired;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnEnter?.Invoke(stateInfo.length);

            if (_hasSpecialEvent)
                _specialEventHasFired = false;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnUpdate?.Invoke();

            if (_hasSpecialEvent)
            {
                if (stateInfo.normalizedTime >= _specialEventTime && !_specialEventHasFired)
                {
                    _specialEventHasFired = true;
                    OnSpecialEvent?.Invoke();
                }
            }      
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnExit?.Invoke();
            //Debug.Log("EXIT");
        }
    }
}
