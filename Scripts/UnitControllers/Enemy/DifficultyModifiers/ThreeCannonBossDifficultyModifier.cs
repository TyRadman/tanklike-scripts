using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "DF_ThreeCannonBoss", menuName = MENU_NAME + "Three Cannon Boss")]
    public class ThreeCannonBossDifficultyModifier : DifficultyModifier
    {
        [Header("Main Machine Gun")]
        [SerializeField] private Vector2 _machineGunShootingDurationRange;
        [SerializeField] private Vector2 _machineGunTelegraphDurationRange;

        [Header("Side Machine Guns")]
        [SerializeField] private Vector2 _sideMachineGunsShootingDurationRange;

        [Header("Ground Pound")]
        [SerializeField] private Vector2 _groundPoundDurationRange;
        [SerializeField] private Vector2 _groundPoundAOERange;

        [Header("Rocket Launcher")]
        [SerializeField] private Vector2 _rocketLauncherProjectilesCountRange;
        [SerializeField] private Vector2 _rocketLauncherTimeBetweenProjectilesRange;
        [SerializeField] private Vector2 _rocketLauncherProjectileAOERange;
        [SerializeField] private Vector2 _rocketLauncherProjectileFlyDurationRange;

        public override void ApplyModifier(TankComponents boss, float difficulty)
        {
            base.ApplyModifier(boss, difficulty);

            BossAttackController attackController = ((BossComponents)boss).AttackController;

            attackController.SetMachineGunValues(_machineGunShootingDurationRange.Lerp(difficulty));
            attackController.SetSideMachineGunsValues(_sideMachineGunsShootingDurationRange.Lerp(difficulty));
        }
    }
}
