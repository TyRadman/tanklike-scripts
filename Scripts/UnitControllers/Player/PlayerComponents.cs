using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Attributes;

    public class PlayerComponents : TankComponents, IPlayerController
    {
        public GameplaySettings PlayerSettings;
        public UnitComponents ComponentsController => this;

        [field: SerializeField, InSelf] public PlayerShield Shield { get; private set; }
        [field: SerializeField, InSelf] public PlayerUpgrades Upgrades { get; private set; }
        [field: SerializeField, InSelf] public PlayerExperience Experience { get; private set; }
        [field: SerializeField, InSelf] public PlayerTools Tools { get; private set; }
        [field: SerializeField, InSelf] public PlayerOverHeat Overheat { get; private set; }
        [field: SerializeField, InSelf] public PlayerSuperAbilities SuperAbilities { get; private set; }
        [field: SerializeField, InSelf] public PlayerSuperAbilityRecharger SuperRecharger { get; private set; }
        [field: SerializeField, InSelf] public PlayerInteractions PlayerInteractions { get; private set; }
        [field: SerializeField, InSelf] public PlayerBoost PlayerBoost { get; private set; }
        [field: SerializeField, InSelf] public PlayerHoldAction ChargeAttack { get; private set; }
        [field: SerializeField, InSelf] public PlayerJump Jump { get; private set; }
        [field: SerializeField, InSelf] public PlayerCrosshairController CrosshairController { get; private set; }
        [field: SerializeField, InSelf] public PlayerUIController UIController { get; private set; }
        [field: SerializeField, InSelf] public PlayerPredictedPosition PredictedPosition { set; get; }
        [field: SerializeField, InSelf] public PlayerFuel Fuel { set; get; }
        [field: SerializeField, InSelf] public PlayerEnergy Energy { set; get; }
        [field: SerializeField, InSelf] public PlayerBoostAbility BoostAbility { set; get; }
        [field: SerializeField, InSelf] public PlayerAimAssist AimAssist { set; get; }
        [field: SerializeField, InSelf] public PlayerDropsCollector DropsCollector { set; get; }
        [field: SerializeField, InSelf] public PlayerSkillsController SkillsController { set; get; }
        [field: SerializeField, InSelf] public PlayerInGameUIController InGameUIController { set; get; }
        [field: SerializeField, InSelf] public PlayerWeaponSwapper PlayerWeaponSwapper { set; get; }
        [field: SerializeField, InSelf] public MiniPlayerSpawner MiniPlayerSpawner { set; get; }
        [field: SerializeField] public bool IsAlive { set; get; }

        [field: SerializeField, Header("Ability Data")] public AbilitySelectionData AbilityData { set; get; }
        public System.Action OnPlayerRevived { get; set; }

        public int PlayerIndex { get; private set; } = 0;

        public bool StartWithDefaultSkills { get; set; }

        [Header("Debug")]
        public Transform TestingSphere;

        private bool _isDisposed = false;

        /// <summary>
        /// For events that need to take place after the player is activated.
        /// </summary>
        public System.Action OnPlayerActivated { get; set; }
        /// <summary>
        /// For events that need to take place only once after the player is activated.
        /// </summary>
        public System.Action OnPlayerActivatedOnce { get; set; }

        public override void SetUp(IController controller = null)
        {
            _isDisposed = false;
            AddComponentToList(TankBodyParts);
            AddComponentToList(AdditionalInfo);

            // Movement
            AddComponentToList(Movement);
            AddComponentToList(TurretController);
            AddComponentToList(Jump);
            AddComponentToList(PlayerBoost);
            AddComponentToList(PredictedPosition);

            // Combat
            AddComponentToList(Overheat);
            AddComponentToList(Shooter);
            AddComponentToList(CrosshairController);
            AddComponentToList(AimAssist);
            AddComponentToList(ElementsEffects);
            AddComponentToList(SkillsController);

            // Visuals
            AddComponentToList(Animation);
            AddComponentToList(Visuals);
            AddComponentToList(TankWiggler);
            AddComponentToList(VisualEffects);

            // Stats
            AddComponentToList(Health);
            AddComponentToList(Fuel);
            AddComponentToList(Energy);
            AddComponentToList(Experience);
            AddComponentToList(Upgrades);
            AddComponentToList(DropsCollector);

            // Abilities
            AddComponentToList(Shield);
            AddComponentToList(SuperRecharger);
            AddComponentToList(SuperAbilities);
            AddComponentToList(ChargeAttack);
            AddComponentToList(BoostAbility);
            AddComponentToList(Tools);

            // UI
            AddComponentToList(UIController);
            AddComponentToList(InGameUIController);

            // Other
            AddComponentToList(Constraints);
            AddComponentToList(PlayerInteractions);
            AddComponentToList(PlayerWeaponSwapper);
            AddComponentToList(StatsController);
            AddComponentToList(KnockBackController);
            AddComponentToList(MiniPlayerSpawner);

            // set up all the components
            for (int i = 0; i < _components.Count; i++)
            {
                if(_components[i] is IUnitDataReciever)
                {
                    (_components[i] as IUnitDataReciever).ApplyData(Stats);
                }

                _components[i].SetUp(this);
            }
        }

        public void SetUpSettings()
        {
            ApplySettings(PlayerSettings);
        }

        public void OnRevived(int reviveHealthAmount)
        {
            _isDisposed = false;
            OnPlayerRevived?.Invoke();

            IsAlive = true;

            Restart();
            Activate();

            if (reviveHealthAmount > 0)
            {
                Health.SetHealthAmount(reviveHealthAmount, 1);
            }

            SpawnMinimapIcon();
            MiniPlayerSpawner.DespawnMiniPlayer();
            GameManager.Instance.HUDController.PlayerHUDs[PlayerIndex].SwitchToPlayerHUD();
            PositionUnit(MiniPlayerSpawner.GetMiniPlayerTransform());
        }

        public void SetIndex(int index)
        {
            PlayerIndex = index;
        }

        public override void OnDeathHandler(TankComponents components)
        {
            base.OnDeathHandler(components);

            Deactivate();
            Dispose();
        }

        public void ApplySettings(GameplaySettings settings)
        {
            // apply settings here
            CrosshairController.SetAimSensitivity(settings.AimSensitivity);
        }

        public void SpawnMinimapIcon()
        {
            _minimapIcon.SpawnIcon();
        }

        public void RemoveComponent(System.Type componentType)
        {
            IController component = _components.Find(c => c.GetType() == componentType);

            if(component == null)
            {
                Debug.Log($"No component of type {componentType.Name}");
                return;
            }

            component.Dispose();

            _components.Remove(component);
        }

        #region Utilities
        public override UnitShooter GetShooter()
        {
            return Shooter;
        }

        public override void PositionUnit(Transform point)
        {
            Vector3 position = point.position;
            position.y = 1f;
            Quaternion rotation = Quaternion.LookRotation(point.forward);
            transform.SetPositionAndRotation(position, Quaternion.identity);
            Movement.SetBodyRotation(rotation);
        }
        #endregion

        #region IController
        public override void Activate()
        {
            _components.ForEach(c => c.Activate());
            OnPlayerActivated?.Invoke();

            if (OnPlayerActivatedOnce != null)
            {
                OnPlayerActivatedOnce.Invoke();
                OnPlayerActivatedOnce = null;
            }

            // TODO: check this when working on elements
            //ElementsEffects.Activate();
        }

        public override void Deactivate()
        {
            _components.ForEach(c => c.Deactivate());
        }

        public override void Restart()
        {
            _components.ForEach(c => c.Restart());
            // TODO: check this when working on elements
            //ElementsEffects.Restart();
        }

        public override void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _components.ForEach(c => c.Dispose());
            // TODO: check this when working on elements
            //ElementsEffects.Dispose();
        }
        #endregion
    }
}
