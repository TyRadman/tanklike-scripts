using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Sound
{
    [CreateAssetMenu(fileName = "AudioDataBase", menuName = "TankLike/Audio/Audio Database")]
    public class AudioDatabase : ScriptableObject
    {
        public Dictionary<string, Audio> Audios = new Dictionary<string, Audio>();
        [SerializeField] private List<Audio> _audios;

        public void OnEnable()
        {
            Audios = new Dictionary<string, Audio>();
            //Debug.Log(Audios.Count);

            for (int i = 0; i < _audios.Count; i++)
            {
                //if (Audios.ContainsKey(audio.AudioName)) Audios.Remove(audio.AudioName);
                Audio audio = _audios[i];

                //Debug.Log(i);
                Audios.Add(audio.AudioName, audio);
            }
        }
    }
}
