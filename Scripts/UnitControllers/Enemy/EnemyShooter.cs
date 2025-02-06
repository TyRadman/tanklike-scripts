using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Misc;
    using Utils;
    using static PlayersManager;

    public class EnemyShooter : TankShooter
    {
        public System.Action OnTelegraphFinished { get; set; }
        public System.Action OnAttackFinished;

        [Header("Telegraphing")]
        [SerializeField] protected float _telegraphDuration = 1f;
        [SerializeField] protected float _telegraphOffset = 0.3f;

        [SerializeField] protected LayerMask _obstacleLayers;

        protected Coroutine _telegraphCoroutine;
        protected Coroutine _attackCoroutine;
        protected PlayerTransforms _currentTarget;
        protected EnemyComponents _enemyComponents;

        public override void SetUp(IController controller)
        {
            EnemyComponents components = controller as EnemyComponents;

            if (controller is null or not EnemyComponents and not BossComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _enemyComponents = components;

            base.SetUp(controller);

            if (_startWeaponHolder != null)
            {
                AddSkill(_startWeaponHolder);
            }
        }

        public bool IsWayToTargetBlocked(Transform target)
        {
            bool rightBlocked = false;
            bool leftBlocked = false;

            //right-ray
            var rightDir = (target.position - (transform.position + transform.right));
            rightDir.y = 0.5f;
            rightDir.Normalize();

            float rightDist = Vector3.Distance(transform.position + transform.right, target.position);
            RaycastHit rightHit;
            if (Physics.Raycast(transform.position + transform.right, rightDir, out rightHit, rightDist, _obstacleLayers))
            {
                if (!rightHit.collider.CompareTag("Destructible"))
                {
                    rightBlocked = true;
                    Debug.DrawRay(transform.position + transform.right, rightDir * rightDist, Color.red);
                }
            }
            else
            {
                rightBlocked = false;
                Debug.DrawRay(transform.position + transform.right, rightDir * rightDist, Color.yellow);
            }

            //left-ray
            var leftDir = (target.position - (transform.position - transform.right));
            leftDir.y = 0.5f;
            leftDir.Normalize();

            float leftDist = Vector3.Distance(transform.position - transform.right, target.position);
            RaycastHit leftHit;

            if (Physics.Raycast(transform.position - transform.right, leftDir, out leftHit, leftDist, _obstacleLayers))
            {
                if (!leftHit.collider.CompareTag("Destructible"))
                {
                    leftBlocked = true;
                    Debug.DrawRay(transform.position - transform.right, leftDir * leftDist, Color.red);
                }
            }
            else
            {
                leftBlocked = false;
                Debug.DrawRay(transform.position - transform.right, leftDir * leftDist, Color.yellow);
            }

            return rightBlocked || leftBlocked;
        }

        public void SetCurrentTarget(PlayerTransforms target)
        {
            _currentTarget = target;
        }

        public PlayerTransforms GetCurrentTarget()
        {
            return _currentTarget;
        }

        public void UnsetCurrentTarget()
        {
            _currentTarget = null;
        }

        public virtual void StartTelegraph()
        {
            this.StopCoroutineSafe(_telegraphCoroutine);

            _telegraphCoroutine = StartCoroutine(TelegraphRoutine());
        }

        protected virtual IEnumerator TelegraphRoutine()
        {
            ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Telegraphs.EnemyTelegraph;
            vfx.transform.parent = ShootingPoints[0];
            vfx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            vfx.transform.position += vfx.transform.forward * _telegraphOffset;
            vfx.gameObject.SetActive(true);
            vfx.Play(vfx.Particles.main.duration / _telegraphDuration);
            _activePoolables.Add(vfx);

            yield return new WaitForSeconds(_telegraphDuration);

            OnTelegraphFinished?.Invoke();
            vfx.TurnOff();
            _activePoolables.Remove(vfx);
        }

        public void StartAttackRoutine(int attacksAmount, float breakBetweenAttacks)
        {
            if (_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);
            _attackCoroutine = StartCoroutine(AttackRoutine(attacksAmount, breakBetweenAttacks));
        }

        private IEnumerator AttackRoutine(int attacksAmount, float breakBetweenAttacks)
        {
            int attackCounter = 0;
            WaitForSeconds breakWaitDuration = new WaitForSeconds(breakBetweenAttacks);

            while (attackCounter < attacksAmount)
            {
                Shoot();
                attackCounter++;
                yield return breakWaitDuration;
            }

            OnAttackFinished?.Invoke();
        }

        public void StartAttackRoutine(float duration)
        {
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
            }

            _attackCoroutine = StartCoroutine(AttackRoutine(duration));
        }

        private IEnumerator AttackRoutine(float duration)
        {
            Shoot();
            yield return new WaitForSeconds(duration);
            OnAttackFinished?.Invoke();
        }

        public void SetTelegraphSpeed(float speed)
        {
            _telegraphDuration = speed;
        }

        #region IController
        public override void Restart()
        {
            base.Restart();
            if (_telegraphCoroutine != null)
            {
                StopCoroutine(_telegraphCoroutine);
            }

            if (_activePoolables.Count > 0)
            {
                _activePoolables.ForEach(e => e.TurnOff());
            }

            _activePoolables.Clear();

            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_telegraphCoroutine != null)
            {
                StopCoroutine(_telegraphCoroutine);
            }

            if (_activePoolables.Count > 0)
            {
                _activePoolables.ForEach(e => e.TurnOff());
            }

            _activePoolables.Clear();

            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
            }
        }
        #endregion
    }
}
