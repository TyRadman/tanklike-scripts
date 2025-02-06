using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;
    using Minimap;
    using TankLike.Attributes;

    [SelectionBase]
    public abstract class TankComponents : UnitComponents
    {
        [field: SerializeField, InSelf] public PlayerStatsController StatsController { private set; get; }
        [field: SerializeField, InSelf] public KnockBackController KnockBackController { private set; get; }
        [field: SerializeField, InSelf] public CharacterController CharacterController { private set; get; }
        [field: SerializeField, InSelf] public TankTurretController TurretController { private set; get; }
        [field: SerializeField, InSelf] public TankShooter Shooter { private set; get; }
        [field: SerializeField, InSelf] public TankHealth Health { private set; get; }
        [field: SerializeField, InSelf] public TankMovement Movement { private set; get; }
        [field: SerializeField, InSelf] public TankSuperAbilities SuperAbility { private set; get; }
        [field: SerializeField, InSelf] public TankElementsEffects ElementsEffects { private set; get; }
        [field: SerializeField, InSelf] public TankVisuals Visuals { private set; get; }
        [field: SerializeField] public UnitData Stats { private set; get; }
        [field: SerializeField, InSelf] public TankAnimation Animation { private set; get; }
        [field: SerializeField, InSelf] public TankConstraints Constraints { private set; get; }
        [field: SerializeField, InSelf] public TankAdditionalInfo AdditionalInfo { private set; get; }
        [field: SerializeField, InSelf] public TankWiggler TankWiggler { private set; get; }
        [field: SerializeField, InSelf] public TankBodyParts TankBodyParts { private set; get; }
        [field: SerializeField, InSelf] public UnitVisualEffects VisualEffects { private set; get; }
        [field: SerializeField, InSelf] public TankSpecialPartsAnimation SpecialPartsAnimation { set; get; }

        [SerializeField] protected UnitMinimapIcon _minimapIcon;


        protected List<IController> _components = new List<IController>();

        protected void SetUpController(IController controller, Type fallBackType = null)
        {
            // Check for null and set fall back type for logging
            if(controller == null)
            {
                Helper.LogSetUpNullReferences(fallBackType);
                return;
            }

            controller.SetUp(this);
        }

        protected void AddComponentToList<T>(T controller) where T : IController
        {
            if (controller == null)
            {
                Helper.LogSetUpNullReferences(typeof(T));
                return;
            }

            _components.Add(controller);
        }

        /// <summary>
        /// Plays effects, handles minimap icon, and freezes the screen
        /// </summary>
        public virtual void OnDeathHandler(TankComponents tank)
        {
            // TODO: check this when working on elements
            //ElementsEffects.SetCanGetEffects(false);

            if (_minimapIcon != null)
            {
                _minimapIcon.KillIcon();
            }
            else
            {
                Debug.LogError($"No minimap on {gameObject.name}");
            }
        }

        public override T GetUnitComponent<T>()
        {
            foreach (IController component in _components)
            {
                if (component is T)
                {
                    return (T)component;
                }
            }

            return default;
        }

        public void SetAlignment(TankAlignment alignment)
        {
            Alignment = alignment;
        }
    }
}
