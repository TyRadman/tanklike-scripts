using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ItemsSystem
{
    using TankLike.UnitControllers;
    using TankLike.Utils;
    using Attributes;

    public class CollectableAttractor : MonoBehaviour
    {
        [SerializeField, InSelf(true)] private SphereCollider _collider;
        [SerializeField] private float _attractionDuration = 1f;

        private bool _canBeCollected = false;
        private bool _isAttracting = false;
        private Transform _target;
        private Collectable _collectable;

        public void SetUpAttractor(float attractionRadius, Collectable collectable)
        {
            _collider.radius = attractionRadius;
            _collectable = collectable;
        }

        public void EnableAttractor(bool enable)
        {
            _isAttracting = enable;
            _collider.enabled = enable;
            _canBeCollected = enable;

            if (!enable)
            {
                _target = null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isAttracting || _target != null || !_canBeCollected)
            {
                return;
            }

            _collider.enabled = false;
            _target = other.transform;
            StartCoroutine(AttractionProcess());
        }

        private IEnumerator AttractionProcess()
        {
            Transform collectable = _collectable.transform;
            float time = 0f;
            _isAttracting = true;
            AnimationCurve curve = GameManager.Instance.Constants.Collectables.AttractionCurve;
            Quaternion rotation = Quaternion.Euler(collectable.position - _target.position);

            while (time < _attractionDuration)
            {
                time += Time.deltaTime;
                float t = curve.Evaluate(time / _attractionDuration);
                collectable.position = Vector3.Lerp(collectable.position, _target.position, t);
                collectable.rotation = Quaternion.Lerp(collectable.rotation, rotation, t);

                if(Vector3.Distance(_collectable.transform.position, _target.position) < 0.2f)
                {
                    _isAttracting = false;
                    _collectable.OnCollected(_target.GetComponent<PlayerComponents>());
                }

                yield return null;
            }
        }
    }
}