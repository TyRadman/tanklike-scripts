using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [RequireComponent(typeof(Rigidbody))]
    public class TankPart : MonoBehaviour
    {
        [SerializeField] private bool _shrinkingDelay;

        private const float SHRINKING_DURATION = 0.5f;
        private const float SHRINKING_DELAY = 2f;

        public Rigidbody RigidBody;
        public MeshRenderer Renderer;

        private void OnValidate()
        {
            RigidBody = GetComponent<Rigidbody>();
            Renderer = GetComponent<MeshRenderer>();
        }

        public void StartShrinkingCountDown(float duration)
        {
            StartCoroutine(ShrinkingProcess(duration));
        }

        private IEnumerator ShrinkingProcess(float duration)
        {
            yield return new WaitForSeconds(duration);

            float time = 0f;

            if (_shrinkingDelay)
            {
                yield return new WaitForSeconds(SHRINKING_DELAY);
            }

            while (time < SHRINKING_DURATION)
            {
                time += Time.deltaTime;

                transform.localScale = Vector3.one * Mathf.Lerp(1f, 0f, time / SHRINKING_DURATION);

                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
