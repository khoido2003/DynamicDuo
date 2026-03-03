using UnityEngine;

namespace Unity.DynamicDuo.Infrastructure
{
    public interface IAudioService
    {
        void PlaySFX(SoundId id);
        void PlayMusic(SoundId id);
        void StopMusic();
        void SetMusicVolume(float volume);
        void SetSFXVolume(float volume);
    }

    public enum SoundId
    {
        None = 0,
        Pour,
        Invalid,
        TubeComplete,
        Win,
        Lose,
        MusicGameplay,
    }
}
