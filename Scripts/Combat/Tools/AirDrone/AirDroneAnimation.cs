using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.AirDrone
{
    public class AirDroneAnimation : MonoBehaviour
    {
        [SerializeField] private Transform _body;
        [SerializeField] private float _frequency = 5f;
        [SerializeField] private float _height = 0.2f;
        [SerializeField] private float _startingHeight = 3f;
        [SerializeField] private Animation _anim;
        [SerializeField] private AnimationClip _spawnClip;
        [SerializeField] private AnimationClip _despawnClip;

        void Update()
        {
            _body.localPosition = (Vector3.up) * (Mathf.Sin(Time.time * _frequency) * _height + _startingHeight);
        }

        public void LiftDrone()
        {
            _body.localPosition = (Vector3.up) * _startingHeight;
        }

        public void PlaySpawnAnimation()
        {
            _anim.clip = _spawnClip;
            _anim.Play();
        }

        public void PlayDespawnAnimation()
        {
            _anim.clip = _despawnClip;
            _anim.Play();
        }

        public float GetSpawnAnimationLength()
        {
            return _anim.clip.length;
        }
    }
}
