using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.AirDrone
{
    using UI.InGame;
    using UnitControllers;

    public class AirDroneController : MonoBehaviour
    {
        [SerializeField] private Transform _body;
        [SerializeField][Range(0f, 1f)] private float _followSpeed = 0.2f;
        [SerializeField][Range(0f, 1f)] private float _rotationFollowSpeed = 0.2f;
        [SerializeField] private float _enemyDetectionRadius = 10f;
        [SerializeField] private float _scanningForEnemiesFrequency = 1.5f;
        private LayerMask _targetsLayerMask;
        private TankComponents _target;
        private bool _isLockedToTarget = false;
        private Transform _owner;
        [SerializeField] private AirDroneShooter _shooter;
        [SerializeField] private AirDroneAnimation _animation;
        private WaitForSeconds _shootingWait;
        private Coroutine _shootingCoroutine;
        private Coroutine _movingCoroutine;
        private Coroutine _movingToPlayerCoroutine;
        private TankShooter _tankShooter;
        private int _shots;
        private int _maxShots;
        public bool IsActive { get; private set; } = false;
        [SerializeField] private SegmentedBar _bar;

        public void SetUp(TankComponents player, LayerMask targetsMask, float shootingCooldown)
        {
            _shootingWait = new WaitForSeconds(shootingCooldown);
            _owner = player.transform;
            _tankShooter = player.GetComponent<TankShooter>();
            _targetsLayerMask = targetsMask;
            _shooter.SetUp(player);
            _bar.SetUp();
        }

        public void SetShots(int shots)
        {
            _shots = shots;
            _maxShots = shots;
            _bar.SetCount(_shots);
            _bar.SetTotalAmount(1);
        }

        public void StartDrone()
        {
            gameObject.SetActive(true);
            StartCoroutine(StartDroneProcess());
        }

        private IEnumerator StartDroneProcess()
        {
            // play animation
            _animation.PlaySpawnAnimation();

            yield return new WaitForSeconds(_animation.GetSpawnAnimationLength());
            
            IsActive = true;
            _tankShooter.OnShootStarted += Shoot;
            _isLockedToTarget = false;
            Invoke(nameof(LookForTarget), _scanningForEnemiesFrequency);
            _animation.LiftDrone();

            if (_movingToPlayerCoroutine != null) StopCoroutine(_movingToPlayerCoroutine);

            _movingToPlayerCoroutine = StartCoroutine(FollowingPlayerProcess());
        }

        private void LookForTarget()
        {
            Collider[] targets = Physics.OverlapSphere(_owner.position, _enemyDetectionRadius, _targetsLayerMask);

            // if there are targets, then lock to the closest one and set it as a target. Otherwise, repeat the scan in a given time
            if (targets.Length > 0)
            {
                _target = targets[0].GetComponent<TankComponents>();
                // set an event for when the target dies
                _target.Health.OnDeath += OnTankDeath;
                // lock the drone to the target
                LockToTarget(_target.transform);
            }
            else
            {
                Invoke(nameof(LookForTarget), _scanningForEnemiesFrequency);
            }
        }

        public void LockToTarget(Transform target)
        {
            _isLockedToTarget = true;

            if (_shootingCoroutine != null) StopCoroutine(_shootingCoroutine);

            if (_movingCoroutine != null) StopCoroutine(_movingCoroutine);

            _movingCoroutine = StartCoroutine(FollowingProcess(target));
        }

        private IEnumerator FollowingProcess(Transform target)
        {
            while (_isLockedToTarget)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, _followSpeed);
                Vector3 direction = target.position - transform.position;
                direction.y = 0f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, transform.up), _rotationFollowSpeed);

                yield return null;
            }
        }

        private IEnumerator FollowingPlayerProcess()
        {
            while (!_isLockedToTarget)
            {
                transform.position = Vector3.Lerp(transform.position, _owner.position, _followSpeed);
                Vector3 direction = _owner.position - transform.position;
                direction.y = 0f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, transform.up), _rotationFollowSpeed);

                yield return null;
            }
        }

        public void Shoot()
        {
            _shooter.Shoot();
            _shots--;
            _bar.AddAmountToSegments(-1);

            // if there are no more shots, then we stop the drone, otherwise, we wait and then shoot again
            if (_shots <= 0)
            {
                DisableDrone();
            }
        }

        private void DisableDrone()
        {
            _tankShooter.OnShootStarted -= Shoot;
            IsActive = false;

            if (_target != null) _target.Health.OnDeath -= OnTankDeath;

            _animation.PlayDespawnAnimation();
            _isLockedToTarget = false;
            Invoke(nameof(DisableDroneObject), _animation.GetSpawnAnimationLength());
        }

        private void DisableDroneObject()
        {
            gameObject.SetActive(false);
        }

        private void OnTankDeath(TankComponents tank)
        {
            print($"Done with {_target.name}");
            // stop aiming at the current target
            _isLockedToTarget = false;
            // unsubscribe the tank's listener
            _target.Health.OnDeath -= OnTankDeath;

            // resume following the player
            if (_movingToPlayerCoroutine != null) StopCoroutine(_movingToPlayerCoroutine);

            _movingToPlayerCoroutine = StartCoroutine(FollowingPlayerProcess());
            // look for a new target
            Invoke(nameof(LookForTarget), _scanningForEnemiesFrequency);
        }
    }
}
