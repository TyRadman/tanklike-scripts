using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;

    public class TankConstraints : MonoBehaviour, IController
    {
        private PlayerComponents _playerComponents;
        private List<AbilityConstraint> _currentConstraints = new List<AbilityConstraint>();
        private AbilityConstraint _activeConstraint = AbilityConstraint.None;
        private List<IConstraintedComponent> _constraintComponents = new List<IConstraintedComponent>();

        public bool IsActive { get; private set; }

        public void SetUp(IController controller)
        {
            PlayerComponents components = controller as PlayerComponents;

            if (components == null)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = components;

            _constraintComponents = new List<IConstraintedComponent>();

            _constraintComponents.Add(_playerComponents.Movement);
            _constraintComponents.Add(_playerComponents.CrosshairController);

            if (_playerComponents.Shooter is TankShooter shooter)
            {
                _constraintComponents.Add(shooter);
            }
            else
            {
                Helper.LogWrongComponentsType(GetType());
            }

            _constraintComponents.Add(_playerComponents.Health);
            _constraintComponents.Add(_playerComponents.PlayerBoost);
            _constraintComponents.Add(_playerComponents.ChargeAttack);
            _constraintComponents.Add(_playerComponents.AimAssist);
            _constraintComponents.Add(_playerComponents.SuperAbilities);
            _constraintComponents.Add(_playerComponents.Jump);
            _constraintComponents.Add(_playerComponents.Tools);
            _constraintComponents.Add(_playerComponents.UIController.InventoryController);
        }

        public void ApplyConstraints(bool enable, AbilityConstraint constraints)
        {
            if (enable)
            {
                _currentConstraints.Add(constraints);
            }
            else
            {
                _currentConstraints.Remove(constraints);
            }

            _activeConstraint = GetCurrentConstraint();

            _constraintComponents.ForEach(c => c.ApplyConstraint(_activeConstraint));
        }

        private AbilityConstraint GetCurrentConstraint()
        {
            AbilityConstraint con = AbilityConstraint.None;

            for (int i = 0; i < _currentConstraints.Count; i++)
            {
                con |= _currentConstraints[i];
            }

            return con;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }
        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            if(_currentConstraints.Count > 0)
            {
                _currentConstraints.Clear();
            }

            _constraintComponents.ForEach(c => c.IsConstrained = false);

            // release all the constraints
            AbilityConstraint constraints = (AbilityConstraint)~0;
            ApplyConstraints(false, constraints);
        }

        public void Dispose()
        {

        }
        #endregion
    }

    [System.Flags]
    public enum AbilityConstraint
    {
        None = 0,
        Movement = 1,
        Rotation = 2,
        Shooting = 4,
        TakingDamage = 8,
        Boost = 16,
        HoldDownAction = 32,
        AimAssist = 64,
        SuperAbility = 128,
        Jump = 256,
        Tools = 512,
        Inventory = 1024,
    }
}
