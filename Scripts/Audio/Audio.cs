using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Sound
{
    [CreateAssetMenu(fileName = "A_", menuName = Directories.MAIN + "Audio/Audio file")]
    public class Audio : ScriptableObject
    {
        private const float PITCH_VALUE = 3f;

        public string AudioName;
        public AudioClip Clip;
        [Range(0f, 2f)] public float VolumeMultiplier = 1f;
        public bool OneShot;
        public bool Loop;

        // pitch
        public float Pitch
        {
            set
            {

            }
            get
            {
                return _randomPitch ? Random.Range(_minPitch, _maxPitch) : _pitchValue;
            }

        }

        [Range(-PITCH_VALUE, PITCH_VALUE)] public float _pitchValue = 1f;
        public bool _randomPitch = false;
        [Range(-PITCH_VALUE, PITCH_VALUE)] public float _maxPitch = 2f;
        [Range(-PITCH_VALUE, PITCH_VALUE)] public float _minPitch = 0f;
    }
}
