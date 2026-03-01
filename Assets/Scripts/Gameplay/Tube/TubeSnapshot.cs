using System.Collections.Generic;

namespace Unity.DynamicDuo.Gameplay
{
    public readonly struct TubeSnapshot
    {
        public readonly int TubeIndex;
        public readonly IReadOnlyList<ColorSegment> Segments;

        public TubeSnapshot(int index, List<ColorSegment> segments)
        {
            TubeIndex = index;
            Segments = segments.AsReadOnly();
        }
    }
}
