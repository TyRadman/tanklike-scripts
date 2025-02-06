using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;
    using static PlayersManager;
    
    public enum ThreeCannonAttackType
    {
        MainMachineGun,
        SideMachineGuns,
        GroundPound,
        RocketLauncher,
    }

    public class BossAttackController : MonoBehaviour, IController
    {
        public Action OnAttackFinished;
        public Action OnGroundPoundTriggered;
        public Action OnGroundPoundTriggerStay;
        public Action OnGroundPoundImpact;

        [field: SerializeField, Header("Machine Gun Attack")] public Transform MachineGunShootingPoint { get; private set; }
        [field: SerializeField] public Transform MachineGunTransform { get; private set; }

        [field: SerializeField, Header("Side Machine Guns Attack")] public Transform RightMachineGunShootingPoint { get; private set; }
        [field: SerializeField] public Transform LeftMachineGunShootingPoint { get; private set; }
        [field: SerializeField] public Transform RightMachineGunTransform { get; private set; }
        [field: SerializeField] public Transform LeftMachineGunTransform { get; private set; }
        [field: SerializeField] public Transform RightTurretTransform { get; private set; }
        [field: SerializeField] public Transform LeftTurretTransform { get; private set; }

        [field: SerializeField, Header("Ground Pound")] public ParticleSystem GroundPoundParticles { get; private set; }
        [field: SerializeField] public CollisionEventPublisher GroundPoundTrigger { get; private set; }
        [field: SerializeField] public Vector2 GroundPoundTriggerDurationRange { get; private set; }

        [field: SerializeField, Header("Rocket Launcher Attack")] public Transform RocketLauncherShootingPoint { get; private set; }

        public bool IsActive { get; private set; }

        private BossComponents _bossComponents;
        private EnemyShooter _shooter;
        private ThreeCannonBossAnimations _animations;
        private PlayerTransforms _currentTarget;
        private Coroutine _attackCoroutine;
        private Coroutine _machineGunAnimationCoroutine;
        private Coroutine _sideMachineGunsAnimationCoroutine;
        private Vector3 _roomCenter;
        private Vector3 _roomSize;
        private List<IPoolable> _activePoolables = new List<IPoolable>();
        private float _groundPoundTimer;
        private float _groundPoundCurrentTriggerDuration;
        private bool _groundPoundTriggerStarted;
        private Coroutine _groundPoundTriggerCoroutine;

        public void SetUp(IController controller)
        {
            BossComponents components = controller as BossComponents;

            if (components == null)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _bossComponents = components;

            _roomCenter = _bossComponents.RoomCenter;
            _roomSize = _bossComponents.RoomSize;
            _shooter = (EnemyShooter)_bossComponents.Shooter;
            _animations = (ThreeCannonBossAnimations)_bossComponents.Animations;

            //_groundPoundWeapon.SetTargetLayer(_groundPoundTargetLayers);         
        }

        public void SetTarget(PlayerTransforms target)
        {
            _currentTarget = target;
        }

        public void UnsetTarget()
        {
            _currentTarget = null;
        }

        public void Attack(ThreeCannonAttackType attackType, Action OnAttackCompleted)
        {
            OnAttackFinished = OnAttackCompleted;
        }

        public void AddToActivePooables(IPoolable poolable)
        {
            if(!_activePoolables.Contains(poolable))
            {
                _activePoolables.Add(poolable);
            }
        }

        public void RemoveFromActivePooables(IPoolable poolable)
        {
            if (_activePoolables.Contains(poolable))
            {
                _activePoolables.Remove(poolable);
            }
        }

        #region Main Machine Gun Attack
        public void SetMachineGunValues(float shootingDuration)
        {
            //_machineGunShootingDuration = shootingDuration;
        }
        #endregion

        #region Side Machine Guns Attack
        public void SetSideMachineGunsValues(float shootingDuration)
        {
            //_sideMachineGunsShootingDuration = shootingDuration;
        }
        #endregion

        #region Ground Pound Attack
        public void EnableGroundPoundTrigger(bool value)
        {
            GroundPoundTrigger.EnableCollider(value);
        }

        private void OnGroundPoundTriggerStayHandler(Collider target)
        {
            if (target.gameObject.CompareTag("Player"))
            {
                //Debug.Log("Player detected in ground pound trigger");
                OnGroundPoundTriggerStay?.Invoke();
            }
        }

        private void GroundPoundImpactHandler()
        {
            OnGroundPoundImpact?.Invoke();
        }
        #endregion

        #region Rocket Launcher Attack
      
        #endregion

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
            GroundPoundTrigger.OnTriggerStayEvent += OnGroundPoundTriggerStayHandler;

            EnableGroundPoundTrigger(false);

            var pubs = _animations.Animator.GetBehaviours<AnimatorEventPublisher>();

            foreach (AnimatorEventPublisher publisher in pubs)
            {
                if (publisher.StateName == "Ground Pound")
                {
                    publisher.OnSpecialEvent += GroundPoundImpactHandler;
                }
            }

            _groundPoundTimer = 0f;
        }

        public void Dispose()
        {
            //Stop all the coroutines
            StopAllCoroutines();

            //Unsubscribe to all events
            GroundPoundTrigger.OnTriggerStayEvent -= OnGroundPoundTriggerStayHandler;

            var pubs = _animations.Animator.GetBehaviours<AnimatorEventPublisher>();

            foreach (AnimatorEventPublisher publisher in pubs)
            {
                if (publisher.StateName == "Ground Pound")
                {
                    publisher.OnSpecialEvent -= GroundPoundImpactHandler;
                }
            }

            // Clear all poolables
            if (_activePoolables.Count > 0)
            {
                _activePoolables.ForEach(e => e.TurnOff());
            }

            _activePoolables.Clear();
        }
        #endregion
    }
}
