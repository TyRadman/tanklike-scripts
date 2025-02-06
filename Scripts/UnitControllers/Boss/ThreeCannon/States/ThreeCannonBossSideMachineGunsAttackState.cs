using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.Misc;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_ThreeCannon_SideMachineGunsAttack", menuName = MENU_PATH + "Three Cannon/Side Machine Guns Attack State")]
    public class ThreeCannonBossSideMachineGunsAttackState : BossAttackState
    {
        [Header("Side Machine Guns Attack")]
        [SerializeField] private Weapon _sideMachineGunsWeapon;
        [SerializeField] private float _sideMachineGunsShootingDuration;
        [SerializeField] private float _sideMachineGunsShotsNumber;

        [Header("Telegraph")]
        [SerializeField] private float _telegraphEffectOffset;
        [SerializeField] private float _telegraphEffectSizeMultiplier;

        [Header("Machine Gun Animation")]
        [SerializeField] private float _machineGunAnimationMaxSpeed;
        [SerializeField] private float _machineGunAccelerationAnimationDuration;
        [SerializeField] private AnimationCurve _machineGunAccelerationAnimationCurve;
        [SerializeField] private float _machineGunDeccelerationAnimationDuration;
        [SerializeField] private AnimationCurve _machineGunDeccelerationAnimationCurve;

        private Vector3 _target;
        private bool _isFacingTarget;
        private float _machineGunAnimationCurrentspeed;
        private bool _playMachineGunAnimation;
        private Coroutine _machineGunAnimationCoroutine;
        private Coroutine _sideMachineGunsAnimationCoroutine;
        private bool _openTurrets;
        private bool _closeTurrets;
        private Coroutine _attackCoroutine;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);
            _sideMachineGunsWeapon.SetUp(bossComponents);
            _movement.OnTargetFaced += OnTargetFacedHandler;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _isActive = true;

            // Chance to attack a random target vs closest target
            float rand = Random.Range(0f, 1f);

            if (rand <= _randomTargetChance)
            {
                var alivePlayers = GameManager.Instance.PlayersManager.GetPlayerTransforms();
                var targetPlayerTransform = alivePlayers[Random.Range(0, alivePlayers.Count)];
                _attackController.SetTarget(targetPlayerTransform);
                _target = targetPlayerTransform.PlayerTransform.position;
            }
            else
            {
                var targetPlayerTransform = GameManager.Instance.PlayersManager.GetClosestPlayer(_movement.transform.position);
                _attackController.SetTarget(targetPlayerTransform);
                _target = targetPlayerTransform.PlayerTransform.position;
            }

            //Debug.Log(_target.gameObject.name);

            _movement.ResetTargetIsFaced();

            _attackCoroutine = _attackController.StartCoroutine(SideMachineGunsAttackRoutine());
            //_attackController.Attack(ThreeCannonAttackType.SideMachineGuns, OnAttackFinished);

            _isFacingTarget = true;
        }

        public override void OnUpdate()
        {
            if (_isFacingTarget)
                _movement.FaceTarget(_target);
        }

        public override void OnExit()
        {
            _isActive = false;
            _isFacingTarget = false;

            if (_attackCoroutine != null)
                _attackController.StopCoroutine(_attackCoroutine);
        }

        public override void OnDispose()
        {
        }

        private void OnTargetFacedHandler()
        {
            if (!_isActive)
                return;

            _isFacingTarget = false;
        }

        private void OnAttackFinished()
        {
            if (!_isActive)
                return;

            _stateMachine.ChangeState(BossStateType.Move);
        }

        #region Attack Methods
        private IEnumerator SideMachineGunsAttackRoutine()
        {
            _machineGunAnimationCurrentspeed = 0f;
            _playMachineGunAnimation = true;

            _sideMachineGunsAnimationCoroutine = _attackController.StartCoroutine(SideMachineGunsAnimationRoutine());

            float elapsedTime = 0f;
            float t = 0;

            _attackController.StartCoroutine(TelegraphRoutine(_attackController.RightMachineGunShootingPoint));
            _attackController.StartCoroutine(TelegraphRoutine(_attackController.LeftMachineGunShootingPoint));

            while (t < 1)
            {
                elapsedTime += Time.deltaTime;
                t = Mathf.Clamp01(elapsedTime / _machineGunAccelerationAnimationDuration);

                _machineGunAnimationCurrentspeed = _machineGunAnimationMaxSpeed * _machineGunAccelerationAnimationCurve.Evaluate(t);
                yield return null;
            }

            //yield return new WaitForSeconds(_machineGunTelegraphDuration);

            float shotDuration = _sideMachineGunsShootingDuration / _sideMachineGunsShotsNumber;
            WaitForSeconds wait = new WaitForSeconds(shotDuration);

            _openTurrets = true;

            for (int i = 0; i < _sideMachineGunsShotsNumber; i++)
            {
                _sideMachineGunsWeapon.OnShot(_attackController.RightMachineGunShootingPoint);
                _sideMachineGunsWeapon.OnShot(_attackController.LeftMachineGunShootingPoint);
                yield return wait;
            }

            elapsedTime = 0f;
            t = 0f;

            while (t < 1)
            {
                elapsedTime += Time.deltaTime;
                t = Mathf.Clamp01(elapsedTime / _machineGunDeccelerationAnimationDuration);

                _machineGunAnimationCurrentspeed = _machineGunAnimationMaxSpeed * _machineGunDeccelerationAnimationCurve.Evaluate(t);
                yield return null;
            }

            _playMachineGunAnimation = false;
            if (_sideMachineGunsAnimationCoroutine != null)
                _attackController.StopCoroutine(_sideMachineGunsAnimationCoroutine);

            OnAttackFinished();
            yield return null;
        }

        private IEnumerator SideMachineGunsAnimationRoutine()
        {
            while (_playMachineGunAnimation)
            {
                _attackController.RightMachineGunTransform.Rotate(Vector3.up, _machineGunAnimationCurrentspeed * Time.deltaTime);
                _attackController.LeftMachineGunTransform.Rotate(-Vector3.up, _machineGunAnimationCurrentspeed * Time.deltaTime);

                if (_openTurrets)
                {
                    _attackController.StartCoroutine(RotateSideTurretsRoutine(90f, _sideMachineGunsShootingDuration / 2f));
                    _openTurrets = false;
                }
                if (_closeTurrets)
                {
                    _attackController.StartCoroutine(RotateSideTurretsRoutine(0f, _sideMachineGunsShootingDuration / 2f));
                    _closeTurrets = false;
                }
                yield return null;
            }
        }

        private IEnumerator RotateSideTurretsRoutine(float targetAngle, float rotationDuration)
        {
            float timer = 0f;
            float t;

            Quaternion rightInitialRotation = _attackController.RightTurretTransform.localRotation;
            Quaternion leftInitialRotation = _attackController.LeftTurretTransform.localRotation;
            Quaternion rightTargetRotation = Quaternion.AngleAxis(targetAngle, Vector3.up);
            Quaternion leftTargetRotation = Quaternion.AngleAxis(-targetAngle, Vector3.up);

            while ((timer / rotationDuration) < 1f)
            {
                // Calculate the interpolation factor based on the elapsed time and rotation duration
                t = Mathf.Clamp01(timer / rotationDuration);

                // Interpolate between the initial and target rotations
                Quaternion rightNewRotation = Quaternion.Lerp(rightInitialRotation, rightTargetRotation, t);
                Quaternion leftNewRotation = Quaternion.Lerp(leftInitialRotation, leftTargetRotation, t);

                // Apply the new rotation
                _attackController.RightTurretTransform.localRotation = rightNewRotation;
                _attackController.LeftTurretTransform.localRotation = leftNewRotation;

                timer += Time.deltaTime;
                yield return null;
            }

            _attackController.RightTurretTransform.localRotation = rightTargetRotation;
            _attackController.LeftTurretTransform.localRotation = leftTargetRotation;

            if (targetAngle != 0f)
                _closeTurrets = true;
        }

        private IEnumerator TelegraphRoutine(Transform shootingPoint)
        {
            ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Telegraphs.EnemyTelegraph;
            vfx.transform.parent = shootingPoint;
            vfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            vfx.transform.position += vfx.transform.forward * _telegraphEffectOffset;
            vfx.transform.localScale = Vector3.one * _telegraphEffectSizeMultiplier;
            vfx.gameObject.SetActive(true);
            vfx.Play(vfx.Particles.main.duration / _machineGunAccelerationAnimationDuration);

            _attackController.AddToActivePooables(vfx);

            yield return new WaitForSeconds(_machineGunAccelerationAnimationDuration);

            vfx.TurnOff();
            _attackController.RemoveFromActivePooables(vfx);
        }
        #endregion
    }
}
