using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class ParticlesChildDetacher : MonoBehaviour
    {
        private Transform _parent;
        [SerializeField] private float _delay = 1f;
        [SerializeField] private ParticleSystem _particles;

        public void DetachAndReattach()
        {
            if (transform.parent == null)
            {
                return;
            }

            _parent = transform.parent;
            transform.parent = null;
            _particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            Invoke(nameof(AttackToParent), _delay);
        }

        private void AttackToParent()
        {
            transform.parent = _parent;
        }
    }
}
