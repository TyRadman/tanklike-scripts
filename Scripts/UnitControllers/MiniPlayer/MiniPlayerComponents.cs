using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Attributes;
    using Utils;

    public class MiniPlayerComponents : UnitComponents, IPlayerController
    {
        public int PlayerIndex { get; set; }
        public UnitComponents ComponentsController => this;

        [field: SerializeField, InSelf] public MiniPlayerMovement Movement  { private set; get; }
        [field: SerializeField, InSelf] public TankWiggler Wiggler { private set; get; }
        [field: SerializeField, InSelf] public MiniPlayerBodyParts BodyParts { private set; get; }
        [field: SerializeField, InSelf] public MiniPlayerCrosshairController Crosshair { private set; get; }
        [field: SerializeField, InSelf] public MiniPlayerTurretController TurretController { private set; get; }
        [field: SerializeField, InSelf] public MiniPlayerShooter Shooter { private set; get; }
        [field: SerializeField, InSelf] public PlayerOverHeat OverHeat { private set; get; }
        [field: SerializeField, InSelf] public PlayerDropsCollector DropsCollector { private set; get; }
        [field: SerializeField, InSelf] public MiniPlayerAnimations Animation { private set; get; }
        public PlayerData Stats { set; get; }

        [field: SerializeField, Header("Unity Defaults"), InSelf] public CharacterController CharacterController { private set; get; }


        private List<IController> _components = new List<IController>();


        public override void SetUp(IController controller)
        {
            if(controller is not PlayerComponents player)
            {
                Debug.LogError("No player passed");
                return;
            }

            PlayerIndex = player.PlayerIndex;

            if(player.Stats is PlayerData playerStats)
            {
                Stats = playerStats;
            }
            else
            {
                Debug.LogError("PlayerStats is not PlayerData");
            }

            AddComponentToList(BodyParts);
            AddComponentToList(Movement);
            AddComponentToList(Wiggler);
            AddComponentToList(Crosshair);
            AddComponentToList(TurretController);
            AddComponentToList(Shooter);
            AddComponentToList(OverHeat);
            AddComponentToList(DropsCollector);
            AddComponentToList(Animation);

            _components.ForEach(c => c.SetUp(this));
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

        public override T GetUnitComponent<T>()
        {
            T component = (T)_components.Find(c => c is T);

            if (component == null)
            {
                Debug.LogError($"Component {typeof(T)} not found in {this}");
            }

            return component;
        }

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

        #region IController
        public override void Activate()
        {
            base.Activate();
            _components.ForEach(c => c.Activate());
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _components.ForEach(c => c.Deactivate());
        }

        public override void Dispose()
        {
            _components.ForEach(c => c.Dispose());
        }

        public override void Restart()
        {
            _components.ForEach(c => c.Restart());
        }
        #endregion
    }
}
