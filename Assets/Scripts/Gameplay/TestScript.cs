using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class TestScript : MonoBehaviour
    {
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

        // Update is called once per frame
        void Update() { }
    }
}
