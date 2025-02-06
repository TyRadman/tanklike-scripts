using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class BossComponents : TankComponents
    {
        [field: SerializeField] public BossAIController AIController { get; private set; }
        [field: SerializeField] public BossAttackController AttackController { get; private set; }
        [field: SerializeField] public BossAnimations Animations { get; private set; }
        
        public Vector3 RoomCenter { get; private set; }
        public Vector3 RoomSize { get; private set; }

        public override void OnDeathHandler(TankComponents components)
        {
            base.OnDeathHandler(components);
            Deactivate();
            Dispose();
        }

        public override UnitShooter GetShooter()
        {
            return Shooter;
        }

        public override void PositionUnit(Transform position)
        {
            Debug.Log("Implement PositionUnit");
        }

        #region IController
        public override void SetUp(IController controller = null)
        {
            // Set References
            RoomCenter = ((BossRoom)GameManager.Instance.RoomsManager.CurrentRoom).GetSpawnPoint().position;
            RoomSize = ((BossRoom)GameManager.Instance.RoomsManager.CurrentRoom).RoomSize;

            // Body Parts
            SetUpController(TankBodyParts, typeof(TankBodyParts));

            // Movement
            SetUpController(Movement, typeof(BossMovementController));

            // Combat
            SetUpController(Shooter, typeof(TankShooter));

            // Visuals
            SetUpController(Visuals, typeof(TankVisuals));

            // Stats
            SetUpController(Health, typeof(BossHealth));

            // AI
            SetUpController(AIController, typeof(BossAIController));
            SetUpController(AttackController, typeof(BossAttackController));
        }

        public override void Activate()
        {
            TankBodyParts.Activate();
            Health.Activate();
            Visuals.Activate();
            AIController.Activate();
            Movement.Activate();
            AttackController.Activate();
            Health.Activate();

            _minimapIcon.SpawnIcon();
        }

        public override void Deactivate()
        {
            TankBodyParts.Deactivate();
            Health.Deactivate();
            Visuals.Deactivate();
            AIController.Deactivate();
            Movement.Deactivate();
            AttackController.Deactivate();
            Health.Deactivate();
        }

        public override void Restart()
        {
            TankBodyParts.Restart();
            Health.Restart();
            Visuals.Restart();
            AIController.Restart();
            Movement.Restart();
            AttackController.Restart();
        }

        public override void Dispose()
        {
            TankBodyParts.Dispose();
            Health.Dispose();
            Visuals.Dispose();
            AIController.Dispose();
            Movement.Dispose();
            AttackController.Dispose();
        }
        #endregion
    }
}
