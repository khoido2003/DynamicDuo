using UnityEngine;

namespace Unity.DynamicDuo.Infrastructure
{
    public class AudioService : IAudioService
    {
        // SFX and music
        readonly AudioSource m_sfxSource;
        readonly AudioSource m_musicSource;
        readonly AudioClipRegistry m_registry;

        public AudioService(
            AudioSource sfxSource,
            AudioSource musicSource,
            AudioClipRegistry registry
        )
        {
            m_sfxSource = sfxSource;
            m_musicSource = musicSource;
            m_registry = registry;
        }

        public void PlaySFX(SoundId id)
        {
            AudioClip clip = m_registry.GetClip(id);
            if (clip == null)
            {
                Debug.LogWarning($"[AudioService] No clip for {id}");
                return;
            }
            m_sfxSource.PlayOneShot(clip);
        }

        public void PlayMusic(SoundId id)
        {
            AudioClip clip = m_registry.GetClip(id);
            if (clip == null)
            {
                Debug.LogWarning($"[AudioService] No music clip for {id}");
                return;
            }

            if (m_musicSource.clip == clip && m_musicSource.isPlaying)
                return;

            m_musicSource.clip = clip;
            m_musicSource.loop = true;
            m_musicSource.Play();
        }

        public void StopMusic() => m_musicSource.Stop();

        public void SetMusicVolume(float volume) => m_musicSource.volume = Mathf.Clamp01(volume);

        public void SetSFXVolume(float volume) => m_sfxSource.volume = Mathf.Clamp01(volume);
    }
}
