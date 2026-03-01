using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    [Serializable]
    public class TubeData
    {
        [SerializeField]
        private TubeColor[] m_colors = Array.Empty<TubeColor>();

        public TubeColor[] Colors => m_colors;

        public ColorSegment[] ToSegments()
        {
            if (m_colors == null || m_colors.Length == 0)
            {
                return Array.Empty<ColorSegment>();
            }

            TubeColor[] validColors = Array.FindAll(m_colors, c => c != TubeColor.None);

            if (validColors.Length == 0)
            {
                return Array.Empty<ColorSegment>();
            }

            List<ColorSegment> result = new();
            TubeColor current = m_colors[0];
            int count = 1;

            for (int i = 1; i < m_colors.Length; i++)
            {
                if (m_colors[i] == current)
                    count++;
                else
                {
                    result.Add(new ColorSegment(current, count));
                    current = m_colors[i];
                    count = 1;
                }
            }

            result.Add(new ColorSegment(current, count));
            return result.ToArray();
        }
    }
}
