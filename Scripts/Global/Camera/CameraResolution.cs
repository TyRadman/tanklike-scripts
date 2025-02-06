using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class CameraResolution : MonoBehaviour
    {
        [SerializeField] private Vector2 _resolution;
        [SerializeField] private Camera _cam;

        void Update()
        {
            Vector2 resViewport = new Vector2(Screen.width, Screen.height);
            Vector2 resNormalized = _resolution / resViewport; // target res in viewport space
            Vector2 size = resNormalized / Mathf.Max(resNormalized.x, resNormalized.y);
            _cam.rect = new Rect(default, size) { center = new Vector2(0.5f, 0.5f) };
        }
    }
}
