using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.Misc;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers.States
{
    [CreateAssetMenu(fileName = "State_ThreeCannon_MachineGunAttack", menuName = MENU_PATH + "Three Cannon/Machine Gun Attack State")]
    public class ThreeCannonBossMachineGunAttackState : BossAttackState
    {
        [Header("Machine Gun Attack")]
        [SerializeField] private Weapon _machineGunWeapon;
        [SerializeField] private float _machineGunShootingDuration;
        [SerializeField] private float _machineGunShotsNumber;
        [SerializeField] private float _machineGunMaxSpreadAngle;
        [SerializeField] private Vector2 _imageDetectionDistanceRange = new Vector2(0f, 4.5f);

        private readonly Vector2 _distanceRange = new Vector2(8f, 20f);

        [Header("Telegraph")]
        [SerializeField] private float _telegraphEffectOffset;
        [SerializeField] private float _telegraphEffectSizeMultiplier;

        [Header("Machine Gun Animation")]
        [SerializeField] private float _machineGunAnimationMaxSpeed;
        [SerializeField] private float _machineGunAccelerationAnimationDuration;
        [SerializeField] private AnimationCurve _machineGunAccelerationAnimationCurve;
        [SerializeField] private float _machineGunDeccelerationAnimationDuration;
        [SerializeField] private AnimationCurve _machineGunDeccelerationAnimationCurve;

        //private Transform _target;
        private float _machineGunAnimationCurrentspeed;
        private bool _playMachineGunAnimation;
        private Coroutine _machineGunAnimationCoroutine;
        private Coroutine _sideMachineGunsAnimationCoroutine;
        private bool _openTurrets;
        private bool _closeTurrets;
        private Coroutine _attackCoroutine;
        private PlayerPredictedPosition _targetImage;

        public override void SetUp(StateMachine<BossStateType> stateMachine, BossComponents bossComponents)
        {
            base.SetUp(stateMachine, bossComponents);
            _machineGunWeapon.SetUp(bossComponents);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _isActive = true;

            // Chance to attack a random target vs closest target
            float rand = Random.Range(0f, 1f);
            
            if(rand <= _randomTargetChance)
            {
                var alivePlayers = GameManager.Instance.PlayersManager.GetPlayerTransforms();
                var targetPlayerTransform = alivePlayers[Random.Range(0, alivePlayers.Count)];
                _attackController.SetTarget(targetPlayerTransform);
                _targetImage = targetPlayerTransform.PredictedPosition;
            }
            else
            {
                var targetPlayerTransform = GameManager.Instance.PlayersManager.GetClosestPlayer(_movement.transform.position);
                _attackController.SetTarget(targetPlayerTransform);
                _targetImage = targetPlayerTransform.PredictedPosition;
            }

            //Debug.Log(_target.gameObject.name);

            _attackCoroutine = _attackController.StartCoroutine(MachineGunAttackRoutine());

            //_attackController.Attack(ThreeCannonAttackType.MainMachineGun, OnAttackFinished);
        }

        public override void OnUpdate()
        {
            float distanceToPlayer = _components.transform.GetDistanceTo(_targetImage.transform.position);
            float distanceProgress = Mathf.InverseLerp(_distanceRange.x, _distanceRange.y, distanceToPlayer);
            float detectionDistance = _imageDetectionDistanceRange.Lerp(distanceProgress);

            _movement.FaceTarget(_targetImage.GetPositionAtDistance(detectionDistance));
        }

        public override void OnExit()
        {
            _isActive = false;

            if (_attackCoroutine != null)
                _attackController.StopCoroutine(_attackCoroutine);
        }

        public override void OnDispose()
        {
        }

        private void OnAttackFinished()
        {
            if (!_isActive)
                return;

            _stateMachine.ChangeState(BossStateType.Move);
        }

        #region Attack Methods
        private IEnumerator MachineGunAttackRoutine()
        {
            _machineGunAnimationCurrentspeed = 0f;
            _playMachineGunAnimation = true;

            float elapsedTime = 0f;
            float t = 0;

            _machineGunAnimationCoroutine = _attackController.StartCoroutine(MachineGunAnimationRoutine());

            _attackController.StartCoroutine(TelegraphRoutine(_attackController.MachineGunShootingPoint));

            while (t < 1)
            {
                elapsedTime += Time.deltaTime;
                t = Mathf.Clamp01(elapsedTime / _machineGunAccelerationAnimationDuration);

                _machineGunAnimationCurrentspeed = _machineGunAnimationMaxSpeed * _machineGunAccelerationAnimationCurve.Evaluate(t);
                yield return null;
            }

            //yield return new WaitForSeconds(_machineGunTelegraphDuration);

            float shotDuration = _machineGunShootingDuration / _machineGunShotsNumber;
            WaitForSeconds wait = new WaitForSeconds(shotDuration);
            float halfAngle = _machineGunMaxSpreadAngle / 2;

            for (int i = 0; i < _machineGunShotsNumber; i++)
            {
                float angle = UnityEngine.Random.Range(-halfAngle, halfAngle);
                _machineGunWeapon.OnShot(_attackController.MachineGunShootingPoint, angle);
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
            if (_machineGunAnimationCoroutine != null)
                _attackController.StopCoroutine(_machineGunAnimationCoroutine);

            OnAttackFinished();
            yield return null;
        }

        private IEnumerator MachineGunAnimationRoutine()
        {
            Transform machineGunTransform = _attackController.MachineGunTransform;

            while (_playMachineGunAnimation)
            {
                machineGunTransform.Rotate(Vector3.up, _machineGunAnimationCurrentspeed * Time.deltaTime);
                yield return null;
            }
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
