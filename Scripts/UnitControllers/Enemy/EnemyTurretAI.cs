using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyTurretAI : MonoBehaviour
    {
        [SerializeField] private EnemyTurretController _controller;
        [SerializeField] private float _lookingAroundDuration = 2f;
        private bool _targetInSight = false;
        private WaitForSeconds _lookingAroundWait;
        private Coroutine _lookingAroundCoroutine;

        private const float SCANNING_FREQUENCY = 0.2f;
        private WaitForSeconds _scanningAroundWait;

        [Header("Shooting Variables")]
        [SerializeField] private float _shootingFrequency = 2f;
        private WaitForSeconds _shootingWait;
        [SerializeField] private EnemyShooter _shooter;

        private void Start()
        {
            _lookingAroundWait = new WaitForSeconds(_lookingAroundDuration);
            _scanningAroundWait = new WaitForSeconds(SCANNING_FREQUENCY);
            _shootingWait = new WaitForSeconds(_shootingFrequency);

            // the tank starts by looking around, until it sees a target, then it switches to aiming mode
            //_lookingAroundCoroutine = StartCoroutine(LookingAroundProcess());
            //StartCoroutine(ScanningForPlayers());
            // for now, we're gonna keep checking for the player the whole time, but in the future this must be enabled and disabled according to the enemy's behaviour
            StartCoroutine(ShootingProcess());
        }

        //private IEnumerator ScanningForPlayers()
        //{
        //    int playerLayerMask = GameManager.Instance.PlayersManager.PlayerLayerMask;

        //    // the true should be replaced with some bool that the AI will have like _isAlive
        //    while (true)
        //    {
        //        Vector3 direction = GameManager.Instance.PlayersManager.GetClosestPlayerTransform().position - transform.position;
        //        Physics.Raycast(transform.position, direction, out RaycastHit hit, 30f, playerLayerMask);

        //        if(hit.collider.gameObject.layer == PlayersManager.PlayerLayer)
        //        {
        //            // to stop the process of looking around if it was already started
        //            if (!_targetInSight)
        //            {
        //                //print($"Stop randomness {Time.time}".Color(Color.green));
        //                StopCoroutine(_lookingAroundCoroutine);
        //            }

        //            _targetInSight = true;
        //            _controller.RotateToTarget(hit.point);
        //        }
        //        else if(_targetInSight)
        //        {
        //            _targetInSight = false;
        //            _lookingAroundCoroutine = StartCoroutine(LookingAroundProcess());
        //        }

        //        yield return _scanningAroundWait;
        //    }
        //}

        //private IEnumerator LookingAroundProcess()
        //{
        //    while (!_targetInSight)
        //    {
        //        // we wait before we move so that there is a waiting time when the enemy loses the player before it starts looking around
        //        yield return _lookingAroundWait;
        //        //print($"Randomness Happens {Time.time}".Color(Color.red));
        //        Vector3 random = Random.insideUnitSphere;
        //        Vector3 randomPoint = transform.position + new Vector3(random.x, 0f, random.z);
        //        _controller.RotateToTarget(randomPoint);
        //    }
        //}

        // should be in a seperate script
        private IEnumerator ShootingProcess()
        {
            yield return new WaitForSeconds(2f);

            while (true)
            {
                if (_targetInSight)
                {
                    _shooter.Shoot();
                    yield return _shootingWait;
                }
                else
                {
                    // if the turret doesn't shoot then it checks for the player again right away
                    yield return null;
                }
            }
        }

        public bool HasTarget()
        {
            return _targetInSight;
        }
    }
}
