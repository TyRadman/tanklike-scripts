using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using UnitControllers;
    using Utils;
    using Combat.SkillTree;
    using Combat.SkillTree.Upgrades;
    using TankLike.Sound;

    [CreateAssetMenu(fileName = PREFIX + "Brock", menuName = ROOT_PATH + "Ability")]
    public class Brock : Ability
    {
        public const string ROOT_PATH = Directories.ABILITIES + "Brock Ability/";

        [Header("Special Values")]
        [SerializeField] private List<AnimationCurve> _verticalCurves;
        [SerializeField] private List<AnimationCurve> _horizontalCurves;
        [SerializeField] private AmmunationData _bulletData;
        [SerializeField] private AnimationCurve _forwardCurve;
        [SerializeField] private float _verticalDistance = 15f;
        [SerializeField] private float _horizontalMaxOffset = 2f;
        [field: SerializeField] public int DamagePerBullet { get; private set; } = 15;
        [field: SerializeField] public float Radius { get; private set; } = 2.5f;
        [field: SerializeField] public int ProjectilesCount { get; private set; } = 10;

        [Header("Audio")]
        [SerializeField] private Audio _onShotAudio;

        [Header("Durations")]
        public Vector2 AimRange = new Vector2(2f, 10f);
        [SerializeField] private float _abilityDuration = 2f;
        [SerializeField] private float _airMovementDuration = 0.3f;
        [SerializeField] private float _landingDuration = 0.3f;
        [SerializeField] private float _inAirDelayDuration = 0.3f;

        [Tooltip("Speed multiplier. How fast should the cursor move in the super aim state in comparison to its normal speed.")]
        [SerializeField] private float _aimSpeed = 0.5f;

        [Header("Special Animations")]
        [SerializeField] private PartAnimationReference _onHoldStartAnimation;
        [SerializeField] private PartAnimationReference _onFinishedAnimation;
        [SerializeField] private PartAnimationReference _onShootAnimation;

        private Transform[] _shootingPoints;
        private TankComponents _tankComponents;
        private PlayerCrosshairController _crosshair;
        private BulletData _projectileData;
        private float _timeBetweenShots;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _tankComponents = components;
            _crosshair = ((PlayerComponents)_tankComponents).CrosshairController;
            _timeBetweenShots = ((_abilityDuration - _airMovementDuration - _landingDuration - _inAirDelayDuration) / ProjectilesCount) / 2;

            // get shooting points. If there are brock shooting points in the additional tank info class, then set them as shooting points, otherwise, set the main shooting point
            if (components.AdditionalInfo is VanguardAdditionalInfo additionalInfo)
            {
                if(additionalInfo.BrockShootingPoints.Length > 0)
                {
                    _shootingPoints = additionalInfo.BrockShootingPoints;
                }
                else
                {
                    _shootingPoints = _tankComponents.Shooter.GetShootingPoints().ToArray(); //need to be tested
                }
            }
            else
            {
                _shootingPoints = _tankComponents.Shooter.GetShootingPoints().ToArray(); //need to be tested
            }

            _projectileData = new BulletData()
            {
                Speed = 0f,
                Damage = DamagePerBullet,
                MaxDistance = 0f,
                CanBeDeflected = false
            };
        }

        private void UpdateCurve()
        {
            float shootingPointHeight = _shootingPoints[0].position.y;

            float groundProgressValue = shootingPointHeight / (_verticalDistance);

            for (int i = 0; i < _verticalCurves.Count; i++)
            {
                AnimationCurve curve = _verticalCurves[i];

                Keyframe[] keys = curve.keys;

                Keyframe key = keys[^1];

                key.value = -groundProgressValue;
                key.time = 1f;

                keys[^1] = key;

                curve.keys = keys;

                curve.keys[^1] = key;
            }
        }

        public override void PopulateSkillProperties()
        {
            base.PopulateSkillProperties();

            SkillProperties projectileCount = new SkillProperties()
            {
                Name = "Projectiles Count",
                Value = ProjectilesCount.ToString(),
                DisplayColor = Colors.Red
            };

            SkillDisplayProperties.Add(projectileCount);

            SkillProperties damagePerProjectile = new SkillProperties()
            {
                Name = "Damage per Projectile",
                Value = _bulletData.Ammunition.Damage.ToString(),
                DisplayColor = Colors.Red,
                UnitString = PropertyUnits.POINTS
            };

            SkillDisplayProperties.Add(damagePerProjectile);
        }

        public override void SetUpIndicatorSpecialValues(AirIndicator indicator)
        {
            if (indicator == null)
            {
                Debug.LogError($"No indicator passed to ability {name}");
                Debug.Break();
                return;
            }

            IndicatorSettings settings = new IndicatorSettings()
            {
                AimRange = AimRange,
                EndSize = Radius,
                AimSpeedMultiplier = _aimSpeed,
                LineHeight = _verticalDistance + 1f,
                AvoidWalls = false,
                IsCrosshairTheParent = true,
                LineState = AirIndicator.IndicatorLineState.AirTrajectory 
            };

            indicator.SetValues(settings);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();
            GameManager.Instance.StartCoroutine(BrockAbilityRoutine());
        }

        private IEnumerator BrockAbilityRoutine()
        {
            List<Bullet> bullets = new List<Bullet>();
            Vector3 crosshairPosition = _crosshair.GetCrosshairTransform().position;
            TankAlignment targetAlignment = Helper.GetOpposingTag(_tankComponents.Alignment);

            _components.SpecialPartsAnimation.PlaySpecialPartAnimation(_onShootAnimation);
            UpdateCurve();

            for (int i = 0; i < ProjectilesCount; i++)
            {
                // create the bullet
                Bullet bullet = GameManager.Instance.VisualEffectsManager.Bullets.GetBullet(_bulletData.GUID);
                bullet.SetValues(_projectileData);

                Vector3 position = _shootingPoints[i % _shootingPoints.Length].position;
                Quaternion rotation = _shootingPoints[i % _shootingPoints.Length].rotation;
                bullet.transform.SetPositionAndRotation(position, rotation);

                bullet.gameObject.SetActive(true);
                bullets.Add(bullet);
                Vector2 randomCircle = Radius * Random.insideUnitCircle;
                Vector3 point = crosshairPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
                // enable the bullet's collider, mesh, and trail
                bullet.EnableBullet();
                bullet.SetActive(true);

                _tankComponents.StartCoroutine(MoveBulletToPoint(bullet.transform, point, _airMovementDuration));

                // set reference to the bullet data
                bullet.SetUpBulletdata(_bulletData);
                // give the bullet a target tag
                bullet.SetTargetLayerMask(targetAlignment);
                // assign the shooter of the bullet
                bullet.SetShooter(_tankComponents);

                // Play SFX
                GameManager.Instance.AudioManager.Play(_onShotAudio);

                yield return new WaitForSeconds(_timeBetweenShots);
            }

            yield return new WaitForSeconds(_airMovementDuration);
            OnAbilityFinished();
        }

        private IEnumerator MoveBulletToPoint(Transform bullet, Vector3 point, float duration)
        {
            Vector3 startPos = bullet.position;
            float time = 0f;
            // catch the direction of the bullet's movement
            AnimationCurve verticalCurve = _verticalCurves.RandomItem();
            AnimationCurve horizontalCurve = _horizontalCurves.RandomItem();
            Vector3 previousPosition = bullet.position; // Store the initial position

            // Smoothly interpolate the rotation over time.
            while (time < duration && bullet.gameObject.activeSelf)
            {
                float t = time / duration;
                Vector3 newPosition = Vector3.Lerp(startPos, point, _forwardCurve.Evaluate(t));
                //newPosition += _horizontalMaxOffset * horizontalCurve.Evaluate(t) * bullet.right;
                newPosition += _verticalDistance * verticalCurve.Evaluate(t) * Vector3.up;

                // Calculate movement direction
                Vector3 direction = newPosition - previousPosition;

                // Update position
                bullet.position = newPosition;

                // Update rotation smoothly
                if (direction.sqrMagnitude > 0.001f) // Avoid jitter for tiny movements
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    bullet.rotation = Quaternion.Lerp(bullet.rotation, targetRotation, Time.deltaTime * Bullet.ROTATION_SPEED); // Smooth rotation
                }

                // Update previous position
                previousPosition = newPosition;

                // Increment time
                time += Time.deltaTime;

                yield return null;
            }

            time = 0f;

            //// to ensure it lands on the ground
            while (time < 1f)
            {
                bullet.position -= 10f * Time.deltaTime * Vector3.up;

                time += Time.deltaTime;

                yield return null;
            }
        }

        public override float GetDuration()
        {
            return _timeBetweenShots * ProjectilesCount + _airMovementDuration;
        }

        public override void Upgrade(SkillUpgrade upgrade)
        {
            base.Upgrade(upgrade);

            if(upgrade is not BrockUpgrades newUpgrades)
            {
                Debug.LogError($"Upgrade passed type doesn't match the expected type: {nameof(BrockUpgrades)}");
                return;
            }

            Radius += newUpgrades.CoverageArea;

            ProjectilesCount += newUpgrades.ShotsCount;

            AimRange.y += newUpgrades.Distance;

            DamagePerBullet += newUpgrades.DamagePerProjectile;
            _projectileData.Damage = DamagePerBullet;
        }

        public override void OnAbilityHoldStart()
        {
            _components.SpecialPartsAnimation.PlaySpecialPartAnimation(_onHoldStartAnimation);
        }

        public override void OnAbilityHoldUpdate()
        {

        }

        public override void OnAbilityFinished()
        {

        }

        public override void OnAbilityInterrupted()
        {
            _components.SpecialPartsAnimation.PlaySpecialPartAnimation(_onFinishedAnimation);
        }

        public override void Dispose()
        {

        }
    }
}
