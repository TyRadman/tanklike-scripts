using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class VanishingWall : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _mesh;
        private Material _material;
        private const float FADING_DURATION = 0.2f;

        private void Awake()
        {
            _material = _mesh.material;
        }

        public void HideObject(float minAlpha)
        {
            StopAllCoroutines();
            StartCoroutine(LerpAlpha(1f, minAlpha));
        }

        public void ShowObject(float minAlpha)
        {
            StopAllCoroutines();
            StartCoroutine(LerpAlpha(minAlpha, 1f));
        }

        private IEnumerator LerpAlpha(float startValue, float endValue)
        {
            float time = 0f;
            Color wallColor = _material.GetColor("_BaseColor");

            while(time < FADING_DURATION)
            {
                time += Time.deltaTime;

                wallColor.a = Mathf.Lerp(startValue, endValue, time / FADING_DURATION);
                _material.SetColor("_BaseColor", wallColor);
                
                //_material.SetFloat("_Alpha", Mathf.Lerp(startValue, endValue, time / FADING_DURATION));

                yield return null;
            }
        }
    }
}
