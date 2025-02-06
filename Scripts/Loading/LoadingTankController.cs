using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Loading
{
    using TankLike.UnitControllers;

    public class LoadingTankController : MonoBehaviour
    {
        [field: SerializeField, Header("Components")] public TankComponents Components { private set; get; }

        private TankMovement _movement;
        private Coroutine _movementCoroutine;
        private Transform _startingPoint;
        private Transform _finishingPoint;
        private List<ParticleSystem> _boostParticles;

        public void SetReferences(Transform startingPoint, Transform finishingPoint)
        {
            _startingPoint = startingPoint;
            _finishingPoint = finishingPoint;
        }

        public void SetUp()
        {
            Components.TankBodyParts.SetUp(Components);
            Components.Movement.SetUp(Components);
            Components.Animation.SetUp(Components);

            _movement = Components.Movement;
            _movement.Restart();

            TankBody body = (TankBody)Components.TankBodyParts.GetBodyPartOfType(BodyPartType.Body);
            _boostParticles = body.BoostParticles;

            StartMovementRoutine();
        }

        public void StartMovementRoutine()
        {
            _movementCoroutine = StartCoroutine(MovementRoutine());
        }

        private IEnumerator MovementRoutine()
        {
            bool resetMovement = false;

            transform.position = _startingPoint.position;
            _movement.SetForwardAmount(1);

            _boostParticles.ForEach(p => p.Stop());
            _boostParticles.ForEach(p => p.Play());

            while (true)
            {
                if(Vector3.Distance(transform.position, _finishingPoint.position) < 0.1f && !resetMovement)
                {
                    resetMovement = true;
                    _movement.SetForwardAmount(0);

                    yield return new WaitForSeconds(0.25f);

                    transform.position = _finishingPoint.position + Vector3.down * 5f;

                    yield return new WaitForSeconds(0.25f);

                    transform.position = _startingPoint.position + Vector3.down * 5f;

                    yield return new WaitForSeconds(0.25f);

                    transform.position = _startingPoint.position;

                    yield return new WaitForSeconds(0.25f);

                    _movement.SetForwardAmount(1);
                    resetMovement = false;
                }

                _movement.MoveCharacterController(Vector3.right);
                yield return null;
            }
        }
    }
}
