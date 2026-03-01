using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.DynamicDuo.Gameplay
{
    public class TubeModel
    {
        public int Index { get; }
        public int Capacity { get; }

        public IReadOnlyList<ColorSegment> Segments => m_segments;

        public bool IsEmpty => m_segments.Count == 0;
        public int FilledCount => m_segments.Sum(s => s.Count);
        public int FreeSpace => Capacity - FilledCount;
        public bool IsFull => FreeSpace == 0;

        public bool IsComplete =>
            !IsEmpty && m_segments.Count == 1 && m_segments[0].Count == Capacity;

        public ColorSegment? TopSegment => IsEmpty ? null : m_segments[m_segments.Count - 1];

        private readonly List<ColorSegment> m_segments = new();

        //////////////////////////////////////////

        public TubeModel(int index, int capacity)
        {
            Index = index;
            Capacity = capacity;
        }

        public void LoadSegments(IEnumerable<ColorSegment> segments)
        {
            m_segments.Clear();
            m_segments.AddRange(segments);
        }

        #region Pour Logic

        public int GetPourAmount(TubeModel source)
        {
            if (source.IsEmpty)
            {
                return 0;
            }

            if (IsFull)
            {
                return 0;
            }

            if (source.Index == Index)
            {
                return 0;
            }

            ColorSegment sourceTopSegment = source.TopSegment.Value;

            if (!IsEmpty && TopSegment.Value.Color != sourceTopSegment.Color)
            {
                return 0;
            }

            return Math.Min(sourceTopSegment.Count, FreeSpace);
        }

        public int ReceiveFrom(TubeModel source)
        {
            int amount = GetPourAmount(source);

            if (amount == 0)
            {
                return 0;
            }

            ColorSegment sourceTop = source.TopSegment.Value;

            source.Pop(amount);

            Push(sourceTop.Color, amount);

            return amount;
        }

        #endregion

        ///////////////////////////////////////////////////////////////

        #region Stack method


        private void Pop(int amount)
        {
            int lastIndex = m_segments.Count - 1;
            ColorSegment topSegment = m_segments[lastIndex];

            if (topSegment.Count == amount)
            {
                m_segments.RemoveAt(lastIndex);
            }
            else
            {
                m_segments[lastIndex] = topSegment.UpdateToCount(topSegment.Count - amount);
            }
        }

        private void Push(TubeColor color, int amount)
        {
            if (!IsEmpty && TopSegment.Value.Color == color)
            {
                int lastIndex = m_segments.Count - 1;

                m_segments[lastIndex] = m_segments[lastIndex]
                    .UpdateToCount(m_segments[lastIndex].Count + amount);
            }
            else
            {
                m_segments.Add(new ColorSegment(color, amount));
            }
        }
        #endregion


        /////////////////////////////////////////////////////////////

        #region Snapshot

        public TubeSnapshot TakeSnapshot()
        {
            return new(Index, m_segments.Select(s => s).ToList());
        }

        public void RestoreSnapshot(TubeSnapshot snapshot)
        {
            m_segments.Clear();
            m_segments.AddRange(snapshot.Segments);
        }

        #endregion
    }
}
