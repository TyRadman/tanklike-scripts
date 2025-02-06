using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using Cam;
    using TankLike.Combat.SkillTree.Upgrades;
    using UnitControllers;

    [CreateAssetMenu(fileName = PREFIX + "ExplosiveTeleportation", menuName = ROOT_PATH + "ability")]
    public class ExplosiveTeleportationAbility : Ability
    {
        public const string ROOT_PATH = Directories.ABILITIES + "Explosive Teleportation/";

        [System.Flags]
        public enum TeleportationExplosionMode
        {
            [InspectorName("Explode on start")]
            OnStart = 1,
            [InspectorName("Explode at the end")]
            OnLand = 2,
            [InspectorName("Explode multiple times along the way")]
            AlongWay = 4,
            [InspectorName("No explosions")]
            None = 8
        }

        [field: SerializeField, Header("Special Values")] public float ShrinkDuration { get; private set; } = 0.25f;
        [field: SerializeField] public float TeleportDistance { get; private set; } = 8f;
        [SerializeField] private LayerMask _obstacleLayers;
        private Vector2 _indicatorRange = new Vector2(3f, 5f);

        [Header("Explosion")]
        [SerializeField] private AOEWeapon _weapon;
        [field: SerializeField] public TeleportationExplosionMode ExplosionMode { get; private set; }
        [SerializeField] private float _alongTheWayExplosionMinDistance;
        [SerializeField] private float _timeBetweenExplosions = 0.2f;

        [Header("Debug")]
        [SerializeField] private bool _debug;

        private Transform _testingSphere;
        private Testing.Playground.PlaygroundManager _playgroundManager;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            _weapon.SetUp(components);
            _indicatorRange.y = TeleportDistance;

            if (_debug)
            {
                if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "S_Playground")
                {
                    _debug = false;
                    return;
                }

                _playgroundManager = FindObjectOfType<Testing.Playground.PlaygroundManager>();
                _testingSphere = _playgroundManager.TestingSphere;
            }
        }

        public override void PopulateSkillProperties()
        {
            base.PopulateSkillProperties();

            AddSkillProperty("Explosion range", _weapon.ExplosionRadius, Colors.LightOrange, PropertyUnits.BLOCKS);
            AddSkillProperty("Teleport distance", TeleportDistance, Colors.Green, PropertyUnits.BLOCKS);
            AddSkillProperty("Damage per explosion", _weapon.Damage, Colors.Red, PropertyUnits.POINTS);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();

            _components.StartCoroutine(TeleportRoutine());
        }

        private IEnumerator TeleportRoutine()
        {
            Vector3 startPoint = _components.transform.position;
            Vector3 targetPoint = GetTargetPoint();

            _components.Deactivate();

            Vector3 initialScale = Vector3.one;
            float elapsedTime = 0f;

            // Scale down
            while (elapsedTime < ShrinkDuration)
            {
                float scaleMultiplier = Mathf.Lerp(1f, 0f, elapsedTime / ShrinkDuration);
                _components.transform.localScale = initialScale * scaleMultiplier;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Teleport
            _components.transform.position = targetPoint;

            // Deal damage on start
            if (ExplosionMode.HasFlag(TeleportationExplosionMode.OnStart))
            {
                _weapon.OnShot(startPoint);
                PlayExplosionEffects(startPoint);
            }

            if (ExplosionMode.HasFlag(TeleportationExplosionMode.AlongWay))
            {
                _components.StartCoroutine(ExplosionTrailRoutine(startPoint, targetPoint));
            }

            // Deal damage on landing
            if (ExplosionMode.HasFlag(TeleportationExplosionMode.OnLand))
            {
                _weapon.OnShot(targetPoint);
                PlayExplosionEffects(targetPoint);
            }

            // Scale up
            elapsedTime = 0f;

            bool explode = false;

            while (elapsedTime < ShrinkDuration)
            {
                float scaleMultiplier = Mathf.Lerp(0f, 1f, elapsedTime / ShrinkDuration);
                _components.transform.localScale = initialScale * scaleMultiplier;
                elapsedTime += Time.deltaTime;

                if(elapsedTime / ShrinkDuration >= 0.5f && !explode)
                {
                    explode = true;
                }

                yield return null;
            }

            _components.Activate();
        }

        private IEnumerator ExplosionTrailRoutine(Vector3 startPoint, Vector3 targetPoint)
        {
            List<Vector3> points =  GetEquallySpacedPoints(startPoint, targetPoint, _alongTheWayExplosionMinDistance);

            foreach (Vector3 point in points)
            {
                _weapon.OnShot(point);
                PlayExplosionEffects(point);

                // Wait for the specified interval before spawning the next object
                yield return new WaitForSeconds(_timeBetweenExplosions);
            }
        }

        public List<Vector3> GetEquallySpacedPoints(Vector3 startPoint, Vector3 endPoint, float minDistance)
        {
            // Create a list to store the points
            List<Vector3> points = new List<Vector3>();

            // Calculate the distance between the two points
            float distance = Vector3.Distance(startPoint, endPoint);

            // Check if the distance is greater than the threshold
            if (distance > minDistance)
            {
                // Add start point
                points.Add(startPoint);

                // Calculate the number of threshold intervals in the distance
                int numberOfPoints = Mathf.FloorToInt(distance / minDistance);

                // Calculate step fraction for equal spacing
                float stepFraction = 1.0f / (numberOfPoints + 1);  // +1 to exclude start and end points

                // Generate points based on multiples of the threshold
                for (int i = 1; i <= numberOfPoints; i++)
                {
                    // Interpolate between startPoint and endPoint using Lerp
                    Vector3 point = Vector3.Lerp(startPoint, endPoint, stepFraction * i);
                    points.Add(point);
                }
            }

            // Add end point
            points.Add(endPoint);

            return points;
        }

        private Vector3 GetTargetPoint()
        {
            Vector3 targetPoint;

            // Get the forward direction of the turret
            Vector3 direction = _components.Shooter.ShootingPoints[0].forward;

            float sphereRadius = _components.CharacterController.radius;
            float checkRadius = sphereRadius + Constants.OffsetToWalls;

            Vector3 castStartPoint = _components.transform.position;
            targetPoint = _components.transform.position + direction * TeleportDistance;

            // Cast a ray in the forward direction to check for walls
            RaycastHit[] hits = Physics.SphereCastAll(castStartPoint, sphereRadius - 0.1f, direction, TeleportDistance + sphereRadius + Constants.OffsetToWalls, _obstacleLayers);

            // Sort the hits by distance along the ray direction
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            if (hits.Length == 1)
            {
                // Now check for obstacles at the target point using an overlap sphere
                Collider[] hitColliders = Physics.OverlapSphere(targetPoint, checkRadius, _obstacleLayers);

                if (hitColliders.Length > 0)
                {
                    targetPoint = hits[0].point - checkRadius * direction;
                }
            }
            else if(hits.Length > 1)
            {
                // Now check for obstacles at the target point using an overlap sphere
                Collider[] hitColliders = Physics.OverlapSphere(targetPoint, checkRadius, _obstacleLayers);

                if (hitColliders.Length > 0)
                {
                    for (int i = hits.Length - 1; i > 0; i--)
                    {
                        // Calculate the midpoint between this hit and the previous one
                        float midDistance = Vector3.Distance(hits[i].point, hits[i - 1].point) / 2f;

                        Vector3 midpoint = hits[i].point - direction * midDistance;

                        // Now check for obstacles at mid point using an overlap sphere
                        Collider[] hitPointColliders = Physics.OverlapSphere(midpoint, sphereRadius, _obstacleLayers);

                        if(hitPointColliders.Length == 0)
                        {
                            float distanceBetweenHits = hits[i].distance - hits[i - 1].distance;

                            if (distanceBetweenHits > checkRadius)
                            {
                                targetPoint = hits[i].point - direction * checkRadius;
                                break;
                            }
                            else
                            {
                                targetPoint = hits[0].point - checkRadius * direction;
                            }
                        }
                        else
                        {
                            targetPoint = hits[0].point - checkRadius * direction;
                        }
                    }
                }
            }

            targetPoint.y = _components.transform.position.y;

            return targetPoint;
        }

        private void PlayExplosionEffects(Vector3? position = null)
        {
            // Camera shake
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.EXPLOSION);

            Vector3 spawnPosition = _components.transform.position;

            if(position != null)
            {
                spawnPosition = (Vector3)position;
            }

            // Explosion effect
            var explosion = GameManager.Instance.VisualEffectsManager.Explosions.ElectricExplosion;
            explosion.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            float size = _weapon.ExplosionRadius / 2f;
            explosion.transform.localScale = new Vector3(size, size, size);
            explosion.gameObject.SetActive(true);
            explosion.Play();
        }
        
        public override void SetUpIndicatorSpecialValues(AirIndicator indicator)
        {
            if (indicator == null)
            {
                Debug.LogError($"No indicator passed to ability {name}");
                return;
            }

            IndicatorSettings settings = new IndicatorSettings()
            {
                AimRange = Vector2.one * TeleportDistance,
                AimSpeedMultiplier = 1,
                AvoidWalls = false,
                IsCrosshairTheParent = true,
                //LineState = AirIndicator.IndicatorLineState.GroundLine
                LineState = AirIndicator.IndicatorLineState.AirTrajectory,
                EndSize = 2f,
                LineHeight = 0f,
                AirLineWidth = 1f
            };

            indicator.SetValues(settings);
        }

        public override float GetDuration()
        {
            return ShrinkDuration * 2f;
        }

        #region Upgrades
        public override void Upgrade(SkillUpgrade upgrade)
        {
            base.Upgrade(upgrade);

            if (upgrade is ExplosiveTeleportationAbilityUpgrades teleportationUpgrades)
            {
                SetTeleportationDistance(TeleportDistance + teleportationUpgrades.TeleporationDistance);

                if (teleportationUpgrades.ExplosionMode != TeleportationExplosionMode.None)
                {
                    ExplosionMode |= teleportationUpgrades.ExplosionMode;
                }
            }
        }

        private void SetTeleportationDistance(float distance)
        {
            TeleportDistance = distance;
            _indicatorRange.y = TeleportDistance;
        }
        #endregion

        public override void OnAbilityFinished()
        {

        }

        public override void OnAbilityHoldStart()
        {

        }

        public override void OnAbilityHoldUpdate()
        {
            if (_debug)
            {
                Vector3 pos = GetTargetPoint();
                _testingSphere.position = pos;
            }
        }

        public override void OnAbilityInterrupted()
        {
            if (_debug)
            {
                _playgroundManager.ReturnTestingSphere();
            }
        }

        public override void Dispose()
        {

        }
    }
}