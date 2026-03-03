// Scripts/Gameplay/LevelGeneratorModel.cs
using System.Collections.Generic;

namespace Unity.DynamicDuo.Gameplay
{
    public class LevelGeneratorModel
    {
        public List<ColorSegment[]> Generate(
            int colorCount,
            int tubeCapacity,
            int emptyTubeCount,
            int seed
        )
        {
            var random = new System.Random(seed);

            //  6 colors × 4 capacity = 24 units total
            var pool = new List<TubeColor>();
            for (int c = 1; c <= colorCount; c++)
            for (int u = 0; u < tubeCapacity; u++)
                pool.Add((TubeColor)c);

            //  Fisher-Yates shuffle the pool
            for (int i = pool.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (pool[i], pool[j]) = (pool[j], pool[i]);
            }

            //  distribute into tubes, bottom to top
            var result = new List<ColorSegment[]>();
            int poolIndex = 0;

            for (int t = 0; t < colorCount; t++)
            {
                var flat = new List<TubeColor>();
                for (int u = 0; u < tubeCapacity; u++)
                    flat.Add(pool[poolIndex++]);
                result.Add(ToSegments(flat));
            }

            //  add empty tubes as buffer
            for (int e = 0; e < emptyTubeCount; e++)
                result.Add(System.Array.Empty<ColorSegment>());

            return result;
        }

        private ColorSegment[] ToSegments(List<TubeColor> flat)
        {
            if (flat.Count == 0)
                return System.Array.Empty<ColorSegment>();

            var segments = new List<ColorSegment>();
            TubeColor current = flat[0];
            int count = 1;

            for (int i = 1; i < flat.Count; i++)
            {
                if (flat[i] == current)
                    count++;
                else
                {
                    segments.Add(new ColorSegment(current, count));
                    current = flat[i];
                    count = 1;
                }
            }

            segments.Add(new ColorSegment(current, count));
            return segments.ToArray();
        }
    }
}
