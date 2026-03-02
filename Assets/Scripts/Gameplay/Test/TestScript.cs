using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class TestScript : MonoBehaviour
    {
        [SerializeField]
        private LevelData m_levelData;

        [SerializeField]
        private GameConfig m_gameConfig;

        void Start()
        {
            // Validate first
            if (!m_levelData.Validate(m_gameConfig, out string error))
            {
                Debug.LogError(error);
                return;
            }

            // Build segment lists from TubeData
            var segmentLists = new System.Collections.Generic.List<ColorSegment[]>();
            foreach (var tube in m_levelData.Tubes)
                segmentLists.Add(tube.ToSegments());

            // Initialize board
            var board = new BoardModel();
            board.Initialize(m_levelData.TubeCount, m_gameConfig.TubeCapacity, segmentLists);

            // Verify it loaded
            Debug.Log($"Board initialized with {board.Tubes.Count} tubes");
            for (int i = 0; i < board.Tubes.Count; i++)
            {
                var tube = board.Tubes[i];
                Debug.Log(
                    $"Tube {i}: {(tube.IsEmpty ? "empty" : tube.TopSegment.ToString())} | Free: {tube.FreeSpace}"
                );
            }

            // Try a pour
            var result = board.TryPour(0, 6);
            Debug.Log($"Pour 0→6: {result}");
            Debug.Log($"Move count: {board.MoveCount}");

            // Try undo
            board.Undo();
            Debug.Log($"After undo, move count: {board.MoveCount}");
        }

        /*
        void Start()
        {
            var board = new BoardModel();
            board.Initialize(
                2,
                4,
                new[]
                {
                    new[]
                    {
                        new ColorSegment(TubeColor.Red, 2),
                        new ColorSegment(TubeColor.Blue, 2),
                    },
                    new ColorSegment[0], // empty tube
                }
            );

            Debug.Log(board.TryPour(0, 1)); // should be Success
            Debug.Log(board.Tubes[1].TopSegment); // should be [Blue x2]
            Debug.Log(board.CheckWin()); // should be false
        }

        */

        // Update is called once per frame
        void Update() { }
    }
}
