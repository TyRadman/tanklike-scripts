using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class SummonMovement : MonoBehaviour
    {
        public System.Action OnReachedSummoner;
        public System.Action OnReachedTarget;

        [Header("Settings")]
        [SerializeField] protected float _movementSpeed;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _stopDistance;

        private Transform _summoner;

        public void SetUp(Transform summoner)
        {
            _summoner = summoner;
        }

        public float GetDistanceToSummoner()
        {
            if (_summoner == null) return 0;

            return Vector3.Distance(transform.position, _summoner.position);
        }

        public void FollowSummoner()
        {
            Vector3 dir = _summoner.position - transform.position;
            dir.y = 0f;
            dir.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            float dist = Vector3.Distance(transform.position, _summoner.position);
            if(dist > _stopDistance)
            {
                transform.position += dir * _movementSpeed * Time.deltaTime;
            }
            else
            {
                OnReachedSummoner?.Invoke();
            }
        }

        public void MoveToTarget(Transform target, float stopDistance)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0f;
            dir.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            float dist = Vector3.Distance(transform.position, target.position);
            float dot = Vector3.Dot(transform.forward, dir);
            if (dist > stopDistance)
            {
                transform.position += dir * _movementSpeed * Time.deltaTime;           
            }
            else
            {
                if (dot > 0.98f)
                    OnReachedTarget?.Invoke();
            }
        }
    }
}
