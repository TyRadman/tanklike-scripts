using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using System.Linq;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_ThreeCannon_Attack", menuName = MENU_PATH + "Three Cannon/Attack State")]
    public class ThreeCannonBossAttackState : BossAttackState
    {
        private Transform _target;
        [SerializeField] private List<StateChance> _attackStatesChances = new List<StateChance>();
        private StateChance _lastSelectedState;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            _attackStatesChances.ForEach(s => s.SetUp());
            base.SetUp(stateMachine, bossComponents);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _isActive = true;

            ChooseAttack();
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

        private void ChooseAttack()
        {
            float chance = Random.value * _attackStatesChances.FindAll(s => s.UseAttack).OrderByDescending(s => s.Chance).FirstOrDefault().Chance;
            List<StateChance> selectedStates = _attackStatesChances.FindAll(s => s.Chance >= chance && s.UseAttack);
            StateChance selectedState = selectedStates.RandomItem();
            
            if(selectedState != _lastSelectedState)
            {
                if(_lastSelectedState != null)
                {
                    // reset the chances of the last selected state if it's no longer chosen
                    _attackStatesChances.ForEach(s => s.IncreaseChance());
                }

                _lastSelectedState = selectedState;
            }

            // reduce the chances of the state to be chosen again
            _lastSelectedState.OnStateSelected();
            _stateMachine.ChangeState(_lastSelectedState.State.BossStateType);
        }
    }
}
