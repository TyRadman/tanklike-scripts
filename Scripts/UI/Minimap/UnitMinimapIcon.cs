using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Minimap;

namespace TankLike.Minimap
{
    public class UnitMinimapIcon : MinimapIcon
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private bool _pulseIcon = false;

        protected override void Start()
        {
            if(!CanInitialize())
            {
                return;
            }

            base.Start();
            //SpawnIcon();
        }

        public void SpawnIcon()
        {
            _animation.clip = GameManager.Instance.Constants.Minimap.SpawnAnimation;
            _animation.Play();

            if (_pulseIcon) StartCoroutine(ImportantIconAnimationProcess());
        }

        private IEnumerator ImportantIconAnimationProcess()
        {
            yield return new WaitForSeconds(GameManager.Instance.Constants.Minimap.SpawnAnimation.length);

            _animation.clip = GameManager.Instance.Constants.Minimap.PulseAnimation;
            _animation.Play();
        }

        public void KillIcon()
        {
            Transform parent = transform.parent;
            transform.parent = null;
            StartCoroutine(IconDeathProcess(parent));
        }

        private IEnumerator IconDeathProcess(Transform parent)
        {
            _animation.clip = GameManager.Instance.Constants.Minimap.KillAnimation;
            _animation.Play();
            yield return new WaitForSeconds(GameManager.Instance.Constants.Minimap.KillAnimation.length);
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
        }
    }
}
