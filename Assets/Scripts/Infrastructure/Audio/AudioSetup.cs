// Scripts/Infrastructure/Audio/AudioSetup.cs
using UnityEngine;

namespace Unity.DynamicDuo.Infrastructure
{
    public class AudioSetup : MonoBehaviour
    {
        [SerializeField]
        private AudioClipRegistry m_registry;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_musicVolume = 0.5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_sfxVolume = 1.0f;

        public AudioClipRegistry Registry => m_registry;

        public (AudioSource sfx, AudioSource music) CreateSources()
        {
            DontDestroyOnLoad(gameObject);

            var sfx = gameObject.AddComponent<AudioSource>();
            sfx.volume = m_sfxVolume;
            sfx.playOnAwake = false;

            var music = gameObject.AddComponent<AudioSource>();
            music.volume = m_musicVolume;
            music.loop = true;
            music.playOnAwake = false;

            return (sfx, music);
        }
    }
}
