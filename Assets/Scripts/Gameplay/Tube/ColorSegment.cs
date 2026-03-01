namespace Unity.DynamicDuo.Gameplay
{
    // NOTE: struct to avoid heap allocation compare to class
    public readonly struct ColorSegment
    {
        public readonly TubeColor Color;
        public readonly int Count;

        public ColorSegment(TubeColor color, int count)
        {
            Color = color;
            Count = count;
        }

        public ColorSegment UpdateToCount(int newCount) => new(Color, newCount);

        public override string ToString()
        {
            return $"{Color} x {Count}";
        }
    }
}
