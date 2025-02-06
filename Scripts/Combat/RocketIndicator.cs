using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat
{
    public class RocketIndicator : Indicator
    {

        [System.Serializable]
        private struct IndicatorPiece
        {
            public Transform _renderer;
            public Vector3 _initialPosition;
            public Vector3 _targetPosition;
        }

        [SerializeField] private IndicatorPiece[] _indicatorPieces;

        public override void Play(float duration)
        {
            for (int i = 0; i < _indicatorPieces.Length; i++)
            {
                IndicatorPiece piece = _indicatorPieces[i];
                piece._renderer.localPosition = piece._initialPosition;
            }

            StartCoroutine(IndicatorRoutine(duration));
        }

        private IEnumerator IndicatorRoutine(float duration)
        {
            float elapsedTime = 0f;
            float t = 0;

            while (t < 1)
            {
                elapsedTime += Time.deltaTime;
                t = Mathf.Clamp01(elapsedTime / duration);

                for (int i = 0; i < _indicatorPieces.Length; i++)
                {
                    IndicatorPiece piece = _indicatorPieces[i];
                    piece._renderer.localPosition = Vector3.Lerp(piece._initialPosition, piece._targetPosition, t);
                }

                yield return null;
            }
        }
    }
}
