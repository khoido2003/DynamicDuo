using System.Collections.Generic;
using UnityEngine;

namespace Unity.DynamicDuo.Gameplay
{
    public class LoadingState : IGameState
    {
        private readonly GameStateMachine m_stateMachine;
        private readonly BoardModel m_boardModel;
        private readonly LevelData m_levelData;
        private readonly GameConfig m_gameConfig;

        public LoadingState(
            GameStateMachine stateMachine,
            BoardModel boardModel,
            LevelData levelData,
            GameConfig gameConfig
        )
        {
            m_stateMachine = stateMachine;
            m_boardModel = boardModel;
            m_levelData = levelData;
            m_gameConfig = gameConfig;
        }

        public void Enter()
        {
            Debug.Log($"[LoadingState] Loading level {m_levelData.LevelNumber}");

            if (!m_levelData.Validate(m_gameConfig, out string error))
            {
                Debug.LogError($"[LoadingState] Invalid level data {error}");
                return;
            }

            List<ColorSegment[]> segmentList = new();

            foreach (var tube in m_levelData.Tubes)
            {
                segmentList.Add(tube.ToSegments());
            }

            m_boardModel.Initialize(m_levelData.TubeCount, m_gameConfig.TubeCapacity, segmentList);

            Debug.Log($"[LoadingState] Board ready - {m_boardModel.Tubes.Count} tubes");

            m_stateMachine.TransitionTo<GameplayState>();
        }

        public void Exit()
        {
            Debug.Log("[LoadingState] Exit");
        }
    }
}
