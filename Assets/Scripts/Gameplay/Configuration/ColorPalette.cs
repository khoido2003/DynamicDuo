using System;
using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "DynamicDuo/ColorPalette")]
    public class ColorPalette : ScriptableObject
    {
        [Serializable]
        public struct ColorEntry
        {
            public TubeColor TubeColor;
            public Color DisplayColor;
        }

        [SerializeField]
        private ColorEntry[] m_entries;

        public Color GetColor(TubeColor tubeColor)
        {
            foreach (var entry in m_entries)
            {
                if (entry.TubeColor == tubeColor)
                {
                    return entry.DisplayColor;
                }
            }

            Debug.LogWarning($"[ColorPalette] No color found for {tubeColor}");
            return Color.white;
        }
    }
}
