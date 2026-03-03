namespace Unity.DynamicDuo.Gameplay
{
    public static class LevelSession
    {
        public static int ColorCount { get; private set; } = 6;
        public static int TubeCapacity { get; private set; } = 4;
        public static int EmptyTubes { get; private set; } = 2;
        public static int Seed { get; private set; } = 0;
        public static bool UseGenerator { get; private set; } = false;

        public static void StartGenerated(int colorCount, int tubeCapacity, int emptyTubes)
        {
            ColorCount = colorCount;
            TubeCapacity = tubeCapacity;
            EmptyTubes = emptyTubes;
            UseGenerator = true;
            NewSeed();
        }

        public static void NewSeed() => Seed = UnityEngine.Random.Range(0, int.MaxValue);

        public static void KeepSeed() { }

        public static void Reset()
        {
            UseGenerator = false;
            Seed = 0;
        }
    }
}
