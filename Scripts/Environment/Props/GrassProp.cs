using System.Collections;
using System.Collections.Generic;
using TankLike.Attributes;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    public class GrassProp : MonoBehaviour
    {
        [SerializeField] private List<Renderer> _grassQuads;
        [SerializeField] private AnimationCurve _swayCurve;
        [SerializeField] private GameObject _test;

        private const string GRASS_POSITION_ID = "_TargetPosition";
        private const string GRASS_TEXTURE_01_ID = "_BackTexture";
        private const string GRASS_TEXTURE_02_ID = "_FrontTexture";

        private const string GRASS_FRONT_TOP_COLOR_ID = "_FrontTopColor";
        private const string GRASS_FRONT_BOTTOM_COLOR_ID = "_FrontBottomColor";
        private const string GRASS_BACK_TOP_COLOR_ID = "_BackTopColor";
        private const string GRASS_BACK_BOTTOM_COLOR_ID = "_BackBottomColor";

        private const string FIRST_COLOR_THRESESHOLD_ID = "_FirstColorLerpThreshold";

        private Vector3 _currentPosition = Vector3.zero;

        private readonly Vector3 _defaultPosition = new Vector3(0.001f, 0, 0);

        private const float SHAKE_DURATION = 1f;
        private const float RESTORE_DURATION = 0.4f;

        private void OnTriggerEnter(Collider other)
        {
            StopAllCoroutines();

            foreach (Renderer grassQuad in _grassQuads)
            {
                StartCoroutine(ShakeProcess(transform.position - other.transform.position, SHAKE_DURATION));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            StopAllCoroutines();

            foreach (Renderer grassQuad in _grassQuads)
            {
                StartCoroutine(ShakeProcess(_defaultPosition, RESTORE_DURATION));
            }
        }

        private IEnumerator ShakeProcess(Vector3 finalPosition, float duration)
        {
            float timer = 0;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                for(int i = 0; i < _grassQuads.Count; i++)
                {
                    float t = _swayCurve.Evaluate(timer / SHAKE_DURATION);
                    _currentPosition = Vector3.Lerp(_currentPosition, finalPosition, t);
                    _grassQuads[i].material.SetVector(GRASS_POSITION_ID, _currentPosition);
                }

                yield return null;
            }
        }

        internal void ApplyTexture(Texture2D backTexture, Texture2D frontTexture)
        {
            for (int i = 0; i < _grassQuads.Count; i++)
            {
                _grassQuads[i].material.SetTexture(GRASS_TEXTURE_01_ID, backTexture);
                _grassQuads[i].material.SetTexture(GRASS_TEXTURE_02_ID, frontTexture);
            }
        }

        internal void SetStartColorThreshold(float threshold)
        {
            for (int i = 0; i < _grassQuads.Count; i++)
            {
                _grassQuads[i].material.SetFloat(FIRST_COLOR_THRESESHOLD_ID, threshold);
            }
        }

        internal void ApplyColors(GrassPainter.GrassColors grassColors)
        {
            for (int i = 0; i < _grassQuads.Count; i++)
            {
                Material material = _grassQuads[i].material;

                material.SetColor(GRASS_FRONT_TOP_COLOR_ID, grassColors.FrontTopColor);
                material.SetColor(GRASS_FRONT_BOTTOM_COLOR_ID, grassColors.FrontBottomColor);
                material.SetColor(GRASS_BACK_TOP_COLOR_ID, grassColors.BackTopColor);
                material.SetColor(GRASS_BACK_BOTTOM_COLOR_ID, grassColors.BackBottomColor);
            }
        }
    }
}
