using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.DynamicDuo.Gameplay
{
    public class BoardModel
    {
        public IReadOnlyList<TubeModel> Tubes => m_tubesList;

        public int MoveCount { get; private set; }

        public bool CanUndo => m_undoBoardStack.Count > 0;

        private readonly List<TubeModel> m_tubesList = new();
        private readonly Stack<BoardSnapshot> m_undoBoardStack = new();

        #region Setup level

        public void Initialize(
            int tubeCount,
            int capacity,
            IEnumerable<IEnumerable<ColorSegment>> tubeSegments
        )
        {
            m_tubesList.Clear();
            m_undoBoardStack.Clear();

            MoveCount = 0;

            List<IEnumerable<ColorSegment>> segmentsList = tubeSegments.ToList();

            for (int i = 0; i < tubeCount; i++)
            {
                var tube = new TubeModel(i, capacity);

                if (i < segmentsList.Count)
                {
                    tube.LoadSegments(segmentsList[i]);
                }

                m_tubesList.Add(tube);
            }
        }

        #endregion


        # region Pour Logic

        public PourResult TryPour(int fromIndex, int toIndex)
        {
            if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex))
            {
                return PourResult.InvalidIndex;
            }

            TubeModel fromTube = m_tubesList[fromIndex];
            TubeModel toTube = m_tubesList[toIndex];

            int amount = toTube.GetPourAmount(fromTube);

            if (amount == 0)
            {
                return PourResult.InvalidMove;
            }

            m_undoBoardStack.Push(TakeSnapshot());

            toTube.ReceiveFrom(fromTube);
            MoveCount++;

            return PourResult.Success;
        }

        #endregion


        #region Win Logic

        public bool CheckWin()
        {
            return m_tubesList.All(t => t.IsEmpty || t.IsComplete);
        }

        // Check if there are no way to win
        public bool HasAnyValidMove()
        {
            foreach (var fromTube in m_tubesList)
            {
                foreach (var toTube in m_tubesList)
                {
                    if (fromTube.Index != toTube.Index && toTube.GetPourAmount(fromTube) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion


        # region Undo History

        public bool Undo()
        {
            if (m_undoBoardStack.Count == 0)
            {
                return false;
            }

            BoardSnapshot snapshot = m_undoBoardStack.Pop();

            foreach (TubeSnapshot tubeSnap in snapshot.TubeSnapshots)
            {
                m_tubesList[tubeSnap.TubeIndex].RestoreSnapshot(tubeSnap);
            }

            MoveCount = Math.Max(0, MoveCount - 1);

            return true;
        }

        #endregion

        #region Method Helpers

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < m_tubesList.Count;
        }

        private BoardSnapshot TakeSnapshot()
        {
            return new(m_tubesList.Select(t => t.TakeSnapshot()).ToArray());
        }

        #endregion
    }
}
