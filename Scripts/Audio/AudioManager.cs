using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace TankLike.Sound
{
    public class AudioManager : MonoBehaviour, IManager
    {
        [SerializeField] private AudioDatabase _database;
        [SerializeField] private AudioSource _oneShotSource;
        //[SerializeField] private bool _hasAudio = true;
        [SerializeField] private AudioPool _pooler;
        [SerializeField] private AudioMixer _mainAudioMixer;
        [SerializeField] private AudioSource _bgMusicSource;

        [field: SerializeField, Header("Subcomponents")] public UIAudio UIAudio { get; private set; }

        public bool IsActive { get; private set; }

        private const string SFX_VOLUME = "SFXVolume";
        private const string BG_MUSIC_VOLUME = "BGMusicVolume";
        private const float FADE_OUT_DURATION = 1f;
        private const float FADE_IN_DURATION = 1f;

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _pooler.SetUpPools();
        }

        public void Dispose()
        {
            IsActive = false;

            _pooler.Dispose();
        }
        #endregion


        public AudioSource Play(Audio audioFile)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return null;
            }

            if (audioFile == null)
            {
                return null;
            }

            AudioSource source = null;

            if (!audioFile.OneShot)
            {
                source = PlayAudio(audioFile);
            }
            else
            {
                PlayOneShotAudio(audioFile);
            }

            return source;
        }

        /// <summary>
        /// Play audio within a playback duration 
        /// </summary>
        public AudioSource Play(Audio audioFile, float duration)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return null;
            }

            if (audioFile == null)
            {
                return null;
            }

            AudioSource source = null;

            if (!audioFile.OneShot)
            {
                source = PlayAudio(audioFile, duration);
            }
            else
            {
                PlayOneShotAudio(audioFile);
            }

            return source;
        }

        private AudioSource PlayAudio(Audio audio, float duration = 0f)
        {
            AudioSource source = _pooler.GetAvailableSource();
            source.loop = audio.Loop;
            source.clip = audio.Clip;
            source.volume = audio.VolumeMultiplier;

            float pitch = audio.Pitch;

            // If we pass a playback duration, use it to control the pitch
            if (duration > 0)
            {
                pitch = source.clip.length / duration;
            }

            source.pitch = pitch;
            source.Play();

            return source;
        }

        private void PlayOneShotAudio(Audio audio)
        {
            _oneShotSource.PlayOneShot(audio.Clip, audio.VolumeMultiplier);
        }

        public void SwitchBGMusic(Audio audio)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            AudioSource source = _bgMusicSource;
            source.clip = audio.Clip;
            source.volume = audio.VolumeMultiplier;
            source.pitch = audio.Pitch;
            source.Play();
        }

        public void StopBGMusic()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _bgMusicSource.Stop();
        }

        public void FadeOutBGMusic()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            StartCoroutine(StartFade(_mainAudioMixer, BG_MUSIC_VOLUME, FADE_OUT_DURATION, 0f));
        }

        public void FadeInBGMusic()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            StartCoroutine(StartFade(_mainAudioMixer, BG_MUSIC_VOLUME, FADE_IN_DURATION, 1f));
        }

        private IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
        {
            float currentTime = 0;
            float currentVol;

            audioMixer.GetFloat(exposedParam, out currentVol);
            currentVol = Mathf.Pow(10, currentVol / 20);
            float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
                audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
                yield return null;
            }

            yield break;
        }
    }
}
