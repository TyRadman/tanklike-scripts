using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyComponents : TankComponents, IPoolable
    {
        public Action<EnemyComponents> OnEnemyDeath { get; set; }
        public Action<IPoolable> OnReleaseToPool { get; private set; }

        [field: SerializeField] public EnemyAIController AIController { get; private set; }
        [field: SerializeField] public EnemyItemDropper ItemDrop { get; private set; }
        [field: SerializeField] public EnemyDifficultyModifier DifficultyModifier { get; private set; }

        [Header("Debug")]
        [SerializeField] private bool _movementDebug;

        private void Start()
        {
            if (_movementDebug)
            {
                SetUp();

                // Restart necessary components
                Movement.Restart();
                TurretController?.Restart();

                // Set debug mode
                ((EnemyMovement)Movement).SetDebugMode(_movementDebug);
                ((EnemyTurretController)TurretController)?.SetDebugMode(_movementDebug);

                // Activate necessary components
                Movement.Activate();
                TurretController?.Activate();
            }
        }

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

        public override void PositionUnit(Transform point)
        {
            Vector3 position = point.position;
            position.y = 1f;
            Quaternion rotation = Quaternion.LookRotation(point.forward);
            transform.SetPositionAndRotation(position, Quaternion.identity);
            Movement.SetBodyRotation(rotation);
        }

        #region IController
        public override void SetUp(IController controller = null)
        {
            // Body Parts
            AddComponentToList(TankBodyParts);

            // Movement
            AddComponentToList(Movement);
            AddComponentToList(TurretController);

            // Combat
            AddComponentToList(Shooter);
            AddComponentToList(ElementsEffects);

            // Visuals
            AddComponentToList(Animation);
            AddComponentToList(Visuals);
            AddComponentToList(TankWiggler);
            AddComponentToList(VisualEffects);

            // Stats
            AddComponentToList(Health);

            // AI
            AddComponentToList(AIController);
            AddComponentToList(ItemDrop);
            AddComponentToList(DifficultyModifier);

            AddComponentToList(KnockBackController);

            _components.ForEach(c => c.SetUp(this));
        }

        public override void Activate()
        {
            _components.ForEach(c => c.Activate());

            if (_minimapIcon != null)
            {
                _minimapIcon.SpawnIcon();
            }
            else
            {
                //Debug.LogError($"No minimap on {gameObject.name}");
            }

            // TODO: check this when working on elements
            //ElementsEffects.Activate();
        }

        public override void Deactivate()
        {
            _components.ForEach(c => c.Deactivate());
            // TODO: check this when working on elements
            //ElementsEffects.Deactivate();
        }

        public override void Restart()
        {
            _components.ForEach(c => c.Restart());
            // TODO: check this when working on elements
            //ElementsEffects.Restart();
        }

        public override void Dispose()
        {
            _components.ForEach(c => c.Dispose());

            OnEnemyDeath?.Invoke(this);
            TurnOff();
            // TODO: check this when working on elements
            //ElementsEffects.Dispose();
        }
        #endregion

        #region Pool
        public void Init(Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }

        public void OnRequest()
        {
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
