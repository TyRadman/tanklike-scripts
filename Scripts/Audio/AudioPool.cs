using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Sound
{
    public class AudioPool : MonoBehaviour
    {
        [SerializeField] private AudioSourcePoolable _audioSourcePool;
        private List<AudioSourcePoolable> _sources = new List<AudioSourcePoolable>();
        private const int STARTING_SOURCES_COUNT = 5;

        public void SetUpPools()
        {
            for (int i = 0; i < STARTING_SOURCES_COUNT; i++)
            {
                AddAudioSource();
            }
        }

        public AudioSource GetAvailableSource()
        {
            AudioSourcePoolable source = _sources.Find(s => s.IsAvailable());

            // if there are available sources, then get it. Otherwise, add one
            if (source != null) return source.GetSource();

            AddAudioSource();
            source = _sources.Find(s => s.IsAvailable());
            return source.GetSource();
        }

        private void AddAudioSource()
        {
            AudioSourcePoolable audioSource = Instantiate(_audioSourcePool, transform);
            _sources.Add(audioSource);
        }

        public void Dispose()
        {
            foreach (var source in _sources)
            {
                source.Clear();
            }

            _sources.Clear();
        }
    }
}
