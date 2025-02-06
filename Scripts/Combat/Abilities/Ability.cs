using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using Combat.SkillTree;
    using Combat.SkillTree.Upgrades;
    using Misc;
    using ScreenFreeze;
    using UnitControllers;

    public abstract class Ability : ScriptableObject
    {
        /// <summary>
        /// The prefix of the asset
        /// </summary>
        public const string PREFIX = "Ability_";

        public List<SkillProperties> SkillDisplayProperties { get; set; } = new List<SkillProperties>();

        [Header("Values")]
        [SerializeField] private ScreenFreezeData _freezeData;
        protected float _duration = 0.5f;

        protected Transform _tankTransform;
        protected TankComponents _components;

        /// <summary>
        /// Whether the ability has been set up or not
        /// </summary>
        public bool IsSetUp { get; private set; } = false;

        public virtual void SetUp(TankComponents components)
        {
            IsSetUp = true;
            _tankTransform = components.transform;
            _components = components;
        }

        /// <summary>
        /// Starts the ability
        /// </summary>
        public virtual void PerformAbility()
        {
            // TODO: Remove from base class and implement it on each derivitive
            if (_freezeData != null)
            {
                GameManager.Instance.ScreenFreezer.FreezeScreen(_freezeData);
            }
        }

        /// <summary>
        /// Called when the ability hold-down starts
        /// </summary>
        public abstract void OnAbilityHoldStart();

        /// <summary>
        /// Called during the coroutine of the super ability hold down.
        /// </summary>
        public abstract void OnAbilityHoldUpdate();

        /// <summary>
        /// Called when the ability finishes performing.
        /// </summary>
        public abstract void OnAbilityFinished();

        /// <summary>
        /// Called when the ability is interrupted.
        /// </summary>
        public abstract void OnAbilityInterrupted();

        /// <summary>
        /// Upgrades the values of the ability.
        /// </summary>
        /// <param name="upgrade">The asset holding the upgrades.</param>
        public virtual void Upgrade(SkillUpgrade upgrade)
        {

        }

        /// <summary>
        /// Returns the duration of the ability.
        /// </summary>
        /// <returns></returns>
        public virtual float GetDuration()
        {
            return _duration;
        }

        /// <summary>
        /// Sets up the indicator by setting values that the ability has.
        /// </summary>
        /// <param name="indicator"></param>
        public abstract void SetUpIndicatorSpecialValues(AirIndicator indicator);

        /// <summary>
        /// Disposes the ability and all its dependencies.
        /// </summary>
        public virtual void Dispose()
        {
            IsSetUp = false;
        }

        /// <summary>
        /// Cache the skills' properties from the ability's crucial variables.
        /// </summary>
        public virtual void PopulateSkillProperties()
        {
            //SkillDisplayProperties.Clear();
        }

        #region 
        /// <summary>
        /// Adds a property that will be display when the skill is viewed in the skills selection menu or the skill tree.
        /// </summary>
        /// <param name="propertyName">The name of the property to display.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="valueColor">The color in which the value will be display.</param>
        /// <param name="valueUnit">The unit that will be displayed after the value.</param>
        public void AddSkillProperty(string propertyName, float value, Color valueColor, string valueUnit)
        {
            SkillProperties property = new SkillProperties()
            {
                Name = propertyName,
                Value = value.ToString(),
                DisplayColor = valueColor,
                UnitString = valueUnit
            };

            SkillDisplayProperties.Add(property);
        }
        #endregion

        private void OnDestroy()
        {
            Dispose();
        }
    }
}
