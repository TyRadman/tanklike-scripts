using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Combat.Abilities;
    using TankLike.Utils;

    public class TankSuperAbilities : MonoBehaviour, IController, IConstraintedComponent, ISkill
    {
        public bool IsActive { get; protected set; }
        public bool IsConstrained { get; set; }

        protected Dictionary<SuperAbilityHolder, SuperAbilityHolder> _superAbilityHolders = new Dictionary<SuperAbilityHolder, SuperAbilityHolder>();
        protected SuperAbilityHolder _currentSuperAbilityHolder;
        protected SuperAbilityHolder _lastSuperAbilityHolder;

        private TankComponents _components;

        public virtual void SetUp(IController controller)
        {
            if (controller is not TankComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _components = components;
        }

        public virtual void UseAbility()
        {
            // use the skill
            _currentSuperAbilityHolder.Ability.PerformAbility();
        }

        #region Skill Addition
        /// <summary>
        /// Adds the ability to the list of abilities of the entity.
        /// </summary>
        /// <param name="newAbilityHolder">Ability to add.</param>
        public virtual void AddSkill(SkillHolder newAbilityHolder)
        {
            if(newAbilityHolder == null)
            {
                Debug.Log($"No ability passed to {gameObject.name}");
                return;
            }

            if (newAbilityHolder is not SuperAbilityHolder superAbilityHolder)
            {
                Helper.LogWrongSkillHolder(gameObject.name, typeof(BoostAbilityHolder).Name, newAbilityHolder.GetType().Name);
                return;
            }

            if (_superAbilityHolders.ContainsKey(superAbilityHolder))
            {
                Debug.Log($"Ability {newAbilityHolder.name} already exists in the list of abilities {gameObject.name} has.");
                return;
            }

            // temporary. We need to look into whether we need scriptable objects for this, or stick to in-game instances
            SuperAbilityHolder holder = Instantiate(superAbilityHolder);

            Ability ability = Instantiate(holder.Ability);
            holder.Ability = ability;

            _superAbilityHolders.Add(superAbilityHolder, holder);

            if(_currentSuperAbilityHolder == null) 
            {
                EquipSkill(superAbilityHolder);   
            }
        }

        /// <summary>
        /// Equips the tank with the passed ability.
        /// </summary>
        /// <param name="abilityHolder">The ability holder that has the ability to equip.</param>
        public virtual void EquipSkill(SkillHolder newAbilityHolder)
        {
            if (newAbilityHolder == null)
            {
                Debug.Log($"No ability passed to {gameObject.name}");
                return;
            }

            if (newAbilityHolder is not SuperAbilityHolder superAbilityHolder)
            {
                Helper.LogWrongSkillHolder(gameObject.name, typeof(BoostAbilityHolder).Name, newAbilityHolder.GetType().Name);
                return;
            }

            // if the ability doesn't exist in the super abilities list, then add it
            if (!_superAbilityHolders.ContainsKey(superAbilityHolder))
            {
                Debug.LogWarning("Ability to set doesn't exist. Have you instantiated the ability somewhere else?");
                return;
            }

            _lastSuperAbilityHolder = superAbilityHolder;
            _currentSuperAbilityHolder = _superAbilityHolders[superAbilityHolder];
            _currentSuperAbilityHolder.Ability.SetUp(_components);
        }

        public void ReEquipSkill()
        {
            EquipSkill(_lastSuperAbilityHolder);
        }
        #endregion

        public virtual void ApplyConstraint(AbilityConstraint constraints)
        {
            bool canUseAbility = (constraints & AbilityConstraint.SuperAbility) == 0;
            IsConstrained = !canUseAbility;
        }

        #region Constraints

        #endregion

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {
        }

        public virtual void Dispose()
        {

        }
        #endregion
    }
}
