using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyMovementAI : MonoBehaviour
    {
        [SerializeField] private EnemyMovement _movement;
        [SerializeField] private EnemyTurretAI _turret;
        [SerializeField] private float _movementUpdateFrequency = 0.2f;
        [SerializeField] private float _minimumDistanceToPlayer = 7f;
        private WaitForSeconds _movementWait;

        private void Start()
        {
            _movementWait = new WaitForSeconds(_movementUpdateFrequency);
            //StartCoroutine(MovementProcess());
        }
    }
}
