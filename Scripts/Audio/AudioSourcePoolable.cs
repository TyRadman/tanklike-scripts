using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Sound
{
    public class AudioSourcePoolable : MonoBehaviour, IPoolable
    {
        [SerializeField] private AudioSource _source;
        public Action<IPoolable> OnReleaseToPool { get; private set; }

        private void ResetAudioSource()
        {
            _source.clip = null;
            _source.volume = 1f;
            _source.pitch = 1f;
        }

        public bool IsAvailable()
        {
            return !_source.isPlaying;
        }

        public AudioSource GetSource()
        {
            return _source;
        }

        #region Pool
        public void Clear()
        {
            Destroy(gameObject);
        }

        public void Init(Action<IPoolable> onRelease)
        {
            OnReleaseToPool += onRelease;
        }

        public void OnRelease()
        {
            ResetAudioSource();
        }

        public void OnRequest()
        {
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }
        #endregion
    }
}
