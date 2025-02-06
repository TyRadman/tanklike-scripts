using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    using Combat;
    using Misc;
    using Utils;
    using static PlayersManager;

    [CreateAssetMenu(fileName = "State_ThreeCannon_RocketLauncherAttack", menuName = MENU_PATH + "Three Cannon/Rocket Launcher Attack State")]
    public class ThreeCannonBossRocketLauncherState : BossAttackState
    {
        [Header("Rocket Launcher Attack")]
        [SerializeField] private NormalShot _rocketLauncherWeapon;
        [SerializeField] private IndicatorEffects.IndicatorType _indicatorType;
        [SerializeField] private int _rocketLauncherProjectilesPerAttack;
        [SerializeField] private float _rocketLauncherTimeBetweenProjectiles = 0.25f;
        [SerializeField] private float _rocketImpactRadius = 5;
        [SerializeField] private int _rocketLauncherRandomProjectilesCount = 5;
        [SerializeField] private float _rocketLauncherRandomProjectilesGap = 4;
        [SerializeField] private float _rocketLauncherProjectileInAirDuration;
        [SerializeField] private float _rocketLauncherProjectileMaxHeightReach = 10;
        [SerializeField] private float _rocketLauncherProjectileIndicatorHeight = 0.16f;
        [SerializeField] protected LayerMask _rocketLauncherTargetLayers;

        private Vector3 _targetPosition;
        private Transform _targetTransform;
        private PlayerTransforms _currentTarget;
        private Coroutine _attackCoroutine;
        private BulletData _bulletData;
        private bool _followTargetTransform;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);
            _movement.OnTargetFaced += OnTargetFacedHandler;

            _bulletData = new BulletData()
            {
                Speed = _rocketLauncherWeapon.BulletSpeed,
                Damage = _rocketLauncherWeapon.Damage,
                MaxDistance = _rocketLauncherWeapon.MaxDistance,
                CanBeDeflected = _rocketLauncherWeapon.CanBeDeflected
            };
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _isActive = true;

            _followTargetTransform = false;

            // Chance to attack a random target vs farthest target
            float rand = Random.Range(0f, 1f);

            if (rand <= _randomTargetChance)
            {
                List<PlayerTransforms> alivePlayers = GameManager.Instance.PlayersManager.GetPlayerTransforms();
                PlayerTransforms targetPlayerTransform = alivePlayers[Random.Range(0, alivePlayers.Count)];
                _attackController.SetTarget(targetPlayerTransform);
                _currentTarget = targetPlayerTransform;
                _targetTransform = targetPlayerTransform.PlayerTransform;
                _targetPosition = targetPlayerTransform.PlayerTransform.position;
            }
            else
            {
                PlayerTransforms targetPlayerTransform = GameManager.Instance.PlayersManager.GetFarthestPlayer(_movement.transform.position);
                _attackController.SetTarget(targetPlayerTransform);
                _currentTarget = targetPlayerTransform;
                _targetTransform = targetPlayerTransform.PlayerTransform;
                _targetPosition = targetPlayerTransform.PlayerTransform.position;
            }

            _movement.ResetTargetIsFaced();
        }

        public override void OnUpdate()
        {
            //Debug.Log("Update rocket launcher");
            if (_followTargetTransform)
            {
                _movement.FaceTarget(_targetTransform);
            }
            else
            {
                _movement.FaceTarget(_targetPosition);
            }
        }

        public override void OnExit()
        {
            _isActive = false;
            _followTargetTransform = false;

            if (_attackCoroutine != null)
            {
                _attackController.StopCoroutine(_attackCoroutine);
            }
        }

        public override void OnDispose()
        {
        }

        private void OnTargetFacedHandler()
        {
            if (!_isActive)
            {
                return;
            }

            _followTargetTransform = true;

            _attackCoroutine = _attackController.StartCoroutine(RocketLauncherAttackRoutine());
        }

        private void OnAttackFinished()
        {
            if (!_isActive)
            {
                return;
            }

            _stateMachine.ChangeState(BossStateType.Move);
        }

        #region Attack Methods
        private IEnumerator RocketLauncherAttackRoutine()
        {
            for (int i = 0; i < _rocketLauncherProjectilesPerAttack; i++)
            {
                //Play animation
               ((ThreeCannonBossAnimations)_animations).TriggerRocketLauncherAnimation();

                Vector3 newPoint = _currentTarget.ImageTransform.position;
                newPoint.y = 0.16f; // dirty, have a variable for it

                LaunchRocket(newPoint);

                if (i % _rocketLauncherRandomProjectilesGap == 0 && i > 0)
                {
                    _attackController.StartCoroutine(ShootRocketsAtRandomPointsRoutine(_rocketLauncherRandomProjectilesCount));
                }

                yield return new WaitForSeconds(_rocketLauncherTimeBetweenProjectiles);
            }

            OnAttackFinished();
        }

        private void LaunchRocket(Vector3 targetPoint)
        {
            Transform shootingPoint = _attackController.RocketLauncherShootingPoint;

            //Muzzle flash effect
            ParticleSystemHandler muzzleEffect = GameManager.Instance.VisualEffectsManager.Bullets.GetMuzzleFlash(_rocketLauncherWeapon.BulletData.GUID);
            muzzleEffect.transform.SetPositionAndRotation(shootingPoint.position, shootingPoint.rotation);
            muzzleEffect.gameObject.SetActive(true);
            muzzleEffect.Play();

            //Spawn bullets
            Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(_rocketLauncherWeapon.BulletData.GUID);
            bullet.transform.SetPositionAndRotation(shootingPoint.position, shootingPoint.rotation);
            bullet.gameObject.SetActive(true);
            // enable the bullet's collider, mesh, and trail
            bullet.EnableBullet();
            bullet.SetActive(true);
            bullet.SetTargetLayerMask(_rocketLauncherTargetLayers);
            bullet.SetValues(_bulletData);
            // set reference to the bullet data
            bullet.SetUpBulletdata(_rocketLauncherWeapon.BulletData);
            // assign the shooter of the bullet
            bullet.SetShooter(_components);
            // move bullet along a spline
            bullet.MoveToPointAlongSpline(targetPoint, _rocketLauncherProjectileMaxHeightReach, _rocketLauncherProjectileInAirDuration);
            GameManager.Instance.AudioManager.Play(_rocketLauncherWeapon.ShotAudio);

            //Spawn indicators
            AreaOfEffectImpact AOEImpact = bullet.Impact as AreaOfEffectImpact;
            float AOERadius = AOEImpact.AreaRadius * 2f;
            Vector3 indicatorSize = new Vector3(AOERadius, 1f, AOERadius);

            Indicator indicator = GameManager.Instance.VisualEffectsManager.Indicators.GetIndicatorByType(_indicatorType);
            indicator.gameObject.SetActive(true);
            indicator.transform.SetPositionAndRotation(targetPoint, Quaternion.identity);
            indicator.transform.localScale = indicatorSize;
            indicator.Play(_rocketLauncherProjectileInAirDuration);

            bullet.SetTargetIndicator(indicator);
        }


        private IEnumerator ShootRocketsAtRandomPointsRoutine(int pointsCount)
        {
            Vector3[] targetPoints = new Vector3[pointsCount];

            CalculatePositions(_rocketImpactRadius, targetPoints);

            for (int i = 0; i < targetPoints.Length; i++)
            {
                LaunchRocket(targetPoints[i]);
                yield return new WaitForSeconds(0.05f);
            }
        }

        private void CalculatePositions(float impactRange, Vector3[] points)
        {
            int totalPoints = points.Length;

            if (totalPoints == 0)
            {
                return;
            }

            for (int i = 0; i < totalPoints; i++)
            {
                int count = 0;

                while (true)
                {
                    Vector3 newPoint = Helper.GetRandomPointInsideSphere(_currentTarget.ImageTransform.position, 10);
                    newPoint.y = _rocketLauncherProjectileIndicatorHeight;

                    points[i] = newPoint;

                    if (!PointIsOverlapping(newPoint, i, impactRange, points))
                    {
                        points[i] = newPoint;
                        break;
                    }

                    count++;

                    if (count > _rocketLauncherProjectilesPerAttack)
                    {
                        Debug.Log("BAD POINT ADDED!");
                        points[i] = newPoint;
                        break;
                    }
                }
            }
        }

        bool PointIsOverlapping(Vector3 point, int maxIndex, float explosionRange, Vector3[] points)
        {
            for (int i = 1; i < maxIndex; i++)
            {
                if (Vector3.SqrMagnitude(point - points[i]) < Mathf.Pow(explosionRange, 2f))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
