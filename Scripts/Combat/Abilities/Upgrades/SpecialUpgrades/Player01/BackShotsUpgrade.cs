using System.Collections;
using UnityEngine;

namespace TankLike.Combat.SkillTree.Upgrades
{
    using UnitControllers;
    using UI.HUD;

    [CreateAssetMenu(fileName = PREFIX + "Special_BackShots", menuName = Directories.SPECIAL_UPGRADES + "Player01/ Back Shots")]
    public class BackShotsUpgrade : SkillUpgrade
    {
        [Header(SPECIAL_VALUES_HEADER)]
        [SerializeField] private NormalShot _lowerWeapon;
        [SerializeField] private NormalShot _upperWeapon;
        [SerializeField] private NormalShot _mediumWeapon;
        [SerializeField] private StatModifierType _statType;
        [SerializeField] private float _shootingCooldown = 3f;

        private StatIconReference _statIconReference;
        private PlayerShooter _shooter;
        private PlayerStatsModifiersDisplayer _statsDisplayer;
        private VanguardAdditionalInfo _additionalInfo;
        private WaitForSeconds _shootingCooldownWait;
        private readonly WaitForSeconds _waitBetweenShots = new WaitForSeconds(0.1f);
        private bool _isShootingEnabled;
        private Coroutine _performShotsCoroutine;
        public override void SetUp(PlayerComponents player)
        {
            base.SetUp(player);

            _shooter = player.GetUnitComponent<PlayerShooter>();
            _additionalInfo = player.GetUnitComponent<VanguardAdditionalInfo>();
            _statsDisplayer = GameManager.Instance.HUDController.PlayerHUDs[player.PlayerIndex].StatModifiersDisplayer;

            _lowerWeapon.SetUp(player);
            _mediumWeapon.SetUp(player);
            _upperWeapon.SetUp(player);

            _statIconReference = GameManager.Instance.StatIconReferenceDB.GetStatIconReference(_statType);

            _shootingCooldownWait = new WaitForSeconds(_shootingCooldown);
        }

        public override void SetUpgradeProperties(PlayerComponents player)
        {
            base.SetUpgradeProperties(player);

            //SkillProperties healthThreshold = new SkillProperties()
            //{
            //    IsComparisonValue = false,
            //    Name = "Activation Threshold",
            //    Value = (_healthThresholdToFill * 100f).ToString(),
            //    DisplayColor = Colors.LightOrange,
            //    UnitString = PropertyUnits.PERCENTAGE
            //};

            //_properties.Add(healthThreshold);

            //SaveProperties();
        }

        public override void ApplyUpgrade()
        {
            base.ApplyUpgrade();

            _isShootingEnabled = true;
            _statsDisplayer.AddIcon(_statIconReference);
            _shooter.OnShootStarted += ShootFromBackCannons;
        }

        public override void CancelUpgrade()
        {
            base.CancelUpgrade();

            _isShootingEnabled = false;
            _statsDisplayer.RemoveIcon(_statIconReference);
            _shooter.OnShootStarted -= ShootFromBackCannons;
            GameManager.Instance.CoroutineManager.StopExternalCoroutine(_performShotsCoroutine);
        }

        private void ShootFromBackCannons()
        {
            if (!_isShootingEnabled)
            {
                return;
            }

            _isShootingEnabled = false;
            _statsDisplayer.RemoveIcon(_statIconReference);

            _performShotsCoroutine = GameManager.Instance.CoroutineManager.StartExternalCoroutine(PerformShots());
        }

        private IEnumerator PerformShots()
        {
            _lowerWeapon.OnShot(_additionalInfo.BackShootingPoints[0], 0f);
            yield return _waitBetweenShots;
            _mediumWeapon.OnShot(_additionalInfo.BackShootingPoints[1], 0f);
            yield return _waitBetweenShots;
            _upperWeapon.OnShot(_additionalInfo.BackShootingPoints[2], 0f);

            yield return _shootingCooldownWait;

            _statsDisplayer.AddIcon(_statIconReference);
            _isShootingEnabled = true;
        }
    }
}
