using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using UnitControllers;
    using Combat.SkillTree;
    using TankLike.Combat.SkillTree.Upgrades;
    using TankLike.Cam;

    [CreateAssetMenu(fileName = PREFIX + "SuperShot", menuName = ROOT_PATH + "Ability")]
    public class SuperShot : Ability
    {
        public const string ROOT_PATH = Directories.ABILITIES + "Super Shot/";

        [Header("Special Values")]
        [SerializeField] private Clonable<Weapon> _weapon;
        [SerializeField] private float _animationDelay = 0.15f;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);

            if(_weapon.Instance == null)
            {
                _weapon.Initiate();
                _weapon.Instance.SetUp(components);
            }
        }

        public override void PopulateSkillProperties()
        {
            base.PopulateSkillProperties();

            SkillProperties damagePerProjectile = new SkillProperties()
            {
                Name = "Damage",
                Value = _weapon.GetOriginal().Damage.ToString(),
                DisplayColor = Colors.Red,
                UnitString = PropertyUnits.POINTS
            };

            SkillDisplayProperties.Add(damagePerProjectile);
        }

        public override void PerformAbility()
        {
            base.PerformAbility();

            _components.StartCoroutine(ShootSuperShot());
        }

        public override void Upgrade(SkillUpgrade upgrade)
        {
            base.Upgrade(upgrade);

            if (upgrade is not SuperShotUpgrade superShotUpgrade)
            {
                return;
            }

            _weapon.Instance.UpgradeDamage(superShotUpgrade.Damage);

            if (superShotUpgrade.Impact != null)
            {
                ((NormalShot)_weapon.Instance).SetImpactType(superShotUpgrade.Impact);
            }
        }

        private IEnumerator ShootSuperShot()
        {
            yield return new WaitForSeconds(_animationDelay);
            _weapon.Instance.OnShot();
            GameManager.Instance.CameraManager.Shake.ShakeCamera(CameraShakeType.GROUND_POUND);
        }

        public Weapon GetWeapon()
        {
            return _weapon.Instance;
        }

        public override void OnAbilityHoldStart()
        {
        }

        public override void OnAbilityHoldUpdate()
        {
        }

        public override void OnAbilityInterrupted()
        {
        }

        public override void OnAbilityFinished()
        {
        }

        public override void SetUpIndicatorSpecialValues(AirIndicator indicator)
        {
        }
    }
}
