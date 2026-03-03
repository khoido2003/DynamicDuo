using System;
using UnityEngine;

namespace Unity.DynamicDuo.Infrastructure
{
    [CreateAssetMenu(fileName = "AudioClipRegistry", menuName = "DynamicDuo/AudioClipRegistry")]
    public class AudioClipRegistry : ScriptableObject
    {
        [Serializable]
        public struct SoundEntry
        {
            public SoundId Id;
            public AudioClip Clip;
        }

        [SerializeField]
        private SoundEntry[] m_entries;

        public AudioClip GetClip(SoundId id)
        {
            foreach (var entry in m_entries)
            {
                if (entry.Id == id)
                {
                    return entry.Clip;
                }
            }
            return null;
        }
    }
}
